using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class VideoUpdateDto
    {
        [Required]
        [StringLength(60, MinimumLength = 5, ErrorMessage = "Video title must be between 5 and 60 characters")]
        public string Title { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Video description must be between 3 and 250 characters")]
        public string Description { get; set; }
    }
}
