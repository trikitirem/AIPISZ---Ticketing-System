using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.Team;

/// <summary>
/// Agregat reprezentujący zespół specjalistów w systemie.
/// </summary>
public class Team : AggregateRoot<string>
{
    public string Name { get; private set; }
    public TicketCategory Specialization { get; private set; }
    public int MaxTickets { get; private set; }

    private readonly List<string> _specialistIds = new();

    private Team()
    {
        Name = string.Empty;
        Specialization = TicketCategory.GENERAL;
        MaxTickets = 0;
    }

    private Team(string id, string name, TicketCategory specialization, int maxTickets)
    {
        Id = id;
        Name = name;
        Specialization = specialization;
        MaxTickets = maxTickets;
    }

    public static Team Create(string id, string name, TicketCategory specialization, int maxTickets = 50)
    {
        var validator = new TeamValidator();
        var validationResult = validator.Validate((id, name, specialization, maxTickets));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("TEAM_DATA_VALIDATION_ERROR", errors);
        }

        return new Team(id, name, specialization, maxTickets);
    }

    public void AddSpecialist(string specialistId)
    {
        if (string.IsNullOrWhiteSpace(specialistId))
            throw new DomainExceptions.ValidationException("TEAM_SPECIALIST_DATA_VALIDATION_ERROR", "Specialist ID cannot be null or empty");

        if (_specialistIds.Contains(specialistId))
            throw new DomainExceptions.ConflictException("TEAM_SPECIALIST_DUPLICATE_ERROR", $"Specialist {specialistId} is already in the team");

        _specialistIds.Add(specialistId);
    }

    public void RemoveSpecialist(string specialistId)
    {
        if (string.IsNullOrWhiteSpace(specialistId))
            throw new DomainExceptions.ValidationException("TEAM_SPECIALIST_DATA_VALIDATION_ERROR", "Specialist ID cannot be null or empty");

        if (!_specialistIds.Contains(specialistId))
            throw new DomainExceptions.NotFoundException("TEAM_SPECIALIST_NOT_FOUND_ERROR", $"Specialist {specialistId} is not in the team");

        _specialistIds.Remove(specialistId);
    }

    public bool CanAcceptMore()
    {
        return _specialistIds.Count > 0;
    }

    public int GetSpecialistCount()
    {
        return _specialistIds.Count;
    }

    public IReadOnlyList<string> GetSpecialistIds()
    {
        return _specialistIds.AsReadOnly();
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        return new Dictionary<string, object>
        {
            { "Id", Id },
            { "Name", Name },
            { "Specialization", Specialization.ToString() },
            { "MaxTickets", MaxTickets },
            { "SpecialistIds", _specialistIds }
        };
    }
}
