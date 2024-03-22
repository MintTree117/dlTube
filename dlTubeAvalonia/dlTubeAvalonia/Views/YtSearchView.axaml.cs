using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public partial class YtSearchView : UserControl
{
    readonly ILogger<YtSearchView>? _logger = Program.ServiceProvider.GetService<ILogger<YtSearchView>>();
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
        Process p = new();
        
        try
        {
            p.StartInfo.FileName = url;
            p.StartInfo.UseShellExecute = true; // Important for .NET Core

            p.Start();
            p.WaitForExit();
        }
        catch ( Exception e )
        {
            p.Kill();
            _logger?.LogError( e, e.Message );
            _viewModel.ShowMessage( $"{ServiceErrorType.AppError} : Failed to open link!" );
        }
    }
}