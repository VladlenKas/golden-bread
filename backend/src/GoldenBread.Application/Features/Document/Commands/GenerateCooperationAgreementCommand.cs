using GoldenBread.Application.Features.Document.Dtos;

namespace GoldenBread.Application.Features.Document.Commands;

public sealed record GenerateCooperationAgreementCommand(int CompanyId)
    : IRequest<GenerateCooperationAgreementResponse>;

public sealed record GenerateCooperationAgreementForRegisterCommand(CompanyInfoDto CompanyInfo)
    : IRequest<GenerateCooperationAgreementResponse>;