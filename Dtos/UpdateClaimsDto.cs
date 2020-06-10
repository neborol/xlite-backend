using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class UpdateClaimsDto
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
