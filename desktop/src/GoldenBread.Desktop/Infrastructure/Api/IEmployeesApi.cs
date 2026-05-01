using GoldenBread.Desktop.Features.References.Employees.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IEmployeesApi
{
    [Get("/api/employees")]
    Task<IApiResponse<EmployeesListResponse>> GetAll();

    [Get("/api/employees/{id}")]
    Task<IApiResponse<EmployeeResponse>> GetById(int id);
}
