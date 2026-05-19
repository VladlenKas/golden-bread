namespace GoldenBread.Application.Abstractions.Services;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream stream, string originalFileName);
    Task<Stream> GetAsync(string fileName);
    Task DeleteAsync(string fileName);
}
