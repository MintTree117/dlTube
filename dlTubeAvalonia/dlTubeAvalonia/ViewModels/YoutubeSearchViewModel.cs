using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Services;
using ReactiveUI;
using YoutubeExplode.Search;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeSearchViewModel : ReactiveObject
{
    readonly YoutubeSearchService _searchService;
    readonly List<int> _resultsPerPageCount = [ 10, 20, 30, 50, 100, 200 ];
    
    List<string> _sortTypes = Enum.GetNames<YoutubeSortType>().ToList();
    List<string> _resultsPerPage = [ "Show 10", "Show 20", "Show 30", "Show 50", "Show 100", "Show 200" ];
    string _selectedSortType = string.Empty;
    string _selectedResultsPerPage = string.Empty;
    string _searchText = string.Empty;
    
    IReadOnlyList<VideoSearchResult> _searchResults = [ ];

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<string, Unit> CopyUrlCommand { get; }
    
    public YoutubeSearchViewModel()
    {
        _searchService = new YoutubeSearchService();
        SelectedSortType = _sortTypes[ 0 ];
        SelectedResultsPerPage = _resultsPerPage[ 0 ];
        SearchCommand = ReactiveCommand.CreateFromTask( Search );
        CopyUrlCommand = ReactiveCommand.CreateFromTask<string>( async ( url ) => { await CopyUrlToClipboard( url ); } );
    }
    
    public List<string> SortTypes
    {
        get => _sortTypes;
        set => this.RaiseAndSetIfChanged( ref _sortTypes, value );
    }
    public List<string> ResultsPerPage
    {
        get => _resultsPerPage;
        set => this.RaiseAndSetIfChanged( ref _resultsPerPage, value );
    }
    public string SelectedSortType
    {
        get => _selectedSortType;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedSortType, value );
            OnChangeSortDropdown();
        }
    }
    public string SelectedResultsPerPage
    {
        get => _selectedResultsPerPage;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedResultsPerPage, value );
            OnChangeResultsPerPageDropdown();
        }
    }
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged( ref _searchText, value );
    }
    public IReadOnlyList<VideoSearchResult> SearchResults
    {
        get => _searchResults;
        set => this.RaiseAndSetIfChanged( ref _searchResults, value );
    }

    async Task Search()
    {
        if ( !_resultsPerPage.Contains( _selectedResultsPerPage ) )
            throw new Exception( "Invalid _selectedResultsPerPage" );
        
        int index = _resultsPerPage.IndexOf( _selectedResultsPerPage );
        
        if ( index < 0 || index > _resultsPerPageCount.Count )
            throw new Exception( "Invalid _selectedResultsPerPage" );
        
        SearchResults = await _searchService.GetStreams( _searchText, _resultsPerPageCount[ index ] );
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
    
    void OnChangeSortDropdown()
    {
        if ( !Enum.TryParse( _selectedSortType, out YoutubeSortType type ) )
            throw new Exception( "Invalid _selectedSortType!" );

        SearchResults = type switch
        {
            YoutubeSortType.Default => SearchResults,
            YoutubeSortType.Alphabetical => _searchResults.OrderBy( r => r.Title ).ToList(),
            YoutubeSortType.Duration => _searchResults.OrderBy( r => r.Duration ).ToList(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    void OnChangeResultsPerPageDropdown()
    {
        if ( string.IsNullOrWhiteSpace( _searchText ) )
            return;
        
        SearchCommand.Execute();
    }
}