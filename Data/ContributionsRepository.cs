
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public class ContributionsRepository: IContributionsRepository
    {
        private EliteDataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        

        public ContributionsRepository(
            EliteDataContext context, 
            UserManager<User> userManager,
            ILogger<ContributionsRepository> logger
         )
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        public async Task<int> AddContribution(ContributionsPostDto contributionsObj)
        {
            var recordExists = await _context.Subscriptions
                .AnyAsync(s => s.Name == contributionsObj.Month && s.Year == contributionsObj.Year && s.UserId == contributionsObj.UserId);

            if (recordExists == true)
            {
                throw new Exception("The contribution exists already.");
            }

            // Get the current year so as to filter only for current year
            var currentYear = DateTime.Now.ToString("yyyy");

            var memberTotal = await _context.Subscriptions
                .Where(s => s.Year == contributionsObj.Year && s.UserId == contributionsObj.UserId)
                .SumAsync(s => s.Amount);

            var newSubscription = new Subscription();
            newSubscription.Name = contributionsObj.Month;
            newSubscription.Amount = contributionsObj.Amount;
            newSubscription.DateEntered = contributionsObj.DateEntered;
            newSubscription.UserId = contributionsObj.UserId;
            newSubscription.Year = contributionsObj.Year;
            newSubscription.Total = memberTotal + contributionsObj.Amount;

            bool includeThisMonth = true;

            if ((int)System.DateTime.Now.DayOfWeek < 15)
            {
                includeThisMonth = false;
            }

            newSubscription.UptoDate = await checkContributionStatus(contributionsObj.UserId, newSubscription.Total, includeThisMonth);

            if (!String.IsNullOrWhiteSpace(contributionsObj.Comment))
            {
                newSubscription.Comment = contributionsObj.Comment;
            }

            await _context.Subscriptions.AddAsync(newSubscription);

            var res = await _context.SaveChangesAsync();

            if (!res.Equals(1))
            {
                throw new Exception("No record was added for a new contribution");
            }
            return res;
        }

        public async Task<bool> checkContributionStatus(string userId, int total, bool includeCurrentMonth)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var currentYear = DateTime.Now.ToString("yyyy");

            if (user.DateJoined.ToString("yyyy") == currentYear)
            {
                var joinedMonth = user.DateJoined.ToString("MM");
                var currentMonth = DateTime.Now.ToString("MM");
                var divisor = Convert.ToInt32(currentMonth) - Convert.ToInt32(joinedMonth);

                if (includeCurrentMonth == false)
                {
                    divisor -= 1;
                }

                if ((total / divisor) < 50)
                {
                    return false;
                } else
                {
                    return true;
                }
                // var value = DateTime.Compare(DateTime.Now, user.DateJoined);
            } else
            {
                var currentMonth = DateTime.Now.ToString("MM");
                var divisor = Convert.ToInt32(currentMonth);

                if (includeCurrentMonth == false)
                {
                    divisor -= 1;
                }

                if ((total / divisor) < 50)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

    }
}
