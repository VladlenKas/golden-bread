using GoldenBread.Desktop.Features.Production.EmployeeTasksList.Dtos;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IEmployeeTasksApi
{
    [Get("/api/employee-tasks/kanban")]
    Task<IApiResponse<List<EmployeeTaskKanbanItem>>> GetKanban();

    [Put("/api/employee-tasks/status")]
    Task<IApiResponse> UpdateStatus([Body] UpdateEmployeeTaskStatusRequest request);

    [Put("/api/employee-tasks/progress")]
    Task<IApiResponse> UpdateProgress([Body] UpdateEmployeeTaskProgressRequest request);

    [Get("/api/employee-tasks/{id}")]
    Task<IApiResponse<EmployeeTaskDetailResponse>> GetById(int id);
}