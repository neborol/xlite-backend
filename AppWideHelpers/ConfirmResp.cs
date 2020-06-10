using EliteForce.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.AppWideHelpers
{
    public class ConfirmResp : IConfirmResp
    {
        public AknowledgementDto Confirm;
        public ConfirmResp()
        {
            this.Confirm = new AknowledgementDto();
        }
        public AknowledgementDto ConfirmResponse(bool Success, string Message)
        {
            Confirm.Success = Success;
            Confirm.Message = Message;
            return Confirm;
        }
    }
}
