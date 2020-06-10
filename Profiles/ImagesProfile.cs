using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EliteForce.Dtos;
using EliteForce.Entities;

namespace EliteForce.Profiles
{
    public class ImagesProfile : Profile
    {
        public ImagesProfile()
        {
            CreateMap<ImageUploadDto, MissionPhoto>();
        }
    }
}
