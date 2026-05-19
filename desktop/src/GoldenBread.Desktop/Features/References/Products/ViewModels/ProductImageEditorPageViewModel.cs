using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DynamicData;
using GoldenBread.Desktop.Configuration.Files;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Refit;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.References.Products.ViewModels;

#warning Перенести весь текст в константы
public partial class ProductImageEditorPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IProductsApi _api;
    private readonly IImagesApi _imagesApi;
    private readonly WindowService _windowService;
    private readonly ToastService _toastService;
    private readonly DialogService _dialogService;
    private readonly HttpClient _httpClient;

    [Reactive] private bool _isBusy;
    [Reactive] public bool _isEmpty = true;
    [Reactive] private ObservableCollection<ProductImageItem> _imagePaths = new();
    [Reactive] public int _productId;

    private List<ProductImageItem>? ImagePathsCache { get; set; }
    public string Title { get; set; } = "Редактирование изображений";

    public ProductImageEditorPageViewModel(
        IProductsApi api,
        IImagesApi imagesApi,
        GoldenBreadApiClient apiClient,
        WindowService windowService,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _imagesApi = imagesApi;
        _toastService = toastService;
        _windowService = windowService;
        _dialogService = dialogService;
        _httpClient = apiClient.HttpClient;

        this.WhenAnyValue(x => x.ProductId)
            .Subscribe(async id =>
            {
                Title = id > 0
                    ? "Редактирование изображений"
                    : "Изображения продукции";

                if (id > 0)
                    await LoadAsync(id);
            });

        this.WhenAnyValue(x => x.ImagePaths)
            .Subscribe(items =>
            {
                if (items == null)
                {
                    IsEmpty = true;
                    return;
                }

                IsEmpty = items.Count == 0;
                items.CollectionChanged += (_, __) => IsEmpty = items.Count == 0;
            });
    }

    private async Task LoadAsync(int id)
    {
        IsBusy = true;
        try
        {
            var response = await _api.GetDetail(id);
            if (!response.IsSuccessStatusCode || response.Content == null) return;

            var items = new List<ProductImageItem>();
            foreach (var url in response.Content.ImageUrls)
            {
                var bitmap = await LoadBitmapAsync(url);
                items.Add(new ProductImageItem(url, bitmap));
            }

            ImagePaths = new ObservableCollection<ProductImageItem>(items);
            ImagePathsCache = items.Select(i => new ProductImageItem(i.FileName, null)).ToList();
        }
        finally { IsBusy = false; }
    }

    private async Task<Bitmap?> LoadBitmapAsync(string fileName)
    {
        try
        {
            var fullUrl = $"{AppSettings.ApiDbUploadUrl}/{fileName}";
            var bytes = await _httpClient.GetByteArrayAsync(fullUrl);

            return await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var ms = new MemoryStream(bytes); 
                return new Bitmap(ms);
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Image load failed: {ex.Message}");
            return null;
        }
    }

    [ReactiveCommand]
    private async Task AddImageAsync()
    {
        if (ImagePaths.Count >= 5)
        {
            _toastService.ShowInfo("Максимум 5 изображений");
            return;
        }

        var topLevel = _windowService.GetMenuWindow();
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите изображение",
            AllowMultiple = true,
            FileTypeFilter = new[]
            {
            new FilePickerFileType("Images") { Patterns = new[] { "*.jpg", "*.jpeg", "*.png" } }
        }
        });

        if (files == null || files.Count == 0) return;

        var remainingSlots = 5 - ImagePaths.Count;
        var filesToProcess = files.Take(remainingSlots).ToList();

        if (files.Count > remainingSlots)
        {
            _toastService.ShowInfo($"Добавлено только {remainingSlots} из {files.Count} (лимит 5)");
        }

        foreach (var file in filesToProcess)
        {
            try
            {
                var localPath = file.TryGetLocalPath() ?? file.Path.AbsolutePath;

                await using var stream = await file.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var bytes = ms.ToArray();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var bitmapStream = new MemoryStream(bytes); // без using!
                    var bitmap = new Bitmap(bitmapStream);
                    ImagePaths.Add(new ProductImageItem(localPath, bitmap));
                });
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Ошибка загрузки превью: {ex.Message}");
            }
        }
    }

    [ReactiveCommand]
    private void RemoveImage(ProductImageItem? item)
    {
        if (item != null) ImagePaths.Remove(item);
    }

    [ReactiveCommand]
    public async Task<bool> SaveAsync()
    {
        // Сначала загружаем все локальные файлы на сервер
        var hasLocalFiles = ImagePaths.Any(i => IsLocalPath(i.FileName));
        if (hasLocalFiles)
        {
            var uploaded = await UploadLocalImagesAsync();
            if (uploaded == null) return false; // ошибка загрузки

            // Заменяем локальные пути на серверные имена
            ImagePaths.Clear();
            foreach (var (fileName, bitmap) in uploaded)
            {
                ImagePaths.Add(new ProductImageItem(fileName, bitmap));
            }
        }

        if (ProductId == 0)
        {
            // Режим создания: данные уйдут в CreateProductWithDetailsCommand
            return true;
        }

        // Режим редактирования: обновляем изображения в БД
        var response = await _api.UpdateImages(
            new UpdateProductImagesCommand(ProductId, ImagePaths.Select(i => i.FileName).ToList()));

        if (response.IsSuccessStatusCode)
        {
            _toastService.ShowSuccess(ConstantMessages.UpdatedToast);
            return true;
        }

        _toastService.ShowError();
        return false;
    }

    private bool IsLocalPath(string fileName)
    {
        return fileName.Contains(Path.DirectorySeparatorChar)
            || fileName.Contains('/')
            || fileName.Contains('\\');
    }

    [ReactiveCommand] public async Task GoBackAsync() { }

    private async Task<List<(string FileName, Bitmap? Bitmap)>>? UploadLocalImagesAsync()
    {
        var result = new List<(string, Bitmap?)>();
        IsBusy = true;

        try
        {
            foreach (var item in ImagePaths.ToList())
            {
                if (!IsLocalPath(item.FileName))
                {
                    result.Add((item.FileName, item.Bitmap));
                    continue;
                }

                try
                {
                    var fileName = Path.GetFileName(item.FileName);
                    var bytes = await File.ReadAllBytesAsync(item.FileName);

                    // ByteArrayPart — не StreamPart
                    var response = await _imagesApi.Upload(new ByteArrayPart(bytes, fileName, "image/jpeg"));

                    if (response.IsSuccessStatusCode && response.Content != null)
                    {
                        var serverName = response.Content.FileName;
                        var bitmap = await LoadBitmapAsync(serverName);
                        result.Add((serverName, bitmap));
                    }
                    else
                    {
                        Debug.WriteLine(response.Content);
                        Debug.WriteLine(response.StatusCode);
                        Debug.WriteLine(response.Error);
                        _toastService.ShowError($"Ошибка загрузки {fileName}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _toastService.ShowError($"Ошибка загрузки: {ex.Message}");
                    return null;
                }
            }
        }
        finally
        {
            IsBusy = false;
        }

        return result;
    }
}