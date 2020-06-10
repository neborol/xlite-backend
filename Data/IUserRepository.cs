using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IUserRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<User>> GetUserssuper();
        Task<IdentityResult> UpdateClaims(string Id, IList<Claim> ClaimsCollection);
        Task<IList<Claim>> GetClaims(string Id);
    }
}



//namespace EliteForce.Data
//{
//    public interface IUserRepository
//    {
//        void Add<T>(T entity) where T : class;
//        void Delete<T>(T entity) where T : class;
//        Task<bool> SaveAll();
//        Task<IEnumerable<User>> GetUsers();
//        Task<User> GetSingleUser(string id);
//        Task<IEnumerable<Subscription>> GetSubscriptionMonths(string userCode);
//        Task<Subscription> GetSubscriptionMonth(string name, string userCode);
//    }                                         
//}