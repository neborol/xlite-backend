using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class EventObj
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 5, ErrorMessage = "Title should be between 5 and 60")]
        public string Title { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Description should be between 3 and 250")]
        public string Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "The time must be between 3 and 60 characters.")]
        public string Time { get; set; }

        [Required]
        [StringLength(140, MinimumLength = 3, ErrorMessage = "The venue must be between 3 and 140 characters")]
        public string Venue { get; set; }

        [StringLength(200, MinimumLength = 0, ErrorMessage = "The comment must be between 0 and 200 characters.")]
        public string Comment { get; set; }
    }
}
