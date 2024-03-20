using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public partial class YoutubeView : UserControl
{
    readonly YoutubeViewModel _viewModel;
    
    public YoutubeView()
    {
        _viewModel = new YoutubeViewModel();
        this.DataContext = _viewModel;
        InitializeComponent();
    }
    
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
    void OnClickCopy( object sender, RoutedEventArgs e )
    {
        var button = ( Button ) sender;

        string url = button.CommandParameter is not null
            ? ( string )button.CommandParameter
            : string.Empty;
        
        _viewModel.CopyUrlCommand.Execute( url );
    }
    void CloseErrorMessage( object? sender, RoutedEventArgs args )
    {
        ErrorMessage.IsVisible = false;
        _viewModel.CloseError();
    }
}