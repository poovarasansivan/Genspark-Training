using HRApi.Models.DTOs.FileHandlingDtos;
using HRApi.Interfaces;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;
using HRApi.Contexts;
using System.Threading.Tasks;

namespace HRApi.Repositories
{
    public class FileRepository : Repository<int,FileModel>
    {
       public FileRepository(DBContext context) : base(context)
        {
        }

        public override async Task<FileModel> Get(int id)
        {
            return await _context.Files.FindAsync(id);
        }

        public override async Task<IEnumerable<FileModel>> GetAll()
        {
            return await _context.Files.ToListAsync();
        }

    }
}