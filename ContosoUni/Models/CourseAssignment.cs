using SchoolBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolBookingSystem.Models
{
    public class CourseAssignment
    {
        public int InstructorID { get; set; }
        public int EquipmentID { get; set; }
        public Instructor Instructor { get; set; }
        public Equipment Equipment { get; set; }
    }
}
