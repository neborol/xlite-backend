using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class Member
    {
        public Member()
        {
            Subscriptions = new List<Subscription>();
        }
        public int MemberId { get; set; }
        public string CodeNr { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public List<Subscription> Subscriptions { get; set; }
    }
}
