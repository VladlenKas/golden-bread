using GoldenBread.Application.Features.Catalog.Dtos;

namespace GoldenBread.Application.Features.Catalog.Queires.GetCatalog;

public sealed record GetCatalogQuery : IRequest<CatalogResponse>;
