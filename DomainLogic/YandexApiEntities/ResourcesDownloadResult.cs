using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.YandexApiEntities
{
    public record ResourcesDownloadResult(
        string Href = "", 
        string Method = "", 
        bool Templated = false
    );
}
