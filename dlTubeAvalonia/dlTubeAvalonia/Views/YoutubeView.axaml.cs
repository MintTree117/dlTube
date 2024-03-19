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
        InitializeComponent();
        _viewModel = new YoutubeViewModel();
        this.DataContext = _viewModel;
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
}