using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TicketingSystem.Domain.Base;

namespace TicketingSystem.Infrastructure.Persistence;

/// <summary>
/// Abstrakcyjna klasa bazowa dla repozytoriów opartych na plikach JSON.
/// </summary>
/// <typeparam name="T">Typ agregatu</typeparam>
/// <typeparam name="TId">Typ identyfikatora agregatu</typeparam>
public abstract class FileBasedRepository<T, TId> : IRepository<T, TId> where T : AggregateRoot<TId>
{
    protected readonly string _dataFilePath;
    protected readonly ILogger<FileBasedRepository<T, TId>> _logger;

    protected FileBasedRepository(string dataFilePath, ILogger<FileBasedRepository<T, TId>> logger)
    {
        _dataFilePath = dataFilePath;
        _logger = logger;

        EnsureDirectoryExists();
        EnsureFileExists();
    }

    /// <summary>
    /// Upewnia się, że katalog dla pliku danych istnieje.
    /// </summary>
    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_dataFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            _logger.LogInformation("Created directory for data file: {Directory}", directory);
        }
    }

    /// <summary>
    /// Upewnia się, że plik danych istnieje. Jeśli nie istnieje, tworzy pusty plik JSON z pustą tablicą.
    /// </summary>
    private void EnsureFileExists()
    {
        if (!File.Exists(_dataFilePath))
        {
            var emptyJson = JsonSerializer.Serialize(new List<Dictionary<string, object>>(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_dataFilePath, emptyJson);
            _logger.LogInformation("Created empty data file: {FilePath}", _dataFilePath);
        }
    }

    /// <summary>
    /// Wczytuje wszystkie agregaty z pliku.
    /// </summary>
    protected virtual List<T> LoadFromFile()
    {
        if (!File.Exists(_dataFilePath))
        {
            _logger.LogInformation("Data file {FilePath} does not exist, returning empty list", _dataFilePath);
            return new List<T>();
        }

        try
        {
            var json = File.ReadAllText(_dataFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            var dataList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
            if (dataList is null)
            {
                return new List<T>();
            }

            var aggregates = new List<T>();
            foreach (var data in dataList)
            {
                var aggregate = FromPrimitive(data);
                if (aggregate is not null)
                {
                    aggregates.Add(aggregate);
                }
            }

            return aggregates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data from {FilePath}", _dataFilePath);
            throw;
        }
    }

    /// <summary>
    /// Zapisuje wszystkie agregaty do pliku.
    /// </summary>
    protected virtual void SaveToFile(List<T> aggregates)
    {
        try
        {
            var directory = Path.GetDirectoryName(_dataFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var dataList = aggregates.Select(a => a.ToPrimitive()).ToList();
            var json = JsonSerializer.Serialize(dataList, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving data to {FilePath}", _dataFilePath);
            throw;
        }
    }

    /// <summary>
    /// Konwertuje słownik danych na agregat.
    /// Musi być zaimplementowane w klasach pochodnych.
    /// </summary>
    protected abstract T? FromPrimitive(Dictionary<string, object> data);

    public virtual async Task<T?> GetByIdAsync(TId id)
    {
        var aggregates = LoadFromFile();
        return await Task.FromResult(aggregates.FirstOrDefault(a => a.Id!.Equals(id)));
    }

    public virtual async Task SaveAsync(T aggregate)
    {
        var aggregates = LoadFromFile();
        var existingIndex = aggregates.FindIndex(a => a.Id!.Equals(aggregate.Id));

        if (existingIndex >= 0)
        {
            aggregates[existingIndex] = aggregate;
        }
        else
        {
            aggregates.Add(aggregate);
        }

        SaveToFile(aggregates);
        aggregate.ClearUncommittedChanges();
        await Task.CompletedTask;
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        var aggregates = LoadFromFile();
        return await Task.FromResult(aggregates);
    }

    public virtual async Task DeleteAsync(TId id)
    {
        var aggregates = LoadFromFile();
        aggregates.RemoveAll(a => a.Id!.Equals(id));
        SaveToFile(aggregates);
        await Task.CompletedTask;
    }
}
