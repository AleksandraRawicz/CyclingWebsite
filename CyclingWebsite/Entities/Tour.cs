using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Entities
{
    public class Tour
    {
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
        [Range(1,1000)]
        public int Length { get; set; }
        public virtual IEnumerable<Photo> Photos { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(1,5)]
        public int Difficulty { get; set; }

    }
}
