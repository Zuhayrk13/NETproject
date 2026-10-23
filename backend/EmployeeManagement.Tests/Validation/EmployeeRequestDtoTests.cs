using EmployeeManagement.Tests.Helpers;

namespace EmployeeManagement.Tests.Validation;

// Tests the validation rules on EmployeeRequestDto (what the client is allowed to send).
public class EmployeeRequestDtoTests
{
    [Fact]
    public void Valid_Request_Has_No_Errors()
    {
        var dto = TestData.ValidRequest();

        var errors = ValidationHelper.Validate(dto);

        Assert.Empty(errors);
    }

    // ---------- Names ----------

    [Theory]
    [InlineData("Zuhayr")]
    [InlineData("Jean-Pierre")]
    [InlineData("O'Neil")]
    [InlineData("Zoë")]
    [InlineData("Mary Ann")]
    public void Valid_First_Names_Are_Accepted(string firstName)
    {
        var dto = TestData.ValidRequest();
        dto.FirstName = firstName;

        var errors = ValidationHelper.Validate(dto);

        Assert.False(errors.HasErrorFor("FirstName"));
    }

    [Theory]
    [InlineData("Jo1hn")]        // digit
    [InlineData("J@mes")]        // symbol
    [InlineData("John  Smith")]  // double space
    [InlineData("-John")]        // starts with a hyphen
    [InlineData("John-")]        // ends with a hyphen
    [InlineData("")]             // empty
    [InlineData("   ")]          // only spaces
    public void Invalid_First_Names_Are_Rejected(string firstName)
    {
        var dto = TestData.ValidRequest();
        dto.FirstName = firstName;

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("FirstName"));
    }

    [Theory]
    [InlineData("Jo1hn")]
    [InlineData("")]
    public void Invalid_Last_Names_Are_Rejected(string lastName)
    {
        var dto = TestData.ValidRequest();
        dto.LastName = lastName;

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("LastName"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Middle_Name_Is_Optional(string? middleName)
    {
        var dto = TestData.ValidRequest();
        dto.MiddleName = middleName;

        var errors = ValidationHelper.Validate(dto);

        Assert.False(errors.HasErrorFor("MiddleName"));
    }

    [Fact]
    public void Middle_Name_With_Symbols_Is_Rejected()
    {
        var dto = TestData.ValidRequest();
        dto.MiddleName = "J@mes";

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("MiddleName"));
    }

    [Fact]
    public void First_Name_Longer_Than_50_Characters_Is_Rejected()
    {
        var dto = TestData.ValidRequest();
        dto.FirstName = new string('a', 51);

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("FirstName"));
    }

    // ---------- Email ----------

    [Theory]
    [InlineData("alex@bdo.mu")]
    [InlineData("alex.brown@company.com")]
    [InlineData("a_b+test@sub.domain.org")]
    public void Valid_Emails_Are_Accepted(string email)
    {
        var dto = TestData.ValidRequest(email);

        var errors = ValidationHelper.Validate(dto);

        Assert.False(errors.HasErrorFor("Email"));
    }

    [Theory]
    [InlineData("Alex@bdo.mu")]    // uppercase
    [InlineData("alex.bdo.mu")]    // no @
    [InlineData("alex@")]          // no domain
    [InlineData("alex@bdo")]       // no dot in the domain
    [InlineData("alex @bdo.mu")]   // space
    [InlineData("")]               // empty
    public void Invalid_Emails_Are_Rejected(string email)
    {
        var dto = TestData.ValidRequest(email);

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("Email"));
    }

    // ---------- Other required fields ----------

    [Fact]
    public void Missing_Date_Of_Birth_Is_Rejected()
    {
        var dto = TestData.ValidRequest();
        dto.DateOfBirth = null;

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("DateOfBirth"));
    }

    [Fact]
    public void Employee_Under_18_Is_Rejected()
    {
        var dto = TestData.ValidRequest();
        dto.DateOfBirth = DateOnly.FromDateTime(DateTime.Today).AddYears(-17);

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("DateOfBirth"));
    }

    [Fact]
    public void Missing_Status_Is_Rejected()
    {
        var dto = TestData.ValidRequest();
        dto.Status = null;

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("Status"));
    }

    [Fact]
    public void Empty_Department_Is_Rejected()
    {
        var dto = TestData.ValidRequest();
        dto.Department = "";

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("Department"));
    }

    [Fact]
    public void Empty_Job_Title_Is_Rejected()
    {
        var dto = TestData.ValidRequest();
        dto.JobTitle = "";

        var errors = ValidationHelper.Validate(dto);

        Assert.True(errors.HasErrorFor("JobTitle"));
    }
}
