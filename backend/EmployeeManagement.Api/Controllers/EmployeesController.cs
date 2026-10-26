using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]                 // auto-validates DTOs and returns 400 if a rule fails
[Route("api/[controller]")]     // => /api/employees
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    // Dependency Injection: ASP.NET Core hands us the AppDbContext registered in Program.cs.
    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/employees  -> 200 OK with the list
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployees()
    {
        var employees = await _context.Employees
            .OrderBy(e => e.Id)
            .ToListAsync();

        return Ok(employees.Select(EmployeeResponseDto.FromEntity));
    }

    // GET /api/employees/5  -> 200 OK, or 404 if it doesn't exist
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound(new { error = $"Employee with ID {id} was not found." });
        }

        return Ok(EmployeeResponseDto.FromEntity(employee));
    }

    // POST /api/employees  -> 201 Created, 400 if invalid, 409 if the email is taken
    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee(EmployeeRequestDto dto)
    {
        // If we get here, every DTO rule has already passed.
        var email = dto.Email.Trim();

        if (await _context.Employees.AnyAsync(e => e.Email == email))
        {
            return Conflict(new { error = $"An employee with email '{email}' already exists." });
        }

        var employee = new Employee();
        ApplyDto(employee, dto);

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();   // the INSERT happens here; employee.Id is now filled in

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id },
            EmployeeResponseDto.FromEntity(employee));
    }

    // PUT /api/employees/5  -> 204 No Content, 400 if invalid, 404 if missing, 409 if email taken
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeRequestDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound(new { error = $"Employee with ID {id} was not found." });
        }

        var email = dto.Email.Trim();

        // Duplicate check must ignore THIS employee, otherwise saving without changing
        // the email would clash with itself.
        if (await _context.Employees.AnyAsync(e => e.Email == email && e.Id != id))
        {
            return Conflict(new { error = $"An employee with email '{email}' already exists." });
        }

        ApplyDto(employee, dto);
        await _context.SaveChangesAsync();   // EF Core tracked the changes and sends an UPDATE

        return NoContent();
    }

    // DELETE /api/employees/5  -> 204 No Content, or 404 if missing
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound(new { error = $"Employee with ID {id} was not found." });
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();   // the DELETE happens here

        return NoContent();
    }

    // Copies the validated DTO values onto the entity (shared by POST and PUT).
    private static void ApplyDto(Employee employee, EmployeeRequestDto dto)
    {
        employee.FirstName = dto.FirstName.Trim();
        employee.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.DateOfBirth = dto.DateOfBirth!.Value;   // [Required] guarantees it's not null
        employee.Email = dto.Email.Trim();
        employee.Department = dto.Department.Trim();
        employee.JobTitle = dto.JobTitle.Trim();
        employee.Status = dto.Status!.Value;
    }
}
