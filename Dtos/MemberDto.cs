using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class Member
    {
        public int MemberId { get; set; }
        public string CodeNr { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
        public int SubScriptionId { get; set; }
    }
}
