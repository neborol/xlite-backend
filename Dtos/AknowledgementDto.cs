using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class AknowledgementDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
