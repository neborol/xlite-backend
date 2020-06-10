using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class FaqPostDto
    {
        [Required]
        [StringLength(250, MinimumLength = 6, ErrorMessage = "Your question is too short.")]
        public string FaqQuestion { get; set; }
        [Required]
        [StringLength(600, MinimumLength = 6, ErrorMessage = "Your answer is too short.")]
        public string FaqAnswer { get; set; }
    }
}
