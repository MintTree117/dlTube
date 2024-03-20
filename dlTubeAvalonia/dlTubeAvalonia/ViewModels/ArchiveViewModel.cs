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

public sealed class ArchiveViewModel : ReactiveObject
{
    // Services
    readonly ILogger<ArchiveViewModel>? _logger;
    readonly ArchiveService _archiveService;
    
    // Property Field List Values
    readonly List<StreamFilterType> _streamTypeDefinitions = Enum.GetValues<StreamFilterType>().ToList();
    readonly List<StreamSortType> _sortTypesDefinition = Enum.GetValues<StreamSortType>().ToList();
    readonly List<int> _resultCounts = [ 10, 20, 30, 50, 100, 200 ];
    
    // Property Fields
    List<string> _categoryNames = [ "a, b, c" ];
    List<ArchiveItem> _searchResults = [ ];
    List<string> _streamTypes;
    List<string> _sortTypes;
    List<string> _resultCountNames;
    string _selectedCategoryName = string.Empty;
    string _selectedStreamType = string.Empty;
    string _selectedSortType = string.Empty;
    string _selectedResultCountName = string.Empty;
    string _searchText = string.Empty;
    string _errorMessage = string.Empty;
    bool _ShowLoginPrompt = true;
    bool _IsUserAuthenticated;
    bool _isFree = true;
    bool _hasError;
    
    // Commands
    public ReactiveCommand<Unit, ApiReply<ArchiveSearch?>> SearchCommand { get; }
    
    // Constructor
    public ArchiveViewModel()
    {
        _logger = Program.ServiceProvider.GetService<ILogger<ArchiveViewModel>>();
        
        _streamTypes = GetStreamFilterTypeNames();
        _sortTypes = GetStreamSortTypeNames();
        _resultCountNames = GetResultsPerPageNames( _resultCounts );

        SelectedCategoryName = string.Empty;
        SelectedStreamType = _streamTypes[ 0 ];
        SelectedSortType = _sortTypes[ 0 ];
        SelectedResultCountName = _resultCountNames[ 0 ];
        _archiveService = Program.ServiceProvider.GetService<ArchiveService>() ?? throw new Exception( "Failed to get archive service!" );
        SearchCommand = ReactiveCommand.CreateFromTask( SearchArchive );

        try
        {
            LoadCategories();
        }
        catch ( Exception e )
        {
            _logger?.LogError( e, e.Message );
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
    
    // Command Definitions
    async void LoadCategories()
    {
        ApiReply<List<ArchiveCategory>?> reply = await _archiveService.GetCategoriesAsync();

        if ( !reply.Success || reply.Data is null )
        {
            HasError = true;
            _logger?.LogError( reply.PrintDetails() );
            ErrorMessage = reply.PrintDetails();
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
    async Task<ApiReply<ArchiveSearch?>> SearchArchive()
    {
        ApiReply<ArchiveSearch?> reply = await _archiveService.SearchVideosAsync( null );
        return reply;
    }

    // Reactive Properties
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
            DefaultDropdownBehaviour();
        }
    }
    public string SelectedStreamType
    {
        get => _selectedStreamType;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedStreamType, value );
            DefaultDropdownBehaviour();
        }
    }
    public string SelectedSortType
    {
        get => _selectedSortType;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedSortType, value );
            DefaultDropdownBehaviour();
        }
    }
    public string SelectedResultCountName
    {
        get => _selectedResultCountName;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedResultCountName, value );
            DefaultDropdownBehaviour();
        }
    }
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged( ref _searchText, value );
    }
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged( ref _errorMessage, value );
    }
    public bool ShowLoginPrompt
    {
        get => _ShowLoginPrompt;
        set => this.RaiseAndSetIfChanged( ref _ShowLoginPrompt, value );
    }
    public bool IsUserAuthenticated
    {
        get => _IsUserAuthenticated;
        set => this.RaiseAndSetIfChanged( ref _IsUserAuthenticated, value );
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
    void DefaultDropdownBehaviour()
    {
        if ( string.IsNullOrWhiteSpace( _searchText ) )
            return;

        SearchCommand.Execute();
    }
}