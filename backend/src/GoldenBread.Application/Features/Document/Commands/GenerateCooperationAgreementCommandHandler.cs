using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Document.Dtos;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Document.Commands;

public class GenerateCooperationAgreementCommandHandler(
    ICompanyRepository companyRepository,
    IAccountRepository accountRepository,
    ICooperationAgreementGenerator generator) :
    IRequestHandler<GenerateCooperationAgreementCommand, GenerateCooperationAgreementResponse>
{
    public async Task<GenerateCooperationAgreementResponse> Handle(
        GenerateCooperationAgreementCommand command,
        CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(command.CompanyId, ct)
            ?? throw new InvalidOperationException("Компания не найдена");

        var company = await companyRepository.GetByIdAsync(account.Company!.CompanyId, ct)
            ?? throw new InvalidOperationException("Компания не найдена");

        var fileBytes = generator.Generate(company);

        return new GenerateCooperationAgreementResponse
        {
            FileBytes = fileBytes,
            FileName = $"Договор_{company.Name}_{DateTime.Now:dd.MM.yyyy}.pdf"
        };
    }
}

public class GenerateCooperationAgreementForRegisterCommandHandler(ICooperationAgreementGenerator generator) :
    IRequestHandler<GenerateCooperationAgreementForRegisterCommand, GenerateCooperationAgreementResponse>
{
    public async Task<GenerateCooperationAgreementResponse> Handle(
        GenerateCooperationAgreementForRegisterCommand command,
        CancellationToken ct)
    {
        var data = command.CompanyInfo;
        var company = Company.Create(0, 0, data.Name, data.Inn, data.Ogrn, data.Phone, data.Address);

        var fileBytes = generator.Generate(company);

        return new GenerateCooperationAgreementResponse
        {
            FileBytes = fileBytes,
            FileName = $"Договор_{company.Name}_{DateTime.Now:dd.MM.yyyy}.pdf"
        };
    }
}