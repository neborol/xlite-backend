using AutoMapper;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        private readonly ILogger _logger;
        


        public UserRepository(
            EliteDataContext context, 
            UserManager<User> userManager, 
            IMapper mapper,
            ILogger<UserRepository> logger
            )
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public void Add<User>(User entity)
        {
            _context.Add(entity);
        }

        public async Task<int> DeleteUser(string email)
        {
            var user2Delete = _userManager.Users.FirstOrDefault(u => u.Email == email);
            if (user2Delete == null)
            {
                throw new Exception("No such user found");
            }
            _context.Users.Remove(user2Delete);
            var num = await _context.SaveChangesAsync();

            return num;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<IdentityResult> UpdateUserStatusActive(string email)
        {
            var user2Update = _userManager.Users.FirstOrDefault(u => u.Email == email);
            user2Update.Status = "ACTIVE";
            var identityRes = await _userManager.UpdateAsync(user2Update);
            return identityRes;
        }

        public async Task<IdentityResult> UpdateUserStatusInActive(string email)
        {
            var user2Update = _userManager.Users.FirstOrDefault(u => u.Email == email);
            user2Update.Status = "INACTIVE";
            var identityRes = await _userManager.UpdateAsync(user2Update);
            return identityRes;
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


        //public async Task<int> changePassword( )
        //{
        //    var faq = await _context.FaqItems.FirstOrDefaultAsync(f => f.FaqId == changedFaq.FaqId);
        //    if (faq != null)
        //    {
        //        faq.FaqAnswer = changedFaq.FaqAnswer;
        //        faq.FaqQuestion = changedFaq.FaqQuestion;

        //        _context.FaqItems.Update(faq);
        //        var numEdited = await _context.SaveChangesAsync();
        //        return numEdited;
        //    }

        //    return 0;

        //}

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
