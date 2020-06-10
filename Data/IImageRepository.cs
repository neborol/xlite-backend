using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IImageRepository
    {
        Task<int> Add(MissionPhoto photo);
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<MissionPhoto>> GetMissionPhotos();
        Task<MissionPhoto> GetAMissionPhoto(int id);
    }
}
