using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class PasswordResetDto
    {
        [Required]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 30 characters")]
        public string Email { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters")]
        public string Password { get; set; }
    }
}
