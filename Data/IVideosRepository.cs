using EliteForce.Dtos;
using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IVideosRepository
    {
        Task<int> AddAVideoItem(MissionVideoPostDto videoDataObj);
        Task<List<MissionVideo>> GetVideos();
        Task<List<MissionVideo>> GetCategorizedVideos(string category);
        Task<bool> RateVideo(RatingsDto ratingsObject);
        Task<int> UpdateVideo(VideoUpdateDto vUpdateObj, int id);
        Task<int> DeleteVideo(int id);
    }
}
