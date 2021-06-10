using AutoMapper;
using DomainLogic.Entities;
using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ResourcesFileItem, File>()
                .ForMember(f => f.ParentFolder, options => options.MapFrom(rf => MapFileParentFolder(rf)))
                .ForMember(f => f.ParentFolderPath, options => options.MapFrom(rf => MapFileParentFolderPath(rf)))
                .ForMember(f => f.Url, options => options.MapFrom(rf => rf.File));

            CreateMap<List<string>, File>()                
                .ForMember(f => f.Name, 
                            options => options.MapFrom(pathFolders => MapName(pathFolders)))
                .ForMember(f => f.Path, 
                            options => options.MapFrom(pathFolders => MapPath(pathFolders)))
                .ForMember(f => f.ParentFolderPath, 
                            options => options.MapFrom(pathFolders => MapParentFolderPath(pathFolders)))
                .ForMember(f => f.ParentFolder,
                            options => options.MapFrom(pathFolders => MapParentFolder(pathFolders)))
                .ForMember(f => f.Type,
                            options => options.MapFrom(pathFolders => "folder"));
        }

        string MapName(List<string> pathFolders)
        {
            var name = pathFolders.LastOrDefault();
            return name;
        }

        string MapPath(List<string> pathFolders)
        {
            var path = string.Join("/", pathFolders.Take(pathFolders.Count));
            return path;
        }

        string MapParentFolderPath(List<string> pathFolders)
        {
            var parentFolderPath = string.Join("/", pathFolders.Take(pathFolders.Count - 1));
            return parentFolderPath;
        }

        string MapParentFolder(List<string> pathFolders)
        {
            var parentFolder = pathFolders.Take(pathFolders.Count - 1).LastOrDefault();
            return parentFolder;
        }

        string MapFileParentFolder(ResourcesFileItem resourcesFileItem)
        {
            var pathFolders = GetPathFolders(resourcesFileItem.Path);
            return pathFolders.Take(pathFolders.Count - 1).LastOrDefault();
        }

        string MapFileParentFolderPath(ResourcesFileItem resourcesFileItem)
        {
            var pathFolders = GetPathFolders(resourcesFileItem.Path);
            return string.Join("/", pathFolders.Take(pathFolders.Count - 1));
        }

        List<string> GetPathFolders(string path)
        {
            var pathFolders = path
                                .Split("/", StringSplitOptions.RemoveEmptyEntries)
                                .ToList();
            return pathFolders;
        }
    }
}
