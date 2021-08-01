using CyclingWebsite.Models;
using CyclingWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Controllers
{
    [Authorize]
    [Route("Tour")]
    public class TourController : Controller
    {
        private readonly IToursService _service;
        public TourController(IToursService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll(SearchFilters search)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var tours = _service.GetAll(search);
            if (isAjax)
            {
                return PartialView("_Tours", tours);
            }
            return View("Tours", tours);
        }

        
        [HttpGet]
        [Route("GetAllForUser")]
        public IActionResult GetAllForUser()
        {
            var tours = _service.GetAllForUser();
            return View("MyTours", tours);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            var tour = _service.Get(id);
            return View("Tour", tour);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add([FromForm] TourCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                _service.AddTour(dto);
                return RedirectToAction("GetAll", "Tour");
            }
            return View("~/Views/Account/MyAccount.cshtml");
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _service.Delete(id);
            return RedirectToAction("GetAllForUser", "Tour");
        }

        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit([FromForm] TourCreateDto dto)
        {
            _service.Edit(dto, dto.Id);
            return RedirectToAction("GetAll", "Tour");
        }

        [HttpGet]
        [Route("EditView/{id}")]
        public IActionResult EditView([FromRoute] int id)
        {
            var toEdit = _service.Edit(id);
            return View("EditMyTour", toEdit);
        }

        [HttpGet]
        [Route("AddPhotoForm/{id}")]
        public IActionResult AddPhotoForm([FromRoute] int id)
        {
            PhotoDto dto = new PhotoDto() { TourId = id };
            return PartialView("_AddPhoto", dto);
        }

        [HttpPost]
        [Route("AddPhoto")]
        public IActionResult AddPhoto([FromForm] PhotoDto dto)
        {
            var tour = _service.AddPhoto(dto);
            return View("EditMyTour", tour);
        }

        [HttpGet]
        [Route("DeletePhoto/{id}")]
        public IActionResult DeletePhoto([FromRoute] int id)
        {
            var tour =_service.DeletePhoto(id);
            return View("EditMyTour", tour);
        }

        
    }
}
