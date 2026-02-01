using GoldenBread.Contracts.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Features.Auth.Commands;

public class RegisterCompanyCommand : IRequest<RegisterCompanyResponse?>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required string Inn { get; set; }
    public required string Ogrn { get; set; }
}
