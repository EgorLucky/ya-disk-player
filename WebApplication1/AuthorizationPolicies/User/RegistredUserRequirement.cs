using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.AuthorizationPolicies.User
{
    public class RegistredUserRequirement : IAuthorizationRequirement
    {
        public bool RegistredUserRequired { get; }

        public RegistredUserRequirement(bool registredUserRequired)
        {
            RegistredUserRequired = registredUserRequired;
        }
    }
}
