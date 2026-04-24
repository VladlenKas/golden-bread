namespace GoldenBread.Application.Abstractions.Services;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream stream);
    Task<Stream> GetAsync(string fileName);
}
