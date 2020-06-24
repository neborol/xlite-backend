using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

namespace EliteForce.Entities
{
    public class User : IdentityUser
    {

        // public int UserId { get; set; }
        //public string Username { get; set; }
        //public byte[] PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
        [PersonalData]
        public string CodeNr { get; set; }

        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        //public string UserEmail { get; set; }
        // public string Phone { get; set; }
        [PersonalData]
        public string City { get; set; }
        public DateTime DateJoined { get; set; }

        [PersonalData]
        public string Status { get; set; }

    }
}
