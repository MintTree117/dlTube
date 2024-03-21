using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;

namespace dlTubeAvalonia.ViewModels;

public sealed class ArchiveViewModel : BaseViewModel
{
    // Services
    readonly ArchiveService _archiveService = null!;
    
    // Property Field List Values
    readonly List<StreamFilterType> _streamTypeDefinitions = Enum.GetValues<StreamFilterType>().ToList();
    readonly List<StreamSortType> _sortTypesDefinition = Enum.GetValues<StreamSortType>().ToList();
    readonly List<int> _resultCounts = [ 10, 20, 30, 50, 100, 200 ];
    
    // Property Fields
    int _searchCount;
    List<ArchiveItem> _searchResults = [ ];
    List<string> _categoryNames = [ "a, b, c" ];
    List<string> _streamTypes = [ ];
    List<string> _sortTypes = [ ];
    List<string> _resultCountNames = [ ];
    string _selectedCategoryName = string.Empty;
    string _selectedStreamType = string.Empty;
    string _selectedSortType = string.Empty;
    string _selectedResultCountName = string.Empty;
    string _searchText = string.Empty;
    bool _isFree = true;
    bool _hasError;

    string _apiKey = string.Empty;
    string _downloadLocation = string.Empty;
    
    // Command Definitions
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<int, Unit> DownloadCommand { get; }
    
    // Constructor
    public ArchiveViewModel() : base( TryGetLogger<ArchiveViewModel>() )
    {
        SearchCommand = ReactiveCommand.CreateFromTask( SearchArchive );
        DownloadCommand = ReactiveCommand.CreateFromTask<int>( async ( id ) => await DownloadArchiveItem( id ) );

        if ( this.SettingsService is null )
            return;
        
        if ( !TryGetArchiveService( ref _archiveService! ) )
            return;
        
        _streamTypes = GetStreamFilterTypeNames();
        _sortTypes = GetStreamSortTypeNames();
        _resultCountNames = GetResultsPerPageNames( _resultCounts );

        SelectedCategoryName = string.Empty;
        SelectedStreamType = _streamTypes[ 0 ];
        SelectedSortType = _sortTypes[ 0 ];
        SelectedResultCountName = _resultCountNames[ 0 ];

        try
        {
            LoadCategories();
        }
        catch ( Exception e )
        {
            Logger?.LogError( e, e.Message );
        }
    }
    
    // Init Methods
    bool TryGetArchiveService( ref ArchiveService? archiveService )
    {
        ArchiveService? service = Program.ServiceProvider.GetService<ArchiveService>();

        if ( service is not null )
        {
            archiveService = service;
            return true;
        }

        IsFree = false;
        HasError = true;
        Message = "Failed to load archive service!";
        return false;
    }
    async void LoadCategories()
    {
        ServiceReply<List<ArchiveCategory>?> reply = await _archiveService.GetCategoriesAsync( _apiKey );

        if ( !reply.Success || reply.Data is null )
        {
            HasError = true;
            Logger?.LogError( reply.PrintDetails() );
            Message = reply.PrintDetails();
            return;
        }

        CategoryNames = GetCategoryNames( reply.Data );
        return;

        static List<string> GetCategoryNames( IEnumerable<ArchiveCategory> categories )
        {
            List<string> names = [ ];
            names.AddRange( from c in categories select c.Name );
            return names;
        }
    }
    static List<string> GetStreamFilterTypeNames()
    {
        List<string> names = [ ];

        foreach ( string s in Enum.GetNames<StreamFilterType>().ToList() )
            names.Add( $"Stream Type: {s}" );

        return names;
    }
    static List<string> GetStreamSortTypeNames()
    {
        List<string> names = [ ];

        foreach ( string s in Enum.GetNames<StreamSortType>().ToList() )
            names.Add( $"Sort By: {s}" );

        return names;
    }
    static List<string> GetResultsPerPageNames( IEnumerable<int> values )
    {
        List<string> names = [ ];
        names.AddRange( from value in values select $"Show: {value}" );
        return names;
    }
    
    // Command Delegates
    public void CloseError()
    {
        HasError = false;
        Message = string.Empty;
    }
    async Task SearchArchive()
    {
        Dictionary<string, object> searchParams = GetSearchParams();
        ServiceReply<ArchiveSearch?> reply = await _archiveService.SearchVideosAsync( _apiKey, searchParams );

        if ( !reply.Success || reply.Data is null )
        {
            HasError = true;
            Message = reply.PrintDetails();
            return;
        }

        SearchCount = reply.Data.TotalMatches;
        SearchResults = reply.Data.Items;
    }
    async Task DownloadArchiveItem( int itemId )
    {
        await Task.Delay( 5000 );
    }

    // Reactive Properties
    public int SearchCount
    {
        get => _searchCount;
        set => this.RaiseAndSetIfChanged( ref _searchCount, value );
    }
    public List<ArchiveItem> SearchResults
    {
        get => _searchResults;
        set => this.RaiseAndSetIfChanged( ref _searchResults, value );
    }
    public List<string> CategoryNames
    {
        get => _categoryNames;
        set => this.RaiseAndSetIfChanged( ref _categoryNames, value );
    }
    public List<string> StreamTypes
    {
        get => _streamTypes;
        set => this.RaiseAndSetIfChanged( ref _streamTypes, value );
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
    public string SelectedCategoryName
    {
        get => _selectedCategoryName;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedCategoryName, value );
            OnChangeDropdownValue();
        }
    }
    public string SelectedStreamType
    {
        get => _selectedStreamType;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedStreamType, value );
            OnChangeDropdownValue();
        }
    }
    public string SelectedSortType
    {
        get => _selectedSortType;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedSortType, value );
            OnChangeDropdownValue();
        }
    }
    public string SelectedResultCountName
    {
        get => _selectedResultCountName;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedResultCountName, value );
            OnChangeDropdownValue();
        }
    }
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged( ref _searchText, value );
    }
    public bool IsFree
    {
        get => _isFree;
        set => this.RaiseAndSetIfChanged( ref _isFree, value );
    }
    public bool HasError
    {
        get => _hasError;
        set => this.RaiseAndSetIfChanged( ref _hasError, value );
    }
    
    // Private Methods
    protected override void OnAppSettingsChanged( AppSettingsModel newSettings )
    {
        _apiKey = newSettings.ApiKey;
        _downloadLocation = newSettings.DownloadLocation;
    }
    Dictionary<string, object> GetSearchParams()
    {
        Dictionary<string, object> searchParams = [ ];

        if ( !string.IsNullOrWhiteSpace( _selectedCategoryName ) )
            searchParams.Add( "category", _selectedCategoryName );

        if ( !string.IsNullOrWhiteSpace( _selectedStreamType ) )
            searchParams.Add( "streamType", _streamTypeDefinitions[ _streamTypes.IndexOf( _selectedStreamType ) ] );

        if ( !string.IsNullOrWhiteSpace( _selectedSortType ) )
            searchParams.Add( "sortType", _sortTypesDefinition[ _sortTypes.IndexOf( _selectedSortType ) ] );

        if ( !string.IsNullOrWhiteSpace( _selectedResultCountName ) )
            searchParams.Add( "resultCount", _resultCounts[ _resultCountNames.IndexOf( _selectedResultCountName ) ] );

        return searchParams;
    }
    void OnChangeDropdownValue()
    {
        if ( string.IsNullOrWhiteSpace( _searchText ) )
            return;

        SearchCommand.Execute();
    }
}