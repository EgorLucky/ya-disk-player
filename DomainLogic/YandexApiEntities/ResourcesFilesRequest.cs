using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.YandexApiEntities
{
    public record ResourcesFilesRequest(
        int Limit,
        int Offset,
        string MediaType,
        List<string> Fields
    );
}
