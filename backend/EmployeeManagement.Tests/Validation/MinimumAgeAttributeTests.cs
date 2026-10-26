using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Api.Validation;

namespace EmployeeManagement.Tests.Validation;

// Tests the custom [MinimumAge(18)] rule on its own, including the edge cases
// around the 18th birthday.
public class MinimumAgeAttributeTests
{
    private readonly MinimumAgeAttribute _attribute = new(18);
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    // Runs the attribute and returns its result (null = valid).
    private ValidationResult? Check(object? value) =>
        _attribute.GetValidationResult(value, new ValidationContext(new object()));

    [Fact]
    public void Turning_18_Today_Is_Accepted()
    {
        var result = Check(Today.AddYears(-18));

        Assert.Null(result);   // null means "valid" (ValidationResult.Success)
    }

    [Fact]
    public void Turning_18_Tomorrow_Is_Rejected()
    {
        var result = Check(Today.AddYears(-18).AddDays(1));

        Assert.NotNull(result);
        Assert.Equal("Employee must be at least 18 years old.", result!.ErrorMessage);
    }

    [Fact]
    public void Adult_Is_Accepted()
    {
        var result = Check(Today.AddYears(-30));

        Assert.Null(result);   // null means "valid" (ValidationResult.Success)
    }

    [Fact]
    public void Future_Date_Is_Rejected()
    {
        var result = Check(Today.AddDays(1));

        Assert.NotNull(result);
        Assert.Equal("Date of birth cannot be in the future.", result!.ErrorMessage);
    }

    [Fact]
    public void Missing_Date_Is_Left_To_Required()
    {
        // A null date is [Required]'s job, so this rule must not report it twice.
        var result = Check(null);

        Assert.Null(result);   // null means "valid" (ValidationResult.Success)
    }
}
