using AutoMapper;
using CyclingWebsite.Authorization;
using CyclingWebsite.Entities;
using CyclingWebsite.Exceptions;
using CyclingWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Services
{
    public interface IToursService
    {
        void AddTour(TourCreateDto dto);
        void Delete(int id);
        void Edit(TourCreateDto dto, int id);
        TourDto Get(int id);
        PageResult<TourDto> GetAll(SearchFilters search);
        public IEnumerable<TourDto> GetAllForUser();
        public TourCreateDto Edit(int id);
        public TourCreateDto AddPhoto(PhotoDto dto);
        public TourCreateDto DeletePhoto(int id);
    }

    public class ToursService : IToursService
    {
        private readonly WebsiteDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IAuthorizationService _authorizationService;
        public ToursService(WebsiteDbContext context, IMapper mapper, IUserContextService userContextService,
            IWebHostEnvironment environment, IAuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
            _hostingEnvironment = environment;
            _authorizationService = authorizationService;
        }

        public PageResult<TourDto> GetAll(SearchFilters search)
        {
            if (search.Size <= 0 )
                search.Size = 5; 
            if (search.Page <= 0)
                search.Page = 1;

            var baseQuery = _context
                .Tours
                .Include(t => t.User)
                .Where(t => search.SearchPhrase == null || t.User.Name.ToLower().Contains(search.SearchPhrase.ToLower()) ||
                t.Name.ToLower().Contains(search.SearchPhrase.ToLower()) || t.Summary.ToLower().Contains(search.SearchPhrase.ToLower()))
                .OrderByDescending(t => t.Date);

            int total = baseQuery.Count();

            var tours = baseQuery
                .Skip(search.Size * (search.Page - 1))
                .Take(search.Size)
                .ToList();
                
            var dtos = _mapper.Map<List<TourDto>>(tours);

            var result = new PageResult<TourDto>(dtos, total, search.Size, search.Page, search.SearchPhrase);
            return result;
        }

        
        public IEnumerable<TourDto> GetAllForUser()
        {
            var userId = (int)_userContextService.GetUserId;
            var tours = _mapper.Map<List<TourDto>>(_context.Tours.Where(t=>t.UserId == userId).Include(t => t.User).OrderByDescending(t=>t.Date).ToList());
            return tours;
        }

        public TourDto Get(int id)
        {
            var tour = _mapper.Map<TourDto>(_context.Tours.Include(t => t.User).Include(t=>t.Photos).FirstOrDefault(t => t.Id == id));
            return tour;
        }

        public void AddTour(TourCreateDto dto)
        {
            dto.UserId = (int)_userContextService.GetUserId;
            dto.Date = DateTime.Now;
            Tour newTour = _mapper.Map<Tour>(dto);

            _context.Tours.Add(newTour);
            _context.SaveChanges();

            if (dto.Files is not null)
            {
                foreach (var file in dto.Files)
                {
                    var uniqueFileName = GetUniqueFileName(file.FileName);
                    string foldername = "tour_" + newTour.Id;
                    var path = Path.Combine(_hostingEnvironment.WebRootPath, "photos", foldername);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var filePath = Path.Combine(path, uniqueFileName);
                    file.CopyTo(new FileStream(filePath, FileMode.Create));

                    var photo = new Photo() { Name = uniqueFileName, TourId = newTour.Id };
                    _context.Photos.Add(photo);
                }
            }

            _context.SaveChanges();                       
        }

        public void Delete(int id)
        {           
            var tour = _context.Tours.FirstOrDefault(t => t.Id == id);

            if (tour is not null)
            {
                var user = _userContextService.User;
                var result = _authorizationService.AuthorizeAsync(user, tour, new ResourceRequirement(OperationOnResource.Delete)).Result;

                if (!result.Succeeded)
                {
                    throw new ForbidException();
                }

                string foldername = "tour_" + id;
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "photos", foldername);
                if (Directory.Exists(path))
                {
                    foreach (var file in Directory.GetFiles(path))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(path, true);
                }

                var photos = _context.Photos.Where(p => p.TourId == id).ToList();
                _context.Photos.RemoveRange(photos);

                _context.Tours.Remove(tour);

                _context.SaveChanges();
            }
        }

        public void Edit(TourCreateDto dto, int id)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == id);

            if(tour is not null)
            {
                var user = _userContextService.User;
                var result = _authorizationService.AuthorizeAsync(user, tour, new ResourceRequirement(OperationOnResource.Edit)).Result;

                if (!result.Succeeded)
                {
                    throw new ForbidException();
                }

                tour.Length = dto.Length;
                tour.Description = dto.Description;
                tour.Name = dto.Name;
                tour.Summary = dto.Summary;
                tour.Photos = dto.Photos;

                _context.Tours.Update(tour);
                _context.SaveChanges();
            }            
        }

        public TourCreateDto Edit(int id)
        {
            var tour = _context.Tours.Include(t => t.Photos).FirstOrDefault(t => t.Id == id);
              
            var dto = _mapper.Map<TourCreateDto>(tour);

            return dto;
        }

        public TourCreateDto AddPhoto(PhotoDto dto)
        {
            if(dto.File is not null)
            {
                var user = _userContextService.User;
                var tour = _context.Tours.Include(t=>t.Photos).FirstOrDefault(t => t.Id == dto.TourId);
                var result = _authorizationService.AuthorizeAsync(user, tour, new ResourceRequirement(OperationOnResource.Edit)).Result;

                if (!result.Succeeded)
                {
                    throw new ForbidException();
                }

                var uniqueFileName = GetUniqueFileName(dto.File.FileName);
                string foldername = "tour_" + dto.TourId;
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "photos", foldername);
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var filePath = Path.Combine(path, uniqueFileName);
                var s = new FileStream(filePath, FileMode.Create);
                dto.File.CopyTo(s);
                s.Close();
                s.Dispose();

                var photo = _mapper.Map<Photo>(dto);
                photo.Name = uniqueFileName;
                _context.Photos.Add(photo);
                _context.SaveChanges();

                var editedtour = _mapper.Map<TourCreateDto>(tour);
                return editedtour;
            }
            return new TourCreateDto();
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        public TourCreateDto DeletePhoto(int id)
        {
            var photo = _context.Photos.FirstOrDefault(p => p.Id == id);

            if (photo is not null)
            {
                var user = _userContextService.User;
                var tour = _context.Tours.Include(t=>t.Photos).FirstOrDefault(t => t.Id == photo.TourId);
                var result = _authorizationService.AuthorizeAsync(user, tour, new ResourceRequirement(OperationOnResource.Edit)).Result;

                if (!result.Succeeded)
                {
                    throw new ForbidException();
                }

                string foldername = "tour_" + photo.TourId;
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "photos", foldername, photo.Name);

                if(File.Exists(path))
                {                    
                    File.Delete(path);
                }

                _context.Photos.Remove(photo);
                _context.SaveChanges();

                var dto = _mapper.Map<TourCreateDto>(tour);
                return dto;
            }
            return new TourCreateDto();
        }
    }
}
