using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public partial class YoutubeSearchView : UserControl
{
    public YoutubeSearchView()
    {
        this.DataContext = new YoutubeSearchViewModel();
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
    void OnClickCopy( object sender, RoutedEventArgs e )
    {
        var button = ( Button ) sender;
        string? url = ( string )button.CommandParameter;
        var viewModel = ( YoutubeSearchViewModel )this.DataContext;
        viewModel.CopyUrlCommand.Execute( url );
    }
}