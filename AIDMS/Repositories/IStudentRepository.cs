using AIDMS.DTOs;
using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> GetAllStudentDataByIdAsync(int studentId);
        Task<Student> GetAllStudentDataByNameAsync(string studentName);
        Task<List<Student>> GetAllStudentsDataAsync();
        Task<List<Student>> GetAllStudentsPersonalInfoAsync();
        Task<bool?> AddStudentAsync(Student studnet);
        Task UpdateStudentAsync(int studentId, UserSettingsDto student);
        Task<bool?> DeleteStudentAsync(int studentId);
        Task<Student> GetStudentPersonalInfoByIdAsync(int studentId);
    }

}
