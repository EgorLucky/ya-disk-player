using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.RequestModels
{
    public record GetIgnorePathsRequestModel(
        int Take, 
        int Page, 
        string Search,
        List<string> Paths, 
        string YandexUserId
    );
}
