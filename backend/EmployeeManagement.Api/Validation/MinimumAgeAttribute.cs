using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Validation;

// A custom validation attribute: lets us write [MinimumAge(18)] on a property,
// just like the built-in [Required] or [StringLength].
public class MinimumAgeAttribute : ValidationAttribute
{
    public int Years { get; }

    public MinimumAgeAttribute(int years)
    {
        Years = years;
        ErrorMessage = $"Employee must be at least {years} years old.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // A missing date is [Required]'s job, not ours.
        if (value is not DateOnly dateOfBirth)
        {
            return ValidationResult.Success;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        // Link the error to the property it belongs to (e.g. "DateOfBirth"),
        // the same way the built-in attributes do.
        var memberNames = validationContext.MemberName is null
            ? null
            : new[] { validationContext.MemberName };

        if (dateOfBirth > today)
        {
            return new ValidationResult("Date of birth cannot be in the future.", memberNames);
        }

        // Their 18th birthday must be today or earlier.
        if (dateOfBirth.AddYears(Years) > today)
        {
            return new ValidationResult(ErrorMessage, memberNames);
        }

        return ValidationResult.Success;
    }
}
