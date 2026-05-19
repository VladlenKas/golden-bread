using GoldenBread.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace GoldenBread.Infrastructure.Services;

internal class FileStorage(IConfiguration configuration) : IFileStorage
{
    private readonly string _dbUploadPath = configuration["Data:DbUploadsPath"]!;

    public async Task<string> SaveAsync(Stream stream, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);

        string fileName = Guid.NewGuid().ToString("N") + extension;
        var fullPath = Path.Combine(_dbUploadPath, fileName);

        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream);

        return fileName;
    }   
 
    public Task<Stream> GetAsync(string fileName)
    {
        var fullPath = Path.Combine(_dbUploadPath, fileName);
        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    public Task DeleteAsync(string fileName)
    {
        var fullPath = Path.Combine(_dbUploadPath, fileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
