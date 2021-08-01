using CyclingWebsite.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Models
{
    public class TourDto
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public IEnumerable<Photo> Photos { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public int Difficulty { get; set; }
    }
}
