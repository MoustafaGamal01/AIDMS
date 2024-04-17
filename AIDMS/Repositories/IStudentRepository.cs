using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<Student> GetStudentByNameAsync(string studentName);
        Task<List<Student>> GetAllStudentsAsync();
        Task AddStudentAsync(Student studnet);
        Task UpdateStudentAsync(int studentId, Student student);
        Task DeleteStudentAsync(int studentId);
    }

}
