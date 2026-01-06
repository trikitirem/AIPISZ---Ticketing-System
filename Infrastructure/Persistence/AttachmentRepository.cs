using Microsoft.Extensions.Logging;
using TicketingSystem.Domain.Aggregates.Ticket;

namespace TicketingSystem.Infrastructure.Persistence;

/// <summary>
/// Repozytorium dla załączników oparte na systemie plików.
/// </summary>
public class AttachmentRepository
{
    private readonly string _uploadDirectory;
    private readonly ILogger<AttachmentRepository> _logger;

    public AttachmentRepository(ILogger<AttachmentRepository> logger)
    {
        _uploadDirectory = "Data/uploads";
        _logger = logger;

        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    /// <summary>
    /// Zapisuje plik załącznika.
    /// </summary>
    public async Task<Attachment> SaveFileAsync(Attachment attachment, Stream fileStream)
    {
        try
        {
            var filePath = GetFilePath(attachment);
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStreamWriter = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamWriter);
            }

            return attachment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving attachment file {FileName}", attachment.FileName);
            throw;
        }
    }

    /// <summary>
    /// Pobiera plik załącznika jako stream.
    /// </summary>
    public async Task<Stream> GetFileAsync(Attachment attachment)
    {
        try
        {
            var filePath = GetFilePath(attachment);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Attachment file not found: {attachment.FileName}");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return await Task.FromResult<Stream>(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading attachment file {FileName}", attachment.FileName);
            throw;
        }
    }

    /// <summary>
    /// Usuwa plik załącznika.
    /// </summary>
    public async Task DeleteFileAsync(Attachment attachment)
    {
        try
        {
            var filePath = GetFilePath(attachment);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attachment file {FileName}", attachment.FileName);
            throw;
        }
    }

    private string GetFilePath(Attachment attachment)
    {
        return Path.Combine(_uploadDirectory, attachment.Id, attachment.FileName);
    }
}
