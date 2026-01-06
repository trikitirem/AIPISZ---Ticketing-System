using System.Reflection;
using Microsoft.Extensions.Logging;
using TicketingSystem.Domain.Aggregates.Ticket;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Infrastructure.Persistence;

/// <summary>
/// Repozytorium dla agregatu Ticket oparte na plikach JSON.
/// </summary>
public class TicketRepository : FileBasedRepository<Ticket, string>
{
    public TicketRepository(ILogger<TicketRepository> logger)
        : base("Data/tickets.json", logger)
    {
    }

    protected override Ticket? FromPrimitive(Dictionary<string, object> data)
    {
        try
        {
            var id = data["Id"].ToString()!;
            var number = TicketNumber.Create(data["Number"].ToString()!);
            var title = data["Title"].ToString()!;
            var description = data["Description"].ToString()!;
            var category = Enum.Parse<TicketCategory>(data["Category"].ToString()!);
            var priorityLevel = Enum.Parse<PriorityLevel>(data["Priority"].ToString()!);
            var priority = Priority.Create(priorityLevel);
            var createdById = data["CreatedById"].ToString()!;

            var ticket = CreateTicketInstance(id, number, title, description, category, priority, createdById);

            if (data.TryGetValue("Status", out var statusObj))
            {
                var status = Enum.Parse<TicketStatus>(statusObj.ToString()!);
                SetPrivateProperty(ticket, nameof(Ticket.Status), status);
            }

            if (data.TryGetValue("AssignedTeamId", out var teamIdObj) && teamIdObj is not null)
            {
                SetPrivateProperty(ticket, nameof(Ticket.AssignedTeamId), teamIdObj.ToString());
            }

            if (data.TryGetValue("AssignedSpecialistId", out var specialistIdObj) && specialistIdObj is not null)
            {
                SetPrivateProperty(ticket, nameof(Ticket.AssignedSpecialistId), specialistIdObj.ToString());
            }

            if (data.TryGetValue("CreatedAt", out var createdAtObj))
            {
                var createdAt = DateTime.Parse(createdAtObj.ToString()!);
                SetPrivateProperty(ticket, nameof(Ticket.CreatedAt), createdAt);
            }

            if (data.TryGetValue("UpdatedAt", out var updatedAtObj))
            {
                var updatedAt = DateTime.Parse(updatedAtObj.ToString()!);
                SetPrivateProperty(ticket, nameof(Ticket.UpdatedAt), updatedAt);
            }

            if (data.TryGetValue("ResolvedAt", out var resolvedAtObj) && resolvedAtObj is not null)
            {
                var resolvedAt = DateTime.Parse(resolvedAtObj.ToString()!);
                SetPrivateProperty(ticket, nameof(Ticket.ResolvedAt), resolvedAt);
            }

            return ticket;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing Ticket from primitive data");
            return null;
        }
    }

    private static Ticket CreateTicketInstance(string id, TicketNumber number, string title, string description, TicketCategory category, Priority priority, string createdById)
    {
        var constructor = typeof(Ticket).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(string), typeof(TicketNumber), typeof(string), typeof(string), typeof(TicketCategory), typeof(Priority), typeof(string) },
            null);

        if (constructor is null)
        {
            throw new InvalidOperationException("CANNOT_FIND_PRIVATE_CONSTRUCTOR");
        }

        return (Ticket)constructor.Invoke(new object[] { id, number, title, description, category, priority, createdById });
    }

    private static void SetPrivateProperty(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property is not null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
    }

    public async Task<Ticket?> GetByNumberAsync(string number)
    {
        var tickets = await GetAllAsync();
        return tickets.FirstOrDefault(t => t.Number.Value == number);
    }

    public async Task<List<Ticket>> GetByStatusAsync(TicketStatus status)
    {
        var tickets = await GetAllAsync();
        return tickets.Where(t => t.Status == status).ToList();
    }

    public async Task<List<Ticket>> GetByAssignedSpecialistAsync(string specialistId)
    {
        var tickets = await GetAllAsync();
        return tickets.Where(t => t.AssignedSpecialistId == specialistId).ToList();
    }

    public async Task<List<Ticket>> GetByTeamAsync(string teamId)
    {
        var tickets = await GetAllAsync();
        return tickets.Where(t => t.AssignedTeamId == teamId).ToList();
    }

    public async Task<List<Ticket>> GetByCategoryAsync(TicketCategory category)
    {
        var tickets = await GetAllAsync();
        return tickets.Where(t => t.Category == category).ToList();
    }
}
