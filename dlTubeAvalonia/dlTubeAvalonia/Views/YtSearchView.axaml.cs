using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public partial class YtSearchView : UserControl
{
    readonly YtSearchViewModel _viewModel;
    
    public YtSearchView()
    {
        InitializeComponent();
        _viewModel = new YtSearchViewModel();
        this.DataContext = _viewModel;
    }
    
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
    void OnClickLink( object sender, RoutedEventArgs e )
    {
        var button = ( Button ) sender;

        string url = button.CommandParameter is not null
            ? ( string ) button.CommandParameter
            : string.Empty;
        
        GoToYoutube( url );
        //_viewModel.OpenYoutubeCommand.Execute( "https://www.youtube.com/watch?v=e6-HossxeWc&t=110s" );
    }
    void OnClickCopy( object sender, RoutedEventArgs e )
    {
        var button = ( Button ) sender;

        string url = button.CommandParameter is not null
            ? ( string )button.CommandParameter
            : string.Empty;
        
        _viewModel.CopyUrlCommand.Execute( url );
    }
    
    // Here instead of view model because of weird binding issue: as of this comment Avalonia still has quirks
    void GoToYoutube( string url )
    {
        try
        {
            ProcessStartInfo psi = new()
            {
                FileName = url,
                UseShellExecute = true // Important for .NET Core
            };

            Process.Start( psi );
        }
        catch ( Exception e )
        {
            Console.WriteLine( "Error" );
            _viewModel.Message = ServiceErrorType.AppError.ToString();
            _viewModel.HasMessage = true;
        }
    }
}