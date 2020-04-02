using System;
using System.Collections.Generic;
using System.Text;

namespace ReservationProcessor
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public string For { get; set; }
        public DateTime ReservationCreated { get; set; }
        public string Books { get; set; }
        public int Status { get; set; }
    }
}
