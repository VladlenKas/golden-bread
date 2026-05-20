using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using GoldenBread.Desktop.Features.Production.OrdersList.ViewModels;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Features.Production.OrdersList.Views;

public partial class OrdersHostPageView : ReactiveUserControl<OrdersHostPageViewModel>
{
    private KanbanItem? _dragItem;
    private string? _dragSourceColumn;
    private Control? _originalCard;
    private Control? _dragGhost;
    private Point _dragOffset;
    private bool _isDragging;

    public OrdersHostPageView()
    {
        InitializeComponent();
    }

    private void OnCardPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (sender is not Control card || card.DataContext is not KanbanItem item)
            return;

        _dragItem = item;
        _dragSourceColumn = FindParentColumnTag(card);
        if (_dragSourceColumn == null)
            return;

        _originalCard = card;
        var bounds = card.Bounds;
        _dragOffset = new Point(bounds.Width / 2, bounds.Height / 2);

        // Ghost через VisualBrush — векторное зеркало, текст чёткий
        _dragGhost = new Border
        {
            Width = bounds.Width,
            Height = bounds.Height,
            Background = new VisualBrush(card)
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            },
            Opacity = 0.92,
            IsHitTestVisible = false,
        };

        DragLayer.Children.Add(_dragGhost);
        UpdateGhostPosition(e.GetPosition(this));

        _originalCard.Opacity = 0.35;

        // Подписываемся на глобальные события
        PointerMoved += OnDragPointerMoved;
        PointerReleased += OnDragPointerReleased;
        PointerCaptureLost += OnDragPointerCaptureLost;

        e.Handled = true;
    }

    private void OnDragPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragGhost == null || _dragItem == null)
            return;

        if (!_isDragging)
        {
            var diff = e.GetPosition(this) - e.GetPosition(_originalCard);
            if (Math.Abs(diff.X) < 3 && Math.Abs(diff.Y) < 3)
                return;
            _isDragging = true;
        }

        UpdateGhostPosition(e.GetPosition(this));

        var col = GetColumnTagAt(e.GetPosition(this));
        HighlightColumn(col);
    }

    private void OnDragPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        EndDrag(e.GetPosition(this));
    }

    private void OnDragPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        EndDrag(new Point(-1, -1)); // отмена
    }

    private async void EndDrag(Point pos)
    {
        PointerMoved -= OnDragPointerMoved;
        PointerReleased -= OnDragPointerReleased;
        PointerCaptureLost -= OnDragPointerCaptureLost;

        var targetColumn = pos.X < 0 ? null : GetColumnTagAt(pos);

        if (_dragGhost != null)
        {
            DragLayer.Children.Remove(_dragGhost);
            _dragGhost = null;
        }

        if (_originalCard != null)
            _originalCard.Opacity = 1.0;

        if (_dragItem != null
            && _dragSourceColumn != null
            && targetColumn != null
            && targetColumn != _dragSourceColumn
            && DataContext is OrdersHostPageViewModel vm)
        {
            bool confirmed = await vm.ConfirmMoveAsync(_dragSourceColumn, targetColumn, _dragItem);

            if (confirmed)
                vm.MoveItem(_dragSourceColumn, targetColumn, _dragItem);
        }

        ResetHighlights();

        _dragItem = null;
        _dragSourceColumn = null;
        _originalCard = null;
        _isDragging = false;
    }

    private void UpdateGhostPosition(Point pos)
    {
        if (_dragGhost == null) return;
        Canvas.SetLeft(_dragGhost, pos.X - _dragOffset.X);
        Canvas.SetTop(_dragGhost, pos.Y - _dragOffset.Y);
    }

    private string? GetColumnTagAt(Point position)
    {
        var hit = this.InputHitTest(position);
        if (hit is Visual visual)
        {
            var current = visual;
            while (current != null)
            {
                if (current is Border border && border.Tag is string tag)
                {
                    if (tag is "New" or "InProgress" or "Review" or "Archive")
                        return tag;
                }
                current = current.GetVisualParent();
            }
        }
        return null;
    }

    private static string? FindParentColumnTag(Control? control)
    {
        while (control != null)
        {
            if (control is Border border && border.Tag is string tag)
            {
                if (tag is "New" or "InProgress" or "Review" or "Archive")
                    return tag;
            }
            control = control.GetVisualParent() as Control;
        }
        return null;
    }

    private void HighlightColumn(string? activeTag)
    {
        foreach (var border in this.GetVisualDescendants().OfType<Border>())
        {
            if (border.Tag is not string tag) continue;

            border.Background = tag == activeTag
                ? (tag == "Archive"
                    ? new SolidColorBrush(Color.Parse("#25FF5252"))
                    : new SolidColorBrush(Color.Parse("#30FFFFFF")))
                : new SolidColorBrush(Color.Parse("#15FFFFFF"));
        }
    }

    private void ResetHighlights()
    {
        foreach (var border in this.GetVisualDescendants().OfType<Border>())
        {
            if (border.Tag is string)
                border.Background = new SolidColorBrush(Color.Parse("#15FFFFFF"));
        }
    }
}