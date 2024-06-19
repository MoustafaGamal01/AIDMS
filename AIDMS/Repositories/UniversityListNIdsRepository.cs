
using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Repositories
{
    public class UniversityListNIdsRepository : IUniversityListNIdsRepository
    {
        private readonly AIDMSContextClass _context;

        public UniversityListNIdsRepository(AIDMSContextClass context)
        {
            this._context = context;
        }

        public async Task<UniversityListNIds> CheckExistanceOfNationalId(string nationalId)
        {
            return await _context.UniversityListNIds.FirstOrDefaultAsync(i => i.NationalId == nationalId);
        }

    }
}
