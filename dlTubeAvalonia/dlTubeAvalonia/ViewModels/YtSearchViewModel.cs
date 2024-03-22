using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;

namespace dlTubeAvalonia.ViewModels;

public sealed class YtSearchViewModel : BaseViewModel
{
    // Services Definitions
    readonly YtSearchService? _youtubeSearchService;
    
    // Property Field List Values
    readonly List<YoutubeSortType> _sortTypesDefinition = Enum.GetValues<YoutubeSortType>().ToList();
    readonly List<int> _resultCounts = [ 10, 20, 30, 50, 100, 200 ];
    
    // Property Fields
    IReadOnlyList<YoutubeSearchResult> _searchResults = [ ];
    List<string> _sortTypes = [ ];
    List<string> _resultCountNames = [ ];
    string _selectedSortType = string.Empty;
    string _selectedResultCountName = string.Empty;
    string _searchText = string.Empty;
    
    // Commands
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<string, Unit> CopyUrlCommand { get; }
    
    // Constructor
    public YtSearchViewModel() : base( TryGetLogger<YtSearchViewModel>() )
    {
        TryGetYoutubeService( ref _youtubeSearchService! );

        SortTypes = GetSortTypeNames();
        ResultCountNames = GetResultsPerPageNames( _resultCounts );
        SelectedSortType = _sortTypes[ 0 ];
        SelectedResultCountName = _resultCountNames[ 0 ];
        SearchCommand = ReactiveCommand.CreateFromTask( Search );
        CopyUrlCommand = ReactiveCommand.CreateFromTask<string>( async ( url ) => { await CopyUrlToClipboard( url ); } );

        IsFree = true;
    }
    void TryGetYoutubeService( ref YtSearchService service )
    {
        try
        {
            var searchService = Program.ServiceProvider.GetService<YtSearchService>();

            if ( searchService is null )
            {
                HasMessage = true;
                Message = $"Failed to get service: {nameof( YtSearchService )}";
                return;
            }

            service = searchService;
        }
        catch ( Exception e )
        {
            Logger?.LogError( e, e.Message );
            Message = $"Failed to get service: {nameof( YtSearchService )}";
            HasMessage = true;
        }
    }
    
    // Reactive Properties
    public IReadOnlyList<YoutubeSearchResult> SearchResults
    {
        get => _searchResults;
        set => this.RaiseAndSetIfChanged( ref _searchResults, value );
    }
    public List<string> SortTypes
    {
        get => _sortTypes;
        set => this.RaiseAndSetIfChanged( ref _sortTypes, value );
    }
    public List<string> ResultCountNames
    {
        get => _resultCountNames;
        set => this.RaiseAndSetIfChanged( ref _resultCountNames, value );
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
    public string SelectedResultCountName
    {
        get => _selectedResultCountName;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedResultCountName, value );
            OnChangeResultsDropdown();
        }
    }
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged( ref _searchText, value );
    }
    
    // Command Delegates
    async Task Search()
    {
        if ( !ValidateSearchParams( out int resultCountIndex ) )
            return;   
        
        IsFree = false;

        try
        {
            ServiceReply<IReadOnlyList<YoutubeSearchResult>> reply = await _youtubeSearchService!.GetStreams( _searchText, _resultCounts[ resultCountIndex ] );

            if ( reply is { Success: true, Data: not null } )
            {
                SearchResults = reply.Data;
            }
            else
            {
                SearchResults = new List<YoutubeSearchResult>();
                Message = ServiceErrorType.NotFound.ToString();
                HasMessage = true;
            }
        }
        catch ( Exception e )
        {
            Logger?.LogError( e, e.Message );
            HasMessage = true;
            Console.WriteLine(e + e.Message);
            Message = e.Message; //ServiceErrorType.ServerError.ToString();
        }
        
        IsFree = true;
    }
    async Task CopyUrlToClipboard( string? url )
    {
        if ( string.IsNullOrWhiteSpace( url ) )
        {
            HasMessage = true;
            Message = "Tried to copy invalid url!";
            return;
        }
        
        // TODO: Make this mobile accessible
        Window? mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop 
            ? desktop.MainWindow : null;

        if ( mainWindow?.Clipboard is null )
        {
            Logger?.LogError( "Failed to obtain clipboard from main window!" );
            HasMessage = true;
            Message = "Failed to perform copy operation!";
            return;   
        }

        await mainWindow.Clipboard.SetTextAsync( url );
    }
    
    // Private Methods
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
    bool ValidateSearchParams( out int resultCountIndex )
    {
        resultCountIndex = -1;

        if ( string.IsNullOrWhiteSpace( _searchText ) )
        {
            Logger?.LogError( "Search text is null!" );
            HasMessage = true;
            Message = "Search text is null!";
            return false;
        }
        
        if ( _youtubeSearchService is null )
        {
            Logger?.LogError( "Search service is null!" );
            HasMessage = true;
            Message = ServiceErrorType.AppError.ToString();
            return false;
        }

        if ( !_resultCountNames.Contains( _selectedResultCountName ) )
        {
            Logger?.LogError( "_resultCountNames out of bounds!" );
            HasMessage = true;
            Message = "Invalid _selectedResultsPerPage";
            return false;
        }

        resultCountIndex = _resultCountNames.IndexOf( _selectedResultCountName );

        if ( resultCountIndex < 0 || resultCountIndex > _resultCounts.Count )
        {
            Logger?.LogError( "resultCountIndex out of bounds!" );
            HasMessage = true;
            Message = "Invalid _selectedResultsPerPage";
            return false;
        }

        return true;
    }
    void OnChangeSortDropdown()
    {
        IsFree = false;

        try
        {
            int index = _sortTypes.IndexOf( _selectedSortType );

            if ( index < 0 || index > _sortTypesDefinition.Count )
            {
                HasMessage = true;
                Message = "Invalid _selectedSortType!";
                IsFree = true;
                return;
            }

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
            Logger?.LogError( e, e.Message );
            HasMessage = true;
            Message = ServiceErrorType.AppError.ToString();
        }

        IsFree = true;
    }
    void OnChangeResultsDropdown()
    {
        if ( string.IsNullOrWhiteSpace( _searchText ) )
            return;   
        
        SearchCommand.Execute();
    }
}