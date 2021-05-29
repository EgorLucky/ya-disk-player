using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.AuthorizationPolicies.Admin
{
    public class AdminRightsRequirement : IAuthorizationRequirement
    {
        public bool AdminRightsRequired { get; }

        public AdminRightsRequirement(bool adminRightsRequired)
        {
            AdminRightsRequired = adminRightsRequired;
        }
    }
}
