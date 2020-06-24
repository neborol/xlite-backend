using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Comment { get; set; }
        public DateTime DateEntered { get; set; }
        public int Amount { get; set; }
        public string UserId { get; set; }
        public bool UptoDate { get; set; }
        public int Total { get; set; }
        
    }
}
