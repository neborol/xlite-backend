
using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IFinancesRepository
    {
        Task<List<Subscription>> GetSubscriptions(string userId);
        Task<int> GetTotalForMembers();
    }
}
