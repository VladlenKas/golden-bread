namespace GoldenBread.Application.Features.Document.Dtos;

public class GenerateCooperationAgreementResponse
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
}