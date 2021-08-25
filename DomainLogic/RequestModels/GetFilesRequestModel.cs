using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.RequestModels
{
    public record GetFilesRequestModel(
      string ParentFolderPath = "", 
      string Search = "",
      int Page = 1,
      int Take = 20
    );
}
