using EliteForce.Dtos;
using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EliteForce.Data
{
    public class VideosRepository : IVideosRepository
    {
        private readonly EliteDataContext _context;
        public VideosRepository(EliteDataContext context)
        {
            _context = context;
        }

        public async Task<int> AddAVideoItem(MissionVideoPostDto videoDataObj)
        {
            var newVideo = new MissionVideo();
            newVideo.Title = videoDataObj.VideoTitle;
            newVideo.Description = videoDataObj.VideoDescription;
            newVideo.Category = videoDataObj.VideoCategory;
            newVideo.PosterPath = videoDataObj.VideoPhotoFilePath;
            newVideo.VideoPath = videoDataObj.VideoFilePath;
            newVideo.DateCreated = DateTime.UtcNow;
            newVideo.Rating = 0;

            await _context.MissionVideos.AddAsync(newVideo);
            var numberInserted = _context.SaveChanges();
            return numberInserted;
        }


        public async Task<List<MissionVideo>> GetVideos()
        {
            var videos =  await _context.MissionVideos.ToListAsync();

            if (videos.Count == 0)
            {
                throw new Exception("No Video was found");
            }
            return videos;
        }



        public async Task<List<MissionVideo>> GetCategorizedVideos(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new Exception("A category was not provided.");
            }

            var videos = await _context.MissionVideos.Where(v => v.Category == category).ToListAsync();

            if (videos.Count == 0)
            {
                throw new Exception("No Video was found");
            }
            return videos;
        }


        public async Task<bool> RateVideo(RatingsDto ratingsObject)
        {
            var video = await _context.MissionVideos.FindAsync(ratingsObject.VideoId);
            video.RatingsCount += 1;
            video.Rating = ratingsObject.Rating / video.RatingsCount;
             _context.Update(video);
            var numbr = await _context.SaveChangesAsync();

            if (numbr.Equals(0))
            {
                return false;
            }

            return true;
        }


    }
}
