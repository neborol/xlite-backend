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
        void Add<User>(User entity);
        Task<int> DeleteUser(string email);
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<User>> GetUserssuper();
        Task<IdentityResult> UpdateClaims(string Id, IList<Claim> ClaimsCollection);
        Task<IList<Claim>> GetClaims(string Id);
        Task<IdentityResult> UpdateUserStatusActive(string email);
        Task<IdentityResult> UpdateUserStatusInActive(string email);
    }
}

