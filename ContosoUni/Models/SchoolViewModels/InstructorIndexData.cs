using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolBookingSystem.Models.SchoolViewModels
{
    public class InstructorIndexData
    {
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Equipment> Equipments { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}
