using CyclingWebsite.Attributes;
using CyclingWebsite.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Models
{
    public class TourCreateDto
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Summary { get; set; }
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Length { get; set; }

        public IEnumerable<Photo> Photos { get; set; }

        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public List<IFormFile> Files { get; set; }

        [ScaffoldColumn(false)]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ScaffoldColumn(false)]
        public User User { get; set; }

        [ScaffoldColumn(false)]
        public DateTime Date { get; set; }

        [Required]
        [Range(1,5)]
        public int Difficulty { get; set; }
    }
}
