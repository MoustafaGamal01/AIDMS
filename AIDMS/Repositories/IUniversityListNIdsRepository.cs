using AIDMS.Entities;
using System.Threading.Tasks;

namespace AIDMS.Repositories
{
    public interface IUniversityListNIdsRepository
    {
        Task <UniversityListNIds> CheckExistanceOfNationalId(string NationalId);
    }
}
