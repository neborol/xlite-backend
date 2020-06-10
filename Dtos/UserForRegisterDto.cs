using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "User name must be between 3 and 20 characters")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Email must be between 5 and 25 characters")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "You must specify password between 6 and 10 characters")]
        public string Password { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Your first name should be beteen 3 and 20 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Your last name should be beteen 3 and 20 characters")]
        public string LastName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Your phone number should be beteen 10 and 15 characters")]
        public string Phone { get; set; }

        [StringLength(25, MinimumLength = 0, ErrorMessage = "Your city should not be more than 25 characters")]
        public string City { get; set; }
    }
}
