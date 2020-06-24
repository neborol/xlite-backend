using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class Code
    {
        [Key]
        public int CodeId { get; set; }
        public string CodeNr { get; set; }
    }
}
