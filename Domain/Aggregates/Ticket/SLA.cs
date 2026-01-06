using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.ValueObjects;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Service Level Agreement (SLA) dla zgłoszenia jako Value Object.
/// </summary>
public class SLA : ValueObject
{
    public TimeSpan ReactionTime { get; private set; }
    public TimeSpan ResolutionTime { get; private set; }
    public Priority Priority { get; private set; }

    private SLA()
    {
        Priority = Priority.Create(Domain.Enums.PriorityLevel.NISKI);
    }

    private SLA(TimeSpan reactionTime, TimeSpan resolutionTime, Priority priority)
    {
        ReactionTime = reactionTime;
        ResolutionTime = resolutionTime;
        Priority = priority;
    }

    public static SLA Create(Priority priority)
    {
        if (priority is null)
            throw new DomainExceptions.ValidationException("SLA_DATA_VALIDATION_ERROR", "Priority cannot be null");

        var (reactionTime, resolutionTime) = GetSLATimes(priority.Level);
        return new SLA(reactionTime, resolutionTime, priority);
    }

    public TimeSpan GetReactionTime()
    {
        return ReactionTime;
    }

    public TimeSpan GetResolutionTime()
    {
        return ResolutionTime;
    }

    private static (TimeSpan reactionTime, TimeSpan resolutionTime) GetSLATimes(Domain.Enums.PriorityLevel priority)
    {
        return priority switch
        {
            Domain.Enums.PriorityLevel.NISKI => (TimeSpan.FromHours(48), TimeSpan.FromDays(7)),
            Domain.Enums.PriorityLevel.SREDNI => (TimeSpan.FromHours(24), TimeSpan.FromDays(3)),
            Domain.Enums.PriorityLevel.WYSOKI => (TimeSpan.FromHours(4), TimeSpan.FromDays(1)),
            Domain.Enums.PriorityLevel.KRYTYCZNY => (TimeSpan.FromHours(1), TimeSpan.FromHours(4)),
            _ => throw new DomainExceptions.ValidationException("SLA_DATA_VALIDATION_ERROR", $"Unknown priority level: {priority}")
        };
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not SLA otherSla)
            return false;

        return ReactionTime == otherSla.ReactionTime &&
               ResolutionTime == otherSla.ResolutionTime &&
               Priority.Equals(otherSla.Priority);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ReactionTime, ResolutionTime, Priority);
    }
}
