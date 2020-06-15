using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class MissionVideo
    {
        [Key]
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string PosterPath { get; set; }
        public string VideoPath { get; set; }
        public DateTime DateCreated { get; set; }
        public int Rating { get; set; }
    }
}
