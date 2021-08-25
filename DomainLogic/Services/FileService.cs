using AutoMapper;
using DomainLogic.Entities;
using DomainLogic.Repositories;
using DomainLogic.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class FileService
    {
        private readonly IFileRepository _repository;
        private readonly IMapper _mapper;

        public FileService(IFileRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<File>> GetFilesByParentFolder(GetFilesRequestModel request, string yandexUserId)
        {
            if (request.Page < 1)
                request = request with { Page = 1 };
            if (request.Take < 1)
                request = request with { Take = 20 };
            if (string.IsNullOrEmpty(request.ParentFolderPath))
                request = request with { ParentFolderPath = "disk:" };

            return await _repository.GetFilesByParentFolderPath(request, yandexUserId);
        }

        public async Task<File> GetRandomFile(GetRandomFileRequestModel request, string yandexUserId)
        {
            if (string.IsNullOrEmpty(request.ParentFolderPath))
                request = request with { ParentFolderPath = "disk:" };
            return await _repository.GetRandomFile(request, yandexUserId);
        }
    }
}
