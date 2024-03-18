using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class ArchiveViewModel : ReactiveObject
{
    List<string> _streamTypes = Enum.GetNames<StreamFilterType>().ToList();
    List<string> _sortTypes = Enum.GetNames<StreamSortType>().ToList();

    string _selectedStreamType;
    string _selectedSortType;

    bool _ShowLoginPrompt = true;
    bool _IsUserAuthenticated;
    bool _IsMenuOpen;

    List<ArchiveItem> _searchResults = [ ];

    readonly ArchiveService _archiveService;
    
    public ReactiveCommand<Unit, ApiReply<ArchiveSearch?>> SearchCommand { get; }

    public ArchiveViewModel()
    {
        _selectedStreamType = _streamTypes[ 0 ];
        _selectedSortType = _sortTypes[ 0 ];
        _archiveService = Program.ServiceProvider.GetService<ArchiveService>() ?? throw new Exception( "Failed to get archive service!" );
        SearchCommand = ReactiveCommand.CreateFromTask( SearchArchive );
    }

    public async Task<ApiReply<ArchiveSearch?>> SearchArchive()
    {
        ApiReply<ArchiveSearch?> reply = await _archiveService.SearchVideosAsync( null );

        return reply;
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
    public string SelectedStreamType
    {
        get => _selectedStreamType;
        set => this.RaiseAndSetIfChanged( ref _selectedStreamType, value );
    }
    public string SelectedSortType
    {
        get => _selectedSortType;
        set => this.RaiseAndSetIfChanged( ref _selectedSortType, value );
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
    public bool IsMenuOpen
    {
        get => _IsMenuOpen;
        set => this.RaiseAndSetIfChanged( ref _IsMenuOpen, value );
    }
    public List<ArchiveItem> SearchResults
    {
        get => _searchResults;
        set => this.RaiseAndSetIfChanged( ref _searchResults, value );
    }
}