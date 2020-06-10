using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class ClaimItemsDto
    {
        public string Id { get; set; }
        public List<Claim> CurrentClaims { get; set; }
    }
}
