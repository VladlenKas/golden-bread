using GoldenBread.Desktop.Features.References.Employees.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IEmployeesApi
{
    [Get("/api/employees")]
    Task<IApiResponse<EmployeesListResponse>> GetAll();

    [Get("/api/employees/{id}")]
    Task<IApiResponse<EmployeeDto>> GetById(int id);

    [Post("/api/employees")]
    Task<IApiResponse<int>> Create([Body] EmployeeDto dto);

    [Put("/api/employees")]
    Task<IApiResponse> Update([Body] EmployeeDto dto);

    [Delete("/api/employees/{id}")]
    Task<IApiResponse> Delete(int id);
}
