using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> GetAllStudentDataByIdAsync(int studentId);
        Task<Student> GetAllStudentDataByNameAsync(string studentName);
        Task<List<Student>> GetAllStudentsDataAsync();
        Task<List<Student>> GetAllStudentsPersonalInfoAsync();
        Task AddStudentAsync(Student studnet);
        Task UpdateStudentAsync(int studentId, Student student);
        Task DeleteStudentAsync(int studentId);
        Task<Student> GetStudentPersonalInfoByIdAsync(int studentId);
    }

}
