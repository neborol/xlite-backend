using EliteForce.Data;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public class MissionPhotosRepository : IMissionPhotosRepository
    {
        private readonly EliteDataContext _context;
        private readonly ILogger _logger;
        

        public MissionPhotosRepository(EliteDataContext context, ILogger<MissionPhotosRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> AddAMissionPhoto(List<string> urlList)
        {
            

            foreach (var data in urlList)
            { 
                var photo = new MissionPhoto();
                photo.UniquePhotoName = data;
                await _context.MissionPhotos.AddAsync(photo);
            }

            var numberCreated = _context.SaveChanges();
            return numberCreated;
        }

        public async Task<IEnumerable<MissionPhoto>> GetAllImages()
        {
            var photos = await _context.MissionPhotos.ToListAsync();
            
            return photos;
        }

        public async Task<int> UpdateMissionPhoto(PhotoUpdateDto updateObject, int photoId, string code)
        {
            var photo2bUpdated = await _context.MissionPhotos.FindAsync(photoId);
            photo2bUpdated.Category = updateObject.Category;
            photo2bUpdated.ImageCaption = updateObject.Caption;
            photo2bUpdated.UploadedBy = code;
            _context.MissionPhotos.Update(photo2bUpdated);
            var numberAffected = await _context.SaveChangesAsync();

            return numberAffected;
        }

        public async Task<int> DeleteMissionPhoto(int photoId)
        {
            var photo2bDeleted = await _context.MissionPhotos.FindAsync(photoId);
            _context.MissionPhotos.Remove(photo2bDeleted);
            var numberAffected = await _context.SaveChangesAsync();

            return numberAffected;
        }
    }
}
