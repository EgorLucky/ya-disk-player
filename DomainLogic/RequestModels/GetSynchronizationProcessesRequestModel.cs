using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.RequestModels
{
    public record GetSynchronizationProcessesRequestModel(
      int Page = 1,
      int Take = 20,
      bool Finished = true
    );
}
