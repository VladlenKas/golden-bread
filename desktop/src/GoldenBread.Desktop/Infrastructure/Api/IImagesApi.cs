using GoldenBread.Desktop.Features.References.Products.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IImagesApi
{
    [Multipart]
    [Post("/api/images")]
    Task<ApiResponse<ImageUploadResponse>> Upload([AliasAs("file")] ByteArrayPart file);
}
