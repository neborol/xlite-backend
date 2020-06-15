using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class MissionVideoDto
    {
        [Required]
        [StringLength(28, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 28 characters")]
        public string Title { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 60 characters")]
        public string Description { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Category must be between 5 and 40 characters")]
        public string Category { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Poster path must be between 5 and 250 characters")]
        public string PosterPath { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Video path must be between 5 and 250 characters")]
        public string VideoPath { get; set; }
    }
}
