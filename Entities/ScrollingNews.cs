using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class ScrollingNews
    {
        [Key]
        public int NewsId { get; set; }
        public string NewsScrollbar { get; set; }
    }
}
