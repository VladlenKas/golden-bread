using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Application.Features.CompanyCart.Commands.ToggleSelected;

public sealed record ToggleSelectedCommand(int ProductId)
    : IRequest<Unit>;
