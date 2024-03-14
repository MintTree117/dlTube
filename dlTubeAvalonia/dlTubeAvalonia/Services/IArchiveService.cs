using System.Collections.Generic;
using System.Threading.Tasks;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public interface IArchiveService
{
    Task<ApiReply<ArchiveSearch?>> SearchVideosAsync( Dictionary<string, object>? parameters );
}