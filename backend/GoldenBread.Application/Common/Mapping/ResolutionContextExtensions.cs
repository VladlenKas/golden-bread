namespace GoldenBread.Application.Common.Mapping;

public static class ResolutionContextExtensions
{
    public static int GetCompanyId(this ResolutionContext context)
    {
        return context.Items.TryGetValue("CompanyId", out var id) && id is int cid ? cid : 0;
    }
}
