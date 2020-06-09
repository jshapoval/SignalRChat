using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRMvcChat.Models
{
    public class FilesContext : DbContext
    {
        public DbSet<User> People { get; set; }
        public FilesContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }


}
