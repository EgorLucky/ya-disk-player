using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.RequestModels
{
    public record CreateUserInviteRequest(
        string Firstname, 
        string Lastname, 
        string Email);
}
