using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface IYandexDiskApi
    {
        Task RefreshToken(YandexToken token);
        public Task<ResourcesFileResponse> ResourcesFiles(ResourcesFilesRequest request, YandexToken token);
        Task<YandexToken> GetToken(string code);
        Task<ResourcesDownloadResult> ResourcesDownload(string path, YandexToken token);
    }
}
