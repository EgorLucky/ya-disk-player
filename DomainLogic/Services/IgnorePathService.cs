using DomainLogic.Entities;
using DomainLogic.Repositories;
using DomainLogic.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class IgnorePathService
    {
        private readonly IIgnorePathRepository _repository;

        public IgnorePathService(IIgnorePathRepository repository)
        {
            _repository = repository;
        }

        public async Task<IgnorePathAddResult> Add(List<string> paths, string yandexUserId)
        {
            if (paths == null || paths.Count == 0)
                return new IgnorePathAddResult(
                    false,
                    "Empty data"
                );

            var alreadyExistingPaths = await _repository.Get(1, 1, paths, yandexUserId);

            if (alreadyExistingPaths.Count > 0)
            {
                var stringList = string.Join(", ", alreadyExistingPaths.Select(p => p.Path));
                return new IgnorePathAddResult(
                    false,
                    $"You trying to add already existing paths: {stringList}"
                );
            }

            await _repository.Add(paths, yandexUserId);

            return new IgnorePathAddResult(true);

        }
        
        public async Task<List<IgnorePath>> Get(int take, int page, string search, string yandexUserId)
        {
            if (take < 1)
                take = 10;

            if (page < 1)
                page = 1;

            var searchCollection = new List<string>();
            if (!string.IsNullOrEmpty(search))
                searchCollection.Add(search);

            var result = await _repository.Get(take, page, searchCollection, yandexUserId);

            return result;
        }

        public async Task<IgnorePathDeleteResult> Delete(List<string> paths, string yandexUserId)
        {
            if (paths == null || paths.Count == 0)
                return new IgnorePathDeleteResult(
                    false,
                    "Empty data"
                );

            var alreadyExistingPaths = await _repository.Get(paths.Count, 1, paths, yandexUserId);

            if(alreadyExistingPaths.Count != paths.Count)
            {
                var stringList = string.Join(", ", 
                    paths
                    .Where(p => alreadyExistingPaths.Any(a => a.Path.ToLower() == p.ToLower()) == false));
                return new IgnorePathDeleteResult(
                    false,
                    $"You trying to delete not existing paths: {stringList}"
                );
            }

            await _repository.Delete(paths, yandexUserId);

            return new IgnorePathDeleteResult(true);
        }
    }
}
