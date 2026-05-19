using Avalonia.Media.Imaging;

namespace GoldenBread.Desktop.Features.References.Products.Models;

/// <summary>
/// Модель изображения продукта с загруженным Bitmap.
/// Хранит имя файла на сервере и готовый Bitmap для отображения.
/// </summary>
public class ProductImageItem(string fileName, Bitmap? bitmap)
{
    public string FileName { get; set; } = fileName;
    public Bitmap? Bitmap { get; set; } = bitmap;
}