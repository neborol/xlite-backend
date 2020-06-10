using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace EliteForce.Dtos
{
    public class NewsImage
    {
        public int ImageId { get; set; }

        [Required(ErrorMessage = "Please choose image to upload")]
        [Display(Name = "PhotoToUpload")]
        public IFormFile UploadedImage { get; set; }

        [StringLength(100)]
        public string Category { get; set; }
        
    }

}

