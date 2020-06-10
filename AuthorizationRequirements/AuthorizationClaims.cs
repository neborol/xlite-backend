using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.AuthorizationRequirements
{
    public class CustomRequireClaim : IAuthorizationRequirement
    {
        public string _claimType { get; set; }
        public CustomRequireClaim(string claimType)
        {
            _claimType = claimType;
        }
    }


    public class CustomRequireClaimHandler : AuthorizationHandler<CustomRequireClaim>  // A handler to process request to authorize and must implement an IAuthorizationRequirement
    {
        public CustomRequireClaimHandler()
        {
            // Services for custom claims can be injected inside here
        }
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CustomRequireClaim requirement
            )
        {
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement._claimType);
            if (hasClaim)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
