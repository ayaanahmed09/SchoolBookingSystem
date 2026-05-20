using SchoolBookingSystem.Data;
using SchoolBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace SchoolBookingSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            context.Database.EnsureCreated();

            // Look for any bookings.
            if (context.Bookings.Any())
            {
                return;   // DB has been seeded
            }

            var bookings = new Booking[]
            {
                new Booking { FirstMidName = "Carson",   LastName = "Alexander",
                    EnrollmentDate = DateTime.Parse("2010-09-01") },
                new Booking { FirstMidName = "Meredith", LastName = "Alonso",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Booking { FirstMidName = "Arturo",   LastName = "Anand",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Booking { FirstMidName = "Gytis",    LastName = "Barzdukas",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Booking { FirstMidName = "Yan",      LastName = "Li",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Booking { FirstMidName = "Peggy",    LastName = "Justice",
                    EnrollmentDate = DateTime.Parse("2011-09-01") },
                new Booking { FirstMidName = "Laura",    LastName = "Norman",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Booking { FirstMidName = "Nino",     LastName = "Olivetto",
                    EnrollmentDate = DateTime.Parse("2005-09-01") }
            };

            foreach (Booking s in bookings)
            {
                context.Bookings.Add(s);
            }
            context.SaveChanges();

            var instructors = new Instructor[]
            {
                new Instructor { FirstMidName = "Kim",     LastName = "Abercrombie",
                    HireDate = DateTime.Parse("1995-03-11") },
                new Instructor { FirstMidName = "Fadi",    LastName = "Fakhouri",
                    HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { FirstMidName = "Roger",   LastName = "Harui",
                    HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { FirstMidName = "Candace", LastName = "Kapoor",
                    HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { FirstMidName = "Roger",   LastName = "Zheng",
                    HireDate = DateTime.Parse("2004-02-12") }
            };

            foreach (Instructor i in instructors)
            {
                context.Instructors.Add(i);
            }
            context.SaveChanges();

            var departments = new Department[]
            {
                new Department { Name = "English",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Abercrombie").ID },
                new Department { Name = "Mathematics", Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Fakhouri").ID },
                new Department { Name = "Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Harui").ID },
                new Department { Name = "Economics",   Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Kapoor").ID }
            };

            foreach (Department d in departments)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            var equipments = new Equipment[]
            {
                new Equipment {EquipmentID = 1050, Title = "Chemistry",      Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Engineering").DepartmentID
                },
                new Equipment {EquipmentID = 4022, Title = "Microeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID
                },
                new Equipment {EquipmentID = 4041, Title = "Macroeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID
                },
                new Equipment {EquipmentID = 1045, Title = "Calculus",       Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID
                },
                new Equipment {EquipmentID = 3141, Title = "Trigonometry",   Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID
                },
                new Equipment {EquipmentID = 2021, Title = "Composition",    Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "English").DepartmentID
                },
                new Equipment {EquipmentID = 2042, Title = "Literature",     Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "English").DepartmentID
                },
            };

            foreach (Equipment c in equipments)
            {
                context.Equipments.Add(c);
            }
            context.SaveChanges();

            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment {
                    InstructorID = instructors.Single( i => i.LastName == "Fakhouri").ID,
                    Location = "Smith 17" },
                new OfficeAssignment {
                    InstructorID = instructors.Single( i => i.LastName == "Harui").ID,
                    Location = "Gowan 27" },
                new OfficeAssignment {
                    InstructorID = instructors.Single( i => i.LastName == "Kapoor").ID,
                    Location = "Thompson 304" },
            };

            foreach (OfficeAssignment o in officeAssignments)
            {
                context.OfficeAssignments.Add(o);
            }
            context.SaveChanges();

            var equipmentInstructors = new CourseAssignment[]
            {
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Chemistry" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Kapoor").ID
                    },
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Chemistry" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").ID
                    },
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Microeconomics" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
                    },
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Macroeconomics" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
                    },
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Calculus" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Fakhouri").ID
                    },
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Trigonometry" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").ID
                    },
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Composition" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
                    },
                new CourseAssignment {
                    EquipmentID = equipments.Single(c => c.Title == "Literature" ).EquipmentID,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
                    },
            };

            foreach (CourseAssignment ci in equipmentInstructors)
            {
                context.CourseAssignments.Add(ci);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Alexander"),
                    EquipmentID = equipments.Single(c => c.Title == "Chemistry" ).EquipmentID,
                    Grade = Grade.A
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Alexander"),
                    EquipmentID = equipments.Single(c => c.Title == "Microeconomics" ).EquipmentID,
                    Grade = Grade.C
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Alexander"),
                    EquipmentID = equipments.Single(c => c.Title == "Macroeconomics" ).EquipmentID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Alonso"),
                    EquipmentID = equipments.Single(c => c.Title == "Calculus" ).EquipmentID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Alonso"),
                    EquipmentID = equipments.Single(c => c.Title == "Trigonometry" ).EquipmentID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Alonso"),
                    EquipmentID = equipments.Single(c => c.Title == "Composition" ).EquipmentID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Anand"),
                    EquipmentID = equipments.Single(c => c.Title == "Chemistry" ).EquipmentID,
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Anand"),
                    EquipmentID = equipments.Single(c => c.Title == "Microeconomics").EquipmentID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Barzdukas"),
                    EquipmentID = equipments.Single(c => c.Title == "Chemistry").EquipmentID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Li"),
                    EquipmentID = equipments.Single(c => c.Title == "Composition").EquipmentID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    Booking = bookings.Single(s => s.LastName == "Justice"),
                    EquipmentID = equipments.Single(c => c.Title == "Literature").EquipmentID,
                    Grade = Grade.B
                    }
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                            s.Booking.ID == e.BookingID &&
                            s.Equipment.EquipmentID == e.EquipmentID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();
        }
    }
}