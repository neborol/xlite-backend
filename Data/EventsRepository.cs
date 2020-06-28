using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EliteForce.Data
{
    public class EventsRepository : IEventsRepository
    {
        private readonly EliteDataContext _context;
        public EventsRepository(EliteDataContext context)
        {
            _context = context;
        }

        public async Task<int> AddEvent(EventObj eventObj)
        {
            var evtObj = await _context.EventItems.AddAsync(eventObj);
            if (evtObj == null)
            {
                throw new Exception("The Event was not created.");
            }
            var numberAffected = await _context.SaveChangesAsync();

            return numberAffected;
        }

        public async Task<int> DeleteEvent(int eventId)
        {
            var event2Delete = await _context.EventItems.FindAsync(eventId);
            var eventRemoved = _context.EventItems.Remove(event2Delete);
            if (eventRemoved == null)
            {
                throw new Exception("Event object was not removed.");
            }
            var num = await _context.SaveChangesAsync();
            return num;
        }

        public async Task<int> UpdateAnEvent(EventObj eventObj)
        {
            var event2Update = await _context.EventItems.FindAsync(eventObj.EventId);
            if (event2Update == null)
            {
                throw new Exception("Event object was not updated.");
            }

            event2Update.Title = eventObj.Title;
            event2Update.Description = eventObj.Description;
            event2Update.Time = eventObj.Time;
            event2Update.EventDate = eventObj.EventDate;
            event2Update.Venue = eventObj.Venue;
            event2Update.Comment = eventObj.Comment;

            _context.EventItems.Update(event2Update);
            var num = await _context.SaveChangesAsync();
            return num;
        }

        public async Task<IEnumerable<EventObj>> GetEvents()
        {
            var eventObjects = await _context.EventItems.ToListAsync();
            if (eventObjects.Count == 0)
            {
                return new List<EventObj>();
            }
            return eventObjects;
        }
    }
}
