using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentRosterDbApi.Data;
using StudentRosterDbApi.DTOs;
using StudentRosterDbApi.Models;

namespace StudentRosterDbApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _context.Students
                .Include(s => s.Department) // Eager loading for relationships
                .Include(s => s.Courses)
                .ToListAsync();

            return _mapper.Map<List<StudentDto>>(students);
        }

        public async Task<StudentDto> CreateStudentAsync(StudentCreateDto dto)
        {
            // 1. Begin Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Map DTO to Entity
                var student = _mapper.Map<Student>(dto);
                _context.Students.Add(student);

                // 3. Create an audit log entry
                var auditLog = new AuditLog
                {
                    Action = $"Created student: {student.FirstName} {student.LastName}"
                };
                _context.AuditLogs.Add(auditLog);

                if (dto.CourseIds is { Count: > 0 })
                {
                    var courses = await _context.Courses
                        .Where(c => dto.CourseIds.Contains(c.Id))
                        .ToListAsync();
                    foreach (var course in courses)
                    {
                        student.Courses.Add(course);
                    }
                }

                // 4. Save all changes
                await _context.SaveChangesAsync();

                // 5. Commit transaction
                await transaction.CommitAsync();

                await _context.Entry(student).Reference(s => s.Department).LoadAsync();
                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception)
            {
                // 6. Rollback if anything fails
                await transaction.RollbackAsync();
                throw; // Re-throw to be caught by global error handler
            }
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return false;
                }

                _context.Students.Remove(student);
                _context.AuditLogs.Add(new AuditLog
                {
                    Action = $"Deleted student: {student.FirstName} {student.LastName} (Id {student.Id})"
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
