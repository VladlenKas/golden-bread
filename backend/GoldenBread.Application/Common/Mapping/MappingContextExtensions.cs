namespace GoldenBread.Application.Common.Mapping;

public static class MappingContextExtensions
{
    public static int CompanyId(this ResolutionContext context) 
        => context.Items.GetValueOrDefault("CompanyId", 0) as int? ?? 0;
}
