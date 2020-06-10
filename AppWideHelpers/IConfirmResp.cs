using EliteForce.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.AppWideHelpers
{
    public interface IConfirmResp
    {
        public AknowledgementDto ConfirmResponse(bool Success, string Message);
    }
}
