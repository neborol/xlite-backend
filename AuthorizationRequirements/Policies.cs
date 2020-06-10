using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

//namespace EliteForce.AuthorizationRequirements
//{
//    public class Policies
//    {
//        public const string Pilot = "false";
//        public const string News = "false";
//        public const string Manager = "false";
//        public const string SuperAdmin = "false";

//        public static AuthorizationPolicy PilotPolicy() {
//            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Pilot).Build();
//        }

//        public static AuthorizationPolicy NewsPolicy()
//        {
//            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(News).Build();
//        }

//        public static AuthorizationPolicy ManagerPolicy()
//        {
//            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Manager).Build();
//        }

//        public static AuthorizationPolicy SuperAdminPolicy()
//        {
//            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(SuperAdmin).Build();
//        }
//    }
//}




namespace EliteForce.AuthorizationRequirements
{
    public class Policies
    {
        public const string Pilot = "Pilot";
        public const string News = "News";
        public const string Manager = "Manager";
        public const string SuperAdmin = "SuperAdmin";

        public static AuthorizationPolicy PilotPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireClaim("Pilot", "true").Build();
        }

        public static AuthorizationPolicy NewsPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireClaim("News", "true").Build();
        }

        public static AuthorizationPolicy ManagerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireClaim("Manager", "true").Build();
        }

        public static AuthorizationPolicy SuperAdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireClaim("SuperAdmin", "true").Build();
        }
    }
}