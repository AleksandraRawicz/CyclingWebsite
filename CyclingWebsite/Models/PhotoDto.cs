using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CyclingWebsite.Attributes;

namespace CyclingWebsite.Models
{
    public class PhotoDto
    {
        [required]
        [MaxLength(100)]
        public string Description { get; set; }
        [ScaffoldColumn(false)]
        [required]
        public int TourId { get; set; }
        [required]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile File { get; set; }
    }
}
