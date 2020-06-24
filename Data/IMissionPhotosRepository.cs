using EliteForce.Dtos;
using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IMissionPhotosRepository
    {
        Task<int> AddAMissionPhoto(List<string> photoDataObj);
        Task<IEnumerable<MissionPhoto>> GetAllImages();
        Task<int> UpdateMissionPhoto(PhotoUpdateDto updateObject, int photoId, string code);
        Task<int> DeleteMissionPhoto(int photoId);
    }
}
