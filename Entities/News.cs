using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Entities
{
    public class News
    {
        public int NewsId { get; set; }
        public string NewsTitle { get; set; }
        public string NewsSummary { get; set; }
        public string NewsFullStory { get; set; }
        public string NewsCategory { get; set; }
        public DateTime DatePublished { get; set; }
        public string ImagePath { get; set; }
    }
}
