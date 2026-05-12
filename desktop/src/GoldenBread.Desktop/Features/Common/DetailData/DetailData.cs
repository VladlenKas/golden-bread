namespace GoldenBread.Desktop.Features.Common.DetailData;

public sealed record DetailDialogData(
    IReadOnlyList<DetailSectionData> Sections);

public sealed record DetailSectionData(
    string Header,
    IReadOnlyList<DetailFieldData> Fields);

public sealed record DetailFieldData(
    string Label, 
    string Value);
