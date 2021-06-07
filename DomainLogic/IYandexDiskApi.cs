﻿using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface IYandexDiskApi
    {
        public Task<ResourcesFileResponse> ResourcesFiles(ResourcesFilesRequest request, string accessToken);
    }
}