using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EliteForce.Data
{
    public class ImageRepository: IImageRepository
    {
        private EliteDataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        


        public ImageRepository(
            EliteDataContext context, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<ImageRepository> logger
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<int> Add(MissionPhoto photo)
        {
            _context.Add(photo);
            var confrm = await _context.SaveChangesAsync();
            return confrm;
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public async Task<IEnumerable<MissionPhoto>> GetMissionPhotos()
        {
            var missionPhotos = await _context.MissionPhotos.ToListAsync();
            return missionPhotos;
        }
        public async Task<MissionPhoto> GetAMissionPhoto(int id)
        {
            var missionPhoto = await _context.MissionPhotos.FindAsync(id);
            return missionPhoto;
        }


        public Task<bool> SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}
