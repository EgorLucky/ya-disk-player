using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Entities
{
    public record User(
        string YandexId = "",
        string Email = "",
        string Firstname = "Noname",
        string Lastname = "Nolastname",
        string Login = "",
        string Sex = "",
        Guid? InviteId = null,
        DateTimeOffset? CreateDateTime = null 
    ) 
    {
        public DateTimeOffset? ActivateDateTime { get; set; }
        public DateTimeOffset? DeactivateDateTime { get; set; }
        public bool IsAdmin { get; set; }
    }
}
