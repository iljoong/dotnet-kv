using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiapp.Models
{
    public class Device
    {
        public string Model { get; set; }
        public float Ver { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }
}