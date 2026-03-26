using GoldenBread.Application.Features.ProductCatalog.Dtos;

namespace GoldenBread.Application.Features.Catalog.Queires.GetCatalog;

public sealed record class GetCatalogQuery :
    IRequest<CatalogResponse>;
