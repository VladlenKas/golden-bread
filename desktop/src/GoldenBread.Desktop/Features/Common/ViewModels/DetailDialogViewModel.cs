using GoldenBread.Desktop.Features.Common.Models;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace GoldenBread.Desktop.Features.Common.ViewModels;

public class DetailDialogViewModel : ReactiveObject
{
    public ReadOnlyObservableCollection<DetailSectionViewModel> Sections { get; }

    public DetailDialogViewModel(DetailDialogData data)
    {
        var sections = new ObservableCollection<DetailSectionViewModel>(
            data.Sections.Select(s => new DetailSectionViewModel(s.Header, s.Fields)));

        Sections = new ReadOnlyObservableCollection<DetailSectionViewModel>(sections);
    }
}

public class DetailSectionViewModel : ReactiveObject
{
    public string Header { get; }
    public ReadOnlyObservableCollection<DetailFieldViewModel> Fields { get; }

    public DetailSectionViewModel(string header, IEnumerable<DetailFieldData> fields)
    {
        Header = header;

        var items = new ObservableCollection<DetailFieldViewModel>(
            fields.Select(f => new DetailFieldViewModel(f.Label, f.Value)));

        Fields = new ReadOnlyObservableCollection<DetailFieldViewModel>(items);
    }
}

public class DetailFieldViewModel(string label, string value) : ReactiveObject
{
    public string Label { get; } = label;
    public string Value { get; } = value;
}
