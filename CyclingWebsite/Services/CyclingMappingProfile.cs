using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CyclingWebsite.Entities;
using CyclingWebsite.Models;

namespace CyclingWebsite.Services
{
    public class CyclingMappingProfile:Profile
    {
        public CyclingMappingProfile()
        {
            CreateMap<Tour, TourDto>();
            CreateMap<TourCreateDto, Tour>();
            CreateMap<Tour, TourCreateDto>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<PhotoDto, Photo>();
        }
    }
}
