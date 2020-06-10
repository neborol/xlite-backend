using AutoMapper;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace EliteForce.Data
{
    public class UserRepository : IUserRepository
    {
        private EliteDataContext _context;
        private UserManager<User> _userManager;
        private IMapper _mapper;

        public UserRepository(EliteDataContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<IEnumerable<User>> GetUserssuper()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<IList<Claim>> GetClaims(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                new List<Claim>();
            }
            var existingClaims = await _userManager.GetClaimsAsync(user);
            return existingClaims;
        }

        public async Task<IdentityResult> UpdateClaims(string id, IList<Claim> claimList)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                new List<Claim>();
            }
            var existingClaims = await _userManager.GetClaimsAsync(user);

            //var updatedClaims = _mapper.Map<IList<Claim>>(claimList);

            var removeRes = await _userManager.RemoveClaimsAsync(user, existingClaims);

            if (!removeRes.Succeeded)
            {
                throw new Exception("Can not remove claims");
            } 

            var updateRes = await _userManager.AddClaimsAsync(user, claimList);

            return updateRes;
        }

        public Task<bool> SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}

















//namespace EliteForce.Data
//{
//    public class UserRepository : IUserRepository
//    {
//        private EliteDataContext _context;

//        public UserRepository(EliteDataContext context)
//        {
//            _context = context;
//        }
//        public void Add<T>(T entity) where T : class
//        {
//            _context.Add(entity);
//        }

//        public void Delete<T>(T entity) where T : class
//        {
//            _context.Remove(entity);
//        }
//        public async Task<IEnumerable<User>> GetUsers()
//        {
//            var users = await _context.Users.ToListAsync();
//            return users;
//        }
//        public async Task<User> GetSingleUser(string id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            var subscriptions = await _context.Subscriptions.Where(s => s.UserId == id).ToListAsync();
//            user.Subscriptions = subscriptions;
//            return user;
//        }

//        public async Task<IEnumerable<Subscription>> GetSubscriptionMonths(string userCode)
//        {
//            //var member = await _context.Members.FirstOrDefaultAsync(m => m.CodeNr == memberCode);
//            //var subscriMonths = _context.Subscriptions.Where(s => s.Member == member).ToList();
//            //return subscriMonths;
//            var user = await _context.Users.FirstOrDefaultAsync(m => m.CodeNr == userCode);
//            var subscriMonths = _context.Subscriptions.Where(m => m.UserId == user.Id).ToList();
//            return subscriMonths;
//        }

//        public async Task<Subscription> GetSubscriptionMonth(string name, string userCode)
//        {
//            var user = await _context.Users.Include(s => s.Subscriptions).FirstOrDefaultAsync(m => m.CodeNr == userCode);
//            var subsMonth = user.Subscriptions.FirstOrDefault(sb => sb.Name == name);
//            return subsMonth;
//        }

//        public Task<bool> SaveAll()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
