using EliteForce.Dtos;
using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public class VideosRepository : IVideosRepository
    {
        private readonly EliteDataContext _context;
        public VideosRepository(EliteDataContext context)
        {
            _context = context;
        }

        public async Task<int> AddAVideoItem(MissionVideoDto videoDataObj)
        {
            var newVideo = new MissionVideo();
            newVideo.Title = videoDataObj.Title;
            newVideo.Description = videoDataObj.Description;
            newVideo.Category = videoDataObj.Category;
            newVideo.PosterPath = videoDataObj.PosterPath;
            newVideo.VideoPath = videoDataObj.VideoPath;
            newVideo.DateCreated = DateTime.UtcNow;
            newVideo.Rating = 0;

            await _context.MissionVideos.AddAsync(newVideo);
            var numberInserted = _context.SaveChanges();
            return numberInserted;
        }
    }
}

