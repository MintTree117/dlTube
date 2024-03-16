using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using dlTubeAvalonia.Services;
using ReactiveUI;
using YoutubeExplode.Search;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeSearchViewModel : ReactiveObject
{
    List<string> _resultsPerPage = [ "Show 10", "Show 20", "Show 30", "Show 50", "Show 100", "Show 200" ];
    string _selectedResultsPerPage = "Show 10";
    
    readonly YoutubeSearchService _searchService;
    string _searchText = string.Empty;
    IReadOnlyList<ISearchResult> _searchResults = [ ];

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<string, Unit> CopyUrlCommand { get; }
    
    public YoutubeSearchViewModel()
    {
        _searchService = new YoutubeSearchService();
        SelectedResultsPerPage = _resultsPerPage[ 0 ];
        SearchCommand = ReactiveCommand.CreateFromTask( Search );
        CopyUrlCommand = ReactiveCommand.CreateFromTask<string>( async ( url ) => { await CopyUrlToClipboard( url ); } );
    }

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged( ref _searchText, value );
    }
    public IReadOnlyList<ISearchResult> SearchResults
    {
        get => _searchResults; 
        set => this.RaiseAndSetIfChanged( ref _searchResults, value );
    }
    public List<string> ResultsPerPage
    {
        get => _resultsPerPage;
        set => this.RaiseAndSetIfChanged( ref _resultsPerPage, value );
    }
    public string SelectedResultsPerPage
    {
        get => _selectedResultsPerPage;
        set => this.RaiseAndSetIfChanged( ref _selectedResultsPerPage, value );
    }

    async Task Search()
    {
        if ( !_resultsPerPage.Contains( _selectedResultsPerPage ) )
            return;

        if ( !int.TryParse( _selectedResultsPerPage, out int resultPerPage ) )
            return;
        
        Console.WriteLine("About to search");
        
        SearchResults = await _searchService.GetStreams( _searchText, resultPerPage );
    }
    async Task CopyUrlToClipboard( string url )
    {
        // TODO: Make this mobile accessible
        Window? mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

        if ( mainWindow?.Clipboard is null )
        {
            Console.WriteLine( "Main Window Not Found" );
            return;   
        }

        await mainWindow.Clipboard.SetTextAsync( url );
    }
}