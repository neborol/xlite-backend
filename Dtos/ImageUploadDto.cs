using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class ImageUploadDto
    {
        public int ImageId {get; set;}

        [Required(ErrorMessage = "Please choose image to upload")]
        [Display(Name = "PhotoToUpload")]
        public IFormFile UploadedImage { get; set; }

        [StringLength(40)]
        public string UploadedBy { get; set; }

        [StringLength(100)]
        public string Category { get; set; }
        public string ImageCaption { get; set; }
    }
}
