using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Entities;

namespace EmployeeManagement.Tests.Helpers;

// Ready-made, VALID test objects. Each test starts from one of these and changes
// only the one field it is testing, so every test stays short and focused.
public static class TestData
{
    public static EmployeeRequestDto ValidRequest(string email = "alex.brown@bdo.mu") => new()
    {
        FirstName = "Alex",
        MiddleName = "James",
        LastName = "Brown",
        DateOfBirth = new DateOnly(1995, 6, 15),
        Email = email,
        Department = "Information Technology",
        JobTitle = "Software Engineer",
        Status = EmployeeStatus.Active
    };

    public static Employee ValidEmployee(string email = "existing@bdo.mu") => new()
    {
        FirstName = "Sam",
        LastName = "Taylor",
        DateOfBirth = new DateOnly(1990, 1, 20),
        Email = email,
        Department = "Finance",
        JobTitle = "Accountant",
        Status = EmployeeStatus.Active
    };
}
