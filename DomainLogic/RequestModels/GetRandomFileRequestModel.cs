using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.RequestModels
{
    public record GetRandomFileRequestModel(
        string ParentFolderPath = "",
        string Search = "",
        bool Recursive = false
    );
}
