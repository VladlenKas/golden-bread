using GoldenBread.Application.Services;
using Microsoft.Extensions.Configuration;

namespace GoldenBread.Infrastructure.Services;

internal class FileStorage : IFileStorage
{
    private readonly string _dbUploadPath;
    public FileStorage(IConfiguration configuration)
    {
        _dbUploadPath = configuration["Data:DbUploadPath"]!;

        if (File.Exists(_dbUploadPath))
        {
            Directory.CreateDirectory(_dbUploadPath);
        }
    }

    public async Task<string> SaveAsync(Stream stream)
    {
        string fileName = Guid.NewGuid().ToString("N");
        var fullPath = Path.Combine(_dbUploadPath, fileName);

        await using var fileStream = File.Create(fullPath);
        await fileStream.CopyToAsync(stream);

        return fileName;
    }   
 
    public Task<Stream> GetAsync(string fileName)
    {
        var fullPath = Path.Combine(_dbUploadPath, fileName);
        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }
}
