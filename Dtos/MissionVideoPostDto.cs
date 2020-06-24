using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class MissionVideoPostDto
    {
        [Required]
        [StringLength(28, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 28 characters")]
        public string VideoTitle { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 60 characters")]
        public string VideoDescription { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Category must be between 5 and 40 characters")]
        public string VideoCategory { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Poster path must be between 5 and 250 characters")]
        public string VideoPhotoFilePath { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Video path must be between 5 and 250 characters")]
        public string VideoFilePath { get; set; }
    }
}
