using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Tests.Helpers;

// Runs all the [Required], [StringLength], [RegularExpression], [MinimumAge]... rules
// on an object, the same check [ApiController] does automatically in the real app.
public static class ValidationHelper
{
    public static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        // validateAllProperties: true = check every rule, not only [Required]
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        return results;
    }

    // True if any error belongs to the given property, e.g. "FirstName".
    public static bool HasErrorFor(this List<ValidationResult> results, string propertyName) =>
        results.Any(r => r.MemberNames.Contains(propertyName));
}
