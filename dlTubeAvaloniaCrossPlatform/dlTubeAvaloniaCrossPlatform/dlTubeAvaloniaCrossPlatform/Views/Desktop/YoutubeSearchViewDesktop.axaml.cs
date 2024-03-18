using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.ViewModels;

namespace dlTubeAvaloniaCrossPlatform.Views.Desktop;

public partial class YoutubeSearchViewDesktop : UserControl
{
    readonly YoutubeSearchViewModel _viewModel;
    
    public YoutubeSearchViewDesktop()
    {
        InitializeComponent();
        this.DataContext = new YoutubeSearchViewModel();
        _viewModel = (this.DataContext as YoutubeSearchViewModel)!;
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
    /*void OnClickPlay( object sender, RoutedEventArgs e )
    {
        Console.WriteLine( "Behind" );
        var button = ( Button )sender;

        if ( button.CommandParameter is null )
        {
            Console.WriteLine( "Command Parameter Is Null" );
            return;   
        }

        VideoId videoId = ( VideoId )button.CommandParameter;

        Console.WriteLine( videoId.Value );
        
        _viewModel.PlayCommand.Execute( videoId );
    }*/
    void OnClickCopy( object sender, RoutedEventArgs e )
    {
        var button = ( Button ) sender;

        if ( button.CommandParameter is null )
            return;
        
        string url = ( string )button.CommandParameter;
        _viewModel.CopyUrlCommand.Execute( url );
    }
}