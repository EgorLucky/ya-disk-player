using DomainLogic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1.AuthorizationPolicies.Admin
{
    public class AdminRightsHandler : AuthorizationHandler<AdminRightsRequirement>
    {
        private readonly UserService _userService;

        public AdminRightsHandler(UserService userService)
        {
            _userService = userService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AdminRightsRequirement requirement)
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

            if (user != null && user.IsAdmin == requirement.AdminRightsRequired)
            {
                context.Succeed(requirement);
            }
        }
    }
}
