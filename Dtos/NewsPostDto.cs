using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class NewsPostDto
    {
        [Required]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "The news title should be between 10 and 120")]
        public string NewsTitle { get; set; }

        [Required]
        [StringLength(350, MinimumLength = 10, ErrorMessage = "The news summary should be between 10 and 200")]
        public string NewsSummary { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "The news full story should be between 10 and 120")]
        public string NewsFullStory { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 3, ErrorMessage = "The category name is faulty")]
        public string NewsCategory { get; set; }

        [Required]
        public string ImagePath { get; set; }
    }
}
