using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.ResponseModels
{
    public record CreateUserInviteResult(
        bool Success = false,
        string ErrorMessage = "",
        Guid? InviteId = null
    );
}
