using EliteForce.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EliteForce.Dtos;

namespace EliteForce.Profiles
{
    public class VideosProfile : Profile
    {
        public VideosProfile()
        {
            CreateMap<MissionVideo, MissionVideoGetDto>();
        }
    }
}
