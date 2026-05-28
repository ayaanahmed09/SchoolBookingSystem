using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolBookingSystem.Migrations
{
    [Migration("20260528220000_FixCourseAssignmentKey")]
    public partial class FixCourseAssignmentKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
IF OBJECT_ID(N'dbo.CourseAssignment', N'U') IS NOT NULL
BEGIN
    -- create new table with desired composite primary key
    IF OBJECT_ID(N'dbo.CourseAssignment_new', N'U') IS NOT NULL
        DROP TABLE dbo.CourseAssignment_new;

    CREATE TABLE dbo.CourseAssignment_new (
        EquipmentID int NOT NULL,
        InstructorID int NOT NULL,
        CONSTRAINT PK_CourseAssignment_new PRIMARY KEY (EquipmentID, InstructorID)
    );

    -- copy data from existing table, mapping CourseID -> EquipmentID if necessary
    IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'dbo.CourseAssignment') AND name = 'EquipmentID')
    BEGIN
        INSERT INTO dbo.CourseAssignment_new (EquipmentID, InstructorID)
        SELECT EquipmentID, InstructorID FROM dbo.CourseAssignment;
    END
    ELSE IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'dbo.CourseAssignment') AND name = 'CourseID')
    BEGIN
        INSERT INTO dbo.CourseAssignment_new (EquipmentID, InstructorID)
        SELECT CourseID, InstructorID FROM dbo.CourseAssignment;
    END
    ELSE
    BEGIN
        -- fallback: try to copy nothing if structure is unexpected
        PRINT 'CourseAssignment table structure not recognized; new empty table created.';
    END

    -- drop old table (this will drop its FKs)
    DROP TABLE dbo.CourseAssignment;

    -- rename new table into place
    EXEC sp_rename N'dbo.CourseAssignment_new', N'CourseAssignment';

    -- recreate foreign keys to Equipment and Instructor
    IF OBJECT_ID(N'dbo.Equipment', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Instructor', N'U') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.CourseAssignment
            ADD CONSTRAINT FK_CourseAssignment_Equipment_EquipmentID FOREIGN KEY (EquipmentID) REFERENCES dbo.Equipment(EquipmentID) ON DELETE CASCADE;

        ALTER TABLE dbo.CourseAssignment
            ADD CONSTRAINT FK_CourseAssignment_Instructor_InstructorID FOREIGN KEY (InstructorID) REFERENCES dbo.Instructor(ID) ON DELETE CASCADE;
    END
END
";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Non-trivial to reverse safely; no-op.
        }
    }
}
