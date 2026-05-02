using GoldenBread.Desktop.Features.References.Employees.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IEmployeesApi
{
    [Get("/api/employees")]
    Task<IApiResponse<EmployeesListResponse>> GetAll();

    [Get("/api/employees/{id}")]
    Task<IApiResponse<EmployeeForm>> GetById(int id);

    [Post("/api/employees")]
    Task<IApiResponse<int>> Create([Body] CreateEmployeeRequest command);

    [Put("/api/employees/{id}")]
    Task<IApiResponse> Update(int id, [Body] UpdateEmployeeRequest command);

    [Delete("/api/employees/{id}")]
    Task<IApiResponse> Delete(int id);
}
