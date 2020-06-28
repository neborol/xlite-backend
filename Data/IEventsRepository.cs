using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IEventsRepository
    {
        Task<int> AddEvent(EventObj eventObj);
        Task<int> UpdateAnEvent(EventObj eventObj);
        Task<int> DeleteEvent(int eventId);
        Task<IEnumerable<EventObj>> GetEvents();
    }
}
