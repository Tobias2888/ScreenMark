using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace ScreenMark;

public partial class MainWindow : Window
{
    private Point _startPoint;
    private Point _endPoint;
    private Rect _preview;
    private bool _drawing;
    private List<Rect> _rects = new();
    private List<Rect> _removedRects = new();
    
    public MainWindow() => InitializeComponent();

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        KeyDown += OnKeyDown;
        PointerMoved += OnPointerMoved;
        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
    }
    
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _startPoint = e.GetPosition(this);
        _drawing = true;
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _drawing = false;
        _rects.Add(_preview);
        _removedRects.Clear();
        _preview = new Rect(0, 0, 0, 0);
        InvalidateVisual();
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_drawing)
        {
            return;
        }
        
        _endPoint = e.GetPosition(this);
        _preview = new Rect(
            Math.Min(_startPoint.X, _endPoint.X), 
            Math.Min(_startPoint.Y, _endPoint.Y),
            Math.Abs(_endPoint.X - _startPoint.X), 
            Math.Abs(_endPoint.Y - _startPoint.Y));
        InvalidateVisual();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }

        if (e.Key == Key.Z && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            if (_rects.Count == 0)
            {
                return;
            }
            e.Handled = true;
            _removedRects.Add(_rects.Last());
            _rects.RemoveAt(_rects.Count - 1);
            InvalidateVisual();
        }

        if (e.Key == Key.Y && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            if (_removedRects.Count == 0)
            {
                return;
            }
            e.Handled = true;
            _rects.Add(_removedRects.Last());
            _removedRects.RemoveAt(_removedRects.Count - 1);
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_preview != null)
        {
            context.DrawRectangle(new Pen(Brushes.Red, 5), _preview);
        }

        foreach (Rect rect in _rects)
        {
            context.DrawRectangle(new Pen(Brushes.Red, 5), rect);
        }
    }
}