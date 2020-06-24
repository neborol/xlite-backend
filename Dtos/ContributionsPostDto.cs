using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class ContributionsPostDto
    {
        [Required]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "User ID must be between 3 and 60 characters")]
        public string UserId { get; set; }

        [Required]
        public int Amount { get; set; }

        [StringLength(9, MinimumLength = 3, ErrorMessage = "Month must be between 3 and 9 characters")]
        public string Month { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "Year must have exactly 4 characters")]
        public string Year { get; set; }

        [Required]
        public DateTime DateEntered { get; set; }

        [StringLength(600, MinimumLength = 5, ErrorMessage = "User name must be between 3 and 20 characters")]
        public string Comment { get; set; }
    }
}
