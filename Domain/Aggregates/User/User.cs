using System.Collections.Generic;
using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Validators;
using TicketingSystem.Domain.ValueObjects;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.User;

/// <summary>
/// Abstrakcyjna klasa bazowa reprezentująca użytkownika w systemie.
/// </summary>
public abstract class User : AggregateRoot
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public AccountStatus AccountStatus { get; private set; }

    protected User()
    {
        Email = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        AccountStatus = AccountStatus.Create(AccountStatusEnum.INACTIVE);
    }

    protected User(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        AccountStatus = AccountStatus.Create(accountStatus);
    }

    protected static void ValidateUserData(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus)
    {
        var validator = new UserValidator();
        var validationResult = validator.Validate((id, email, firstName, lastName, accountStatus));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("USER_DATA_VALIDATION_ERROR", errors);
        }
    }

    public string GetEmail()
    {
        return Email;
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    public bool IsActive()
    {
        return AccountStatus.Status == AccountStatusEnum.ACTIVE;
    }

    public abstract UserType GetUserType();

    public override Dictionary<string, object> ToPrimitive()
    {
        return new Dictionary<string, object>
        {
            { "Id", Id },
            { "Email", Email },
            { "FirstName", FirstName },
            { "LastName", LastName },
            { "AccountStatus", AccountStatus.Status.ToString() },
            { "UserType", GetUserType().ToString() }
        };
    }
}
