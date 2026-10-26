using EmployeeManagement.Api.Entities;

namespace EmployeeManagement.Api.Dtos;

// What the API SENDS BACK to the client.
// Kept separate from the entity so internal database details are never exposed by accident.
public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public EmployeeStatus Status { get; set; }

    // Converts an entity (database row) into a response DTO.
    public static EmployeeResponseDto FromEntity(Employee employee) => new()
    {
        Id = employee.Id,
        FirstName = employee.FirstName,
        MiddleName = employee.MiddleName,
        LastName = employee.LastName,
        DateOfBirth = employee.DateOfBirth,
        Email = employee.Email,
        Department = employee.Department,
        JobTitle = employee.JobTitle,
        Status = employee.Status
    };
}
