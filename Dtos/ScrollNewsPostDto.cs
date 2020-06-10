using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class ScrollNewsPostDto
    {
        [Required]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "The scolling news message should be between 10 and 200")]
        public string NewsScrollbar { get; set; }
    }
}
