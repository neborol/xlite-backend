using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string DateEntered { get; set; }
        public int Amount { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        
    }
}
