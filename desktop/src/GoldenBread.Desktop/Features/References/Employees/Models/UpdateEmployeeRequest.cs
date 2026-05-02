using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Features.References.Employees.Models;

public sealed record UpdateEmployeeRequest(
    int EmployeeId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday);