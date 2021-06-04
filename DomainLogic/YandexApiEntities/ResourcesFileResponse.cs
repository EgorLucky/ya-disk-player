using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.YandexApiEntities
{
    public record ResourcesFileResponse(
        List<ResourcesFileItem> Items,
        int Offset
    );

    public record ResourcesFileItem(
        string Name,
        string Path,
        string File,
        string ResourceId
    );
}
