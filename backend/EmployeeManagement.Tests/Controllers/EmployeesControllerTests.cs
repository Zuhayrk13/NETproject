using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests.Controllers;

// Tests every endpoint of EmployeesController, using an in-memory database
// instead of SQL Server. Each test gets its own fresh, empty database.
public class EmployeesControllerTests
{
    private readonly AppDbContext _context;
    private readonly EmployeesController _controller;

    // xUnit creates a NEW instance of this class for every test,
    // so this constructor runs before each test (like JUnit's @BeforeEach).
    public EmployeesControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())   // unique name = isolated database
            .Options;

        _context = new AppDbContext(options);

        // Dependency Injection by hand: we give the controller our test database.
        _controller = new EmployeesController(_context);
    }

    // Puts an employee straight into the database and returns it (with its new Id).
    private async Task<Employee> SeedEmployeeAsync(string email = "existing@bdo.mu")
    {
        var employee = TestData.ValidEmployee(email);
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    // ---------- GET /api/employees ----------

    [Fact]
    public async Task GetEmployees_Returns_200_With_Empty_List_When_No_Employees()
    {
        var result = await _controller.GetEmployees();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var employees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponseDto>>(ok.Value);
        Assert.Empty(employees);
    }

    [Fact]
    public async Task GetEmployees_Returns_All_Employees()
    {
        await SeedEmployeeAsync("first@bdo.mu");
        await SeedEmployeeAsync("second@bdo.mu");

        var result = await _controller.GetEmployees();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var employees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponseDto>>(ok.Value);
        Assert.Equal(2, employees.Count());
    }

    // ---------- GET /api/employees/{id} ----------

    [Fact]
    public async Task GetEmployee_Returns_200_When_Found()
    {
        var existing = await SeedEmployeeAsync();

        var result = await _controller.GetEmployee(existing.Id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var employee = Assert.IsType<EmployeeResponseDto>(ok.Value);
        Assert.Equal(existing.Id, employee.Id);
        Assert.Equal("existing@bdo.mu", employee.Email);
    }

    [Fact]
    public async Task GetEmployee_Returns_404_When_Not_Found()
    {
        var result = await _controller.GetEmployee(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // ---------- POST /api/employees ----------

    [Fact]
    public async Task CreateEmployee_Returns_201_And_Saves_To_Database()
    {
        var dto = TestData.ValidRequest();

        var result = await _controller.CreateEmployee(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);

        var employee = Assert.IsType<EmployeeResponseDto>(created.Value);
        Assert.True(employee.Id > 0);                       // the database generated an Id
        Assert.Equal(1, await _context.Employees.CountAsync());
    }

    [Fact]
    public async Task CreateEmployee_Trims_Values_And_Stores_Empty_Middle_Name_As_Null()
    {
        var dto = TestData.ValidRequest();
        dto.FirstName = "  Alex  ";
        dto.MiddleName = "   ";

        await _controller.CreateEmployee(dto);

        var saved = await _context.Employees.SingleAsync();
        Assert.Equal("Alex", saved.FirstName);
        Assert.Null(saved.MiddleName);
    }

    [Fact]
    public async Task CreateEmployee_Returns_409_When_Email_Already_Exists()
    {
        await SeedEmployeeAsync("taken@bdo.mu");
        var dto = TestData.ValidRequest("taken@bdo.mu");

        var result = await _controller.CreateEmployee(dto);

        Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(1, await _context.Employees.CountAsync());   // nothing new was saved
    }

    // ---------- PUT /api/employees/{id} ----------

    [Fact]
    public async Task UpdateEmployee_Returns_204_And_Updates_Database()
    {
        var existing = await SeedEmployeeAsync();
        var dto = TestData.ValidRequest("existing@bdo.mu");
        dto.JobTitle = "Senior Accountant";
        dto.Status = EmployeeStatus.Inactive;

        var result = await _controller.UpdateEmployee(existing.Id, dto);

        Assert.IsType<NoContentResult>(result);
        var updated = await _context.Employees.FindAsync(existing.Id);
        Assert.Equal("Senior Accountant", updated!.JobTitle);
        Assert.Equal(EmployeeStatus.Inactive, updated.Status);
    }

    [Fact]
    public async Task UpdateEmployee_Allows_Keeping_The_Same_Email()
    {
        // Saving an employee without changing their email must not count as a duplicate.
        var existing = await SeedEmployeeAsync("same@bdo.mu");
        var dto = TestData.ValidRequest("same@bdo.mu");

        var result = await _controller.UpdateEmployee(existing.Id, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateEmployee_Returns_409_When_Email_Belongs_To_Another_Employee()
    {
        await SeedEmployeeAsync("other@bdo.mu");
        var employee = await SeedEmployeeAsync("mine@bdo.mu");
        var dto = TestData.ValidRequest("other@bdo.mu");

        var result = await _controller.UpdateEmployee(employee.Id, dto);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task UpdateEmployee_Returns_404_When_Not_Found()
    {
        var result = await _controller.UpdateEmployee(999, TestData.ValidRequest());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ---------- DELETE /api/employees/{id} ----------

    [Fact]
    public async Task DeleteEmployee_Returns_204_And_Removes_From_Database()
    {
        var existing = await SeedEmployeeAsync();

        var result = await _controller.DeleteEmployee(existing.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, await _context.Employees.CountAsync());
    }

    [Fact]
    public async Task DeleteEmployee_Returns_404_When_Not_Found()
    {
        var result = await _controller.DeleteEmployee(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
