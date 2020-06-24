using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class Faq
    {
        [Key]
        public string FaqId { get; set; }
        public string FaqAnswer { get; set; }
        public string FaqQuestion { get; set; }
    }
}
