using DomainLogic.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface IIgnorePathRepository
    {
        Task<List<string>> GetAll(string yandexUserId);
        Task Add(List<string> paths, string yandexUserId);
        Task<List<IgnorePath>> Get(int take, int page, List<string> search, string yandexUserId);
        Task Delete(List<string> ignorePaths, string yandexUserId);
    }
}