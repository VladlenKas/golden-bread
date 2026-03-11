namespace GoldenBread.Application.Abstractions.Services;

public interface IUniquenessChecker
{
	Task CompanyNameMustBeUniqueAsync(
		string name,
		int? excludeId = null,
		CancellationToken ct = default);

	Task CompanyPhoneMustBeUniqueAsync(
		string? phone,
		int? excludeId = null,
		CancellationToken ct = default);

	Task CompanyAddressMustBeUniqueAsync(
		string? address,
		int? excludeId = null,
		CancellationToken ct = default);

	Task CompanyInnMustBeUniqueAsync(
		string inn,
		int? excludeId = null,
		CancellationToken ct = default);

	Task CompanyOgrnMustBeUniqueAsync(
		string ogrn,
		int? excludeId = null,
		CancellationToken ct = default);

	Task EmailMustBeUniqueAsync(
		string email,
		int? excludeId = null,
		CancellationToken ct = default);
}

