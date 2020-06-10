﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Dtos
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime DateEntered { get; set; }
        public float Amount { get; set; }
        public int MemberId { get; set; }
    }
}
