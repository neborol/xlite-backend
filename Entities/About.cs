using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class About
    {
        [Key]
        public int Id { get; set; }
        public string Statement1 { get; set; }
        public string Statement2 { get; set; }
        public string Statement3 { get; set; }
        public string ImagePath { get; set; }
    }
}
