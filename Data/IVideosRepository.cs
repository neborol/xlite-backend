using EliteForce.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IVideosRepository
    {
        Task<int> AddAVideoItem(MissionVideoDto videoDataObj);
    }
}
