using SchoolBookingSystem.Models;
using SchoolBookingSystem.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolBookingSystem.Data;

namespace SchoolBookingSystem.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult> Index(int? id, int? equipmentID)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = await _context.Instructors
                  .Include(i => i.OfficeAssignment)
                  .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Equipment)
                        .ThenInclude(i => i.Enrollments)
                            .ThenInclude(i => i.Booking)
                  .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Equipment)
                        .ThenInclude(i => i.Department)
                  .AsNoTracking()
                  .OrderBy(i => i.LastName)
                  .ToListAsync();

            if (id != null)
            {
                ViewData["InstructorID"] = id.Value;
                Instructor instructor = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single();
                viewModel.Equipments = instructor.CourseAssignments.Select(s => s.Equipment);
            }

            if (equipmentID != null)
            {
                ViewData["EquipmentID"] = equipmentID.Value;
                viewModel.Enrollments = viewModel.Equipments.Where(
                    x => x.EquipmentID == equipmentID).Single().Enrollments;
            }

            return View(viewModel);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            var instructor = new Instructor();
            instructor.CourseAssignments = new List<CourseAssignment>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstMidName,HireDate,LastName,OfficeAssignment")] Instructor instructor, string[] selectedEquipments)
        {
            if (selectedEquipments != null)
            {
                instructor.CourseAssignments = new List<CourseAssignment>();
                foreach (var equipment in selectedEquipments)
                {
                    var equipmentToAdd = new CourseAssignment { InstructorID = instructor.ID, EquipmentID = int.Parse(equipment) };
                    instructor.CourseAssignments.Add(equipmentToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var instructor = await _context.Instructors
        .Include(i => i.OfficeAssignment)
        .Include(i => i.CourseAssignments).ThenInclude(i => i.Equipment)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allEquipments = _context.Equipments;
            var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(c => c.EquipmentID));
            var viewModel = new List<AssignedEquipmentData>();
            foreach (var equipment in allEquipments)
            {
                viewModel.Add(new AssignedEquipmentData
                {
                    EquipmentID = equipment.EquipmentID,
                    Title = equipment.Title,
                    Assigned = instructorCourses.Contains(equipment.EquipmentID)
                });
            }
            ViewData["Equipments"] = viewModel;
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedEquipments)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                     .ThenInclude(i => i.Equipment)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "",
                i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
            {
                if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }
                UpdateInstructorCourses(selectedEquipments, instructorToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateInstructorCourses(selectedEquipments, instructorToUpdate);
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        private void UpdateInstructorCourses(string[] selectedEquipments, Instructor instructorToUpdate)
        {
            if (selectedEquipments == null)
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedEquipmentsHS = new HashSet<string>(selectedEquipments);
            var instructorEquipments = new HashSet<int>
                (instructorToUpdate.CourseAssignments.Select(c => c.Equipment.EquipmentID));
            foreach (var equipment in _context.Equipments)
            {
                if (selectedEquipmentsHS.Contains(equipment.EquipmentID.ToString()))
                {
                    if (!instructorEquipments.Contains(equipment.EquipmentID))
                    {
                        instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, EquipmentID = equipment.EquipmentID });
                    }
                }
                else
                {

                    if (instructorEquipments.Contains(equipment.EquipmentID))
                    {
                        CourseAssignment equipmentToRemove = instructorToUpdate.CourseAssignments.FirstOrDefault(i => i.EquipmentID == equipment.EquipmentID);
                        _context.Remove(equipmentToRemove);
                    }
                }
            }
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Instructor instructor = await _context.Instructors
        .Include(i => i.CourseAssignments)
        .SingleAsync(i => i.ID == id);

            var departments = await _context.Departments
        .Where(d => d.InstructorID == id)
        .ToListAsync();
            departments.ForEach(d => d.InstructorID = null);

            _context.Instructors.Remove(instructor);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
