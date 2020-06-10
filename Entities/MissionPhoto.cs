using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class MissionPhoto
    {
        public int MissionPhotoId { get; set; }

        [Required]
        [StringLength(255)]
        public string UniquePhotoName { get; set; } 

        public DateTime UploadDate { get; set; }

        [StringLength(40)]
        public string UploadedBy { get; set; }

        public string ImageCaption { get; set; }
        public string Category { get; set; }
    }
}
