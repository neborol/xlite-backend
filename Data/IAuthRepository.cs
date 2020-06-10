using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IAuthRepository
    {
        //Task<User> Register(User user, string password);
        //Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<Code> GetSCEFRandomNr();
        void RemoveSCEFRandomNr(Code cd);
    }
}
