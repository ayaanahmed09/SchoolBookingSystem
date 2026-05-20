using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolBookingSystem.Models.SchoolViewModels
{
    public class AssignedEquipmentData
    {
        public int EquipmentID { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }
    }
}