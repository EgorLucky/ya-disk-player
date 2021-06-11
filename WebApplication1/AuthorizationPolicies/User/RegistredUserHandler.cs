using DomainLogic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1.AuthorizationPolicies.User
{
    public class RegistredUserHandler : AuthorizationHandler<RegistredUserRequirement>
    {
        private readonly UserService _userService;

        public RegistredUserHandler(UserService userService)
        {
            _userService = userService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       RegistredUserRequirement requirement)
        {
            var email = context
                            .User
                            .Claims
                            .Where(c => c.Type == "default_email")
                            .Select(c => c.Value)
                            .FirstOrDefault();

            if(string.IsNullOrEmpty(email))
            {
                return;
            }

            var user = await _userService.GetUserByEmail(email);

            if (user != null && 
                user.ActivateDateTime.HasValue &&
                user.DeactivateDateTime.HasValue == false &&
                requirement.RegistredUserRequired)
            {
                context.Succeed(requirement);
            }
        }
    }
}
