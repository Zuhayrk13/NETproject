using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Validation;

namespace EmployeeManagement.Api.Dtos;

// What the client is ALLOWED to send when creating (POST) or updating (PUT) an employee.
// No Id here: the database generates it, so the client can never choose or overwrite one.
public class EmployeeRequestDto
{
    // Letters (any language, e.g. é, ç), with single spaces, hyphens or apostrophes between parts.
    private const string NamePattern = @"^\p{L}+(?:[ '’\-]\p{L}+)*$";

    // Lowercase only, one @, and a domain ending in at least 2 letters (e.g. .mu, .com).
    private const string EmailPattern = @"^[a-z0-9._%+\-]+@[a-z0-9.\-]+\.[a-z]{2,}$";

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    [RegularExpression(NamePattern,
        ErrorMessage = "First name must contain letters only. Spaces, hyphens and apostrophes are allowed.")]
    public string FirstName { get; set; } = string.Empty;

    // Optional: no [Required]. The regex is skipped automatically when it's empty.
    [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
    [RegularExpression(NamePattern,
        ErrorMessage = "Middle name must contain letters only. Spaces, hyphens and apostrophes are allowed.")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    [RegularExpression(NamePattern,
        ErrorMessage = "Last name must contain letters only. Spaces, hyphens and apostrophes are allowed.")]
    public string LastName { get; set; } = string.Empty;

    // DateOnly? (nullable) so that a MISSING date triggers [Required]
    // instead of silently becoming 0001-01-01.
    [Required(ErrorMessage = "Date of birth is required.")]
    [MinimumAge(18)]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    [RegularExpression(EmailPattern,
        ErrorMessage = "Please enter a valid email address using lowercase letters only.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters.")]
    public string JobTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    public EmployeeStatus? Status { get; set; }
}
