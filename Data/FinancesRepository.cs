using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EliteForce.Data
{
    public class FinancesRepository: IFinancesRepository
    {
        private readonly EliteDataContext _context;
        private UserManager<User> _userManager;
        private readonly ILogger _logger;
        

        public FinancesRepository(
            EliteDataContext context, 
            UserManager<User> userManager,
            ILogger<FinancesRepository> logger
         )
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        public async Task<List<Subscription>> GetSubscriptions(string userId)
        {
            var subsList = await _context.Subscriptions.Where(s => s.UserId == userId).ToListAsync();      
            
            if (subsList.Count == 0)
            {
                return new List<Subscription>();
            }

            return subsList;
        }

        public async Task<int> GetTotalForMembers()
        {
            var currentYear = DateTime.Now.ToString("yyyy");

            var total = await _context.Subscriptions.Where(s => s.Year == currentYear).SumAsync(s => s.Amount);
            return total;
        }

    }
}
