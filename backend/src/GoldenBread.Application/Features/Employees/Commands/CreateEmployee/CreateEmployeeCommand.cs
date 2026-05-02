using GoldenBread.Application.Features.CompanyOrder.Dtos;

namespace GoldenBread.Application.Features.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand(
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday) : IRequest<int>;