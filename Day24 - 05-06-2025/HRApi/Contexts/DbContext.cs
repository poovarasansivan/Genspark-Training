using Microsoft.EntityFrameworkCore;
using HRApi.Models;

namespace HRApi.Contexts
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FileModel> Files { get; set; }  
        public DbSet<User> Users { get; set; }
    }
}
