namespace GoldenBread.Desktop.Features.Administration.Companies.Models;

public record CreateCompanyCommand(CompanyDto CompanyDto, string Email, string Password);
