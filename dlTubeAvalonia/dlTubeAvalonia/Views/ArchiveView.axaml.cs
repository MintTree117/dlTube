using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class ArchiveView : UserControl
{
    readonly ArchiveViewModel _viewModel;
    
    public ArchiveView()
    {
        _viewModel = new ArchiveViewModel();
        this.DataContext = _viewModel;
        InitializeComponent();
    }
    
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
    void CloseErrorMessage( object? sender, RoutedEventArgs args )
    {
        ErrorMessage.IsVisible = false;
        _viewModel.CloseError();
    }
}