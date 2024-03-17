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
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeSearchViewModel : ReactiveObject
{
    readonly YoutubeSearchService _searchService;
    readonly List<YoutubeSortType> _sortTypesDefinition = Enum.GetValues<YoutubeSortType>().ToList();
    readonly List<int> _resultsPerPageDefinition = [ 10, 20, 30, 50, 100, 200 ];

    List<string> _sortTypes = [ ];
    List<string> _resultsPerPage = [ ];
    string _selectedSortType = string.Empty;
    string _selectedResultsPerPage = string.Empty;
    string _searchText = string.Empty;
    
    IReadOnlyList<VideoSearchResult> _searchResults = [ ];
    
    bool _isFree = true;

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<string, Unit> CopyUrlCommand { get; }
    
    public YoutubeSearchViewModel()
    {
        SortTypes = GetSortTypeNames();
        ResultsPerPage = GetResultsPerPageNames( _resultsPerPageDefinition );
        _searchService = Program.ServiceProvider.GetService<YoutubeSearchService>() ?? throw new Exception( "Failed to get Youtube Search Service!" );
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
    public bool IsFree
    {
        get => _isFree;
        set => this.RaiseAndSetIfChanged( ref _isFree, value );
    }

    async Task Search()
    {
        if ( !_resultsPerPage.Contains( _selectedResultsPerPage ) )
            throw new Exception( "Invalid _selectedResultsPerPage" );
        
        int index = _resultsPerPage.IndexOf( _selectedResultsPerPage );
        
        if ( index < 0 || index > _resultsPerPageDefinition.Count )
            throw new Exception( "Invalid _selectedResultsPerPage" );

        IsFree = false;

        try
        {
            SearchResults = await _searchService.GetStreams( _searchText, _resultsPerPageDefinition[ index ] );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
        }
        
        IsFree = true;
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

    static List<string> GetSortTypeNames()
    {
        List<string> types = Enum.GetNames<YoutubeSortType>().ToList();

        for ( int i = 0; i < types.Count; i++ )
            types[ i ] = $"Sort: {types[ i ]}";

        return types;
    }
    static List<string> GetResultsPerPageNames( IEnumerable<int> values )
    {
        List<string> names = [ ];
        names.AddRange( from value in values select $"Show: {value}" );
        return names;
    }
    
    void OnChangeSortDropdown()
    {
        IsFree = false;

        try
        {
            int index = _sortTypes.IndexOf( _selectedSortType );

            if ( index < 0 || index > _sortTypesDefinition.Count )
                throw new Exception( "Invalid _selectedSortType!" );

            SearchResults = _sortTypesDefinition[ index ] switch
            {
                YoutubeSortType.Default => SearchResults,
                YoutubeSortType.Alphabetical => _searchResults.OrderBy( r => r.Title ).ToList(),
                YoutubeSortType.Duration => _searchResults.OrderBy( r => r.Duration ).ToList(),
                _ => throw new Exception( "Invalid _sortTypesDefinition!" )
            };
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
        }

        IsFree = true;
    }
    void OnChangeResultsPerPageDropdown()
    {
        if ( string.IsNullOrWhiteSpace( _searchText ) )
            return;
        
        SearchCommand.Execute();
    }
}