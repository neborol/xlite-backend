using EliteForce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EliteDataContext _context;
        private Random _random = new Random();
        private readonly ILogger _logger;
        

        public AuthRepository(EliteDataContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        //public async Task<User> Login(string userEmail, string password)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
        //    if (user == null)
        //        return null;
        //    if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        //        return null;
        //    return user;
        //}

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Now loop through the computedHash in order to compare the characters
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    return false;
                }
                return true;
            }
        }

        //public async Task<User> Register(User user, string password)
        //{
        //    byte[] passwordHash, passwordSalt;
        //    CreatePasswordHash(password, out passwordHash, out passwordSalt);
            
        //    try
        //    {
        //        Code randCode = GetSCEFRandomNr();
        //        user.PasswordHash = passwordHash;
        //        user.PasswordSalt = passwordSalt;
        //        user.CodeNr = "SCEF-" + randCode.CodeNr;
        //        await _context.Users.AddAsync(user);
        //        await _context.SaveChangesAsync();
        //        if (randCode.CodeNr.Length == 4 || !string.IsNullOrWhiteSpace(randCode.CodeNr))
        //        {
        //            RemoveSCEFRandomNr(randCode);
        //        }
        //    }
        //    catch
        //    {
        //        throw (new Exception("Radomisation failed"));
        //    }
        //    return user;
        //}

        
        // Generate a random 4 digit number
        //public string GenerateSCEFRandomNo()
        //{
        //    return _random.Next(0, 9999).ToString("D4");
        //}

        // Generate a random 4 digit number
        public async Task<Code> GetSCEFRandomNr()
        {
            Code randCode = await _context.Codes.FirstOrDefaultAsync();
            // Check the database if this number exists, else validate and then add number to list in database.
            return randCode;
        }

        public void RemoveSCEFRandomNr(Code cd)
        {
            _context.Codes.Remove(cd);
            _context.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) 
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
                
        }

        public async Task<bool> UserExists(string userEmail)
        {
            if (await _context.Users.AnyAsync(x => x.Email == userEmail))
                return true; // Means a matching user has been found in the database.

            return false;
        }

    }
}













//namespace EliteForce.Data
//{
//    public class AuthRepository : IAuthRepository
//{
//    private readonly EliteDataContext _context;
//    private Random _random = new Random();
//    public AuthRepository(EliteDataContext context)
//    {
//        _context = context;
//    }
//    //public async Task<User> Login(string userEmail, string password)
//    //{
//    //    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
//    //    if (user == null)
//    //        return null;
//    //    if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
//    //        return null;
//    //    return user;
//    //}

//    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
//    {
//        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
//        {
//            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
//            // Now loop through the computedHash in order to compare the characters
//            for (int i = 0; i < computedHash.Length; i++)
//            {
//                if (computedHash[i] != passwordHash[i])
//                    return false;
//            }
//            return true;
//        }
//    }

//    //public async Task<User> Register(User user, string password)
//    //{
//    //    byte[] passwordHash, passwordSalt;
//    //    CreatePasswordHash(password, out passwordHash, out passwordSalt);

//    //    try
//    //    {
//    //        Code randCode = GetSCEFRandomNr();
//    //        user.PasswordHash = passwordHash;
//    //        user.PasswordSalt = passwordSalt;
//    //        user.CodeNr = "SCEF-" + randCode.CodeNr;
//    //        await _context.Users.AddAsync(user);
//    //        await _context.SaveChangesAsync();
//    //        if (randCode.CodeNr.Length == 4 || !string.IsNullOrWhiteSpace(randCode.CodeNr))
//    //        {
//    //            RemoveSCEFRandomNr(randCode);
//    //        }
//    //    }
//    //    catch
//    //    {
//    //        throw (new Exception("Radomisation failed"));
//    //    }
//    //    return user;
//    //}


//    // Generate a random 4 digit number
//    //public string GenerateSCEFRandomNo()
//    //{
//    //    return _random.Next(0, 9999).ToString("D4");
//    //}

//    // Generate a random 4 digit number
//    public async Task<Code> GetSCEFRandomNr()
//    {
//        Code randCode = await _context.Codes.FirstOrDefaultAsync();
//        // Check the database if this number exists, else validate and then add number to list in database.
//        return randCode;
//    }

//    public void RemoveSCEFRandomNr(Code cd)
//    {
//        _context.Codes.Remove(cd);
//        _context.SaveChanges();
//    }

//    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
//    {
//        using (var hmac = new System.Security.Cryptography.HMACSHA512())
//        {
//            passwordSalt = hmac.Key;
//            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
//        }

//    }

//    public async Task<bool> UserExists(string userEmail)
//    {
//        if (await _context.Users.AnyAsync(x => x.Email == userEmail))
//            return true; // Means a matching user has been found in the database.

//        return false;
//    }


//    public async Task<IEnumerable<User>> GetUsers()
//    {
//        var users = await _context.Users.ToListAsync();
//        return users;
//    }
//    public async Task<User> GetSingleUser(int id)
//    {
//        var user = await _context.Users.FindAsync(id);
//        var subscriptions = await _context.Subscriptions.Where(u => u.UserId == user.Id).ToListAsync();
//        user.Subscriptions = subscriptions;
//        return user;
//    }
//    public async Task<IEnumerable<Subscription>> GetSubscriptionMonths(string userCode)
//    {
//        //var member = await _context.Members.FirstOrDefaultAsync(m => m.CodeNr == memberCode);
//        //var subscriMonths = _context.Subscriptions.Where(s => s.Member == member).ToList();
//        //return subscriMonths;
//        var user = await _context.Users.FirstOrDefaultAsync(u => u.CodeNr == userCode);
//        var subscriMonths = _context.Subscriptions.Where(u => u.UserId == user.Id).ToList();
//        return subscriMonths;
//    }

//    public async Task<Subscription> GetSubscriptionMonth(string name, string userCode)
//    {
//        var user = await _context.Users.Include(s => s.Subscriptions).FirstOrDefaultAsync(u => u.CodeNr == userCode);
//        var subsMonth = user.Subscriptions.FirstOrDefault(sb => sb.Name == name);
//        return subsMonth;
//    }
//}
//}
