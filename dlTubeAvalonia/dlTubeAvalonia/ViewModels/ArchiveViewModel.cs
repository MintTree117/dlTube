using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Models;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class ArchiveViewModel : ReactiveObject
{
    List<string> _streamTypes = Enum.GetNames<StreamFilterType>().ToList();
    List<string> _sortTypes = Enum.GetNames<StreamSortType>().ToList();

    string _selectedStreamType;
    string _selectedSortType;

    bool _IsMenuOpen;
    
    public ObservableCollection<ArchiveItem> SearchResults { get; set; } = [];

    public ArchiveViewModel()
    {
        _selectedStreamType = _streamTypes[ 0 ];
        _selectedSortType = _sortTypes[ 0 ];
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
    public bool IsMenuOpen
    {
        get => _IsMenuOpen;
        set => this.RaiseAndSetIfChanged( ref _IsMenuOpen, value );
    }
}