using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Implementations.EFModels
{
    public class YaDiskPlayerDbContext: DbContext
    {
        public YaDiskPlayerDbContext(DbContextOptions<YaDiskPlayerDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SynchronizationProcess> SynchronizationProcesses { get; set; }
        public DbSet<SynchronizationProcessUserCancellation> SynchronizationProcessUserCancellations { get; set; }
        public DbSet<SynchronizationProcessError> SynchronizationProcessErrors { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<IgnorePath> IgnorePaths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<SynchronizationProcess>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<SynchronizationProcessUserCancellation>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne<SynchronizationProcess>().WithMany().HasForeignKey(x => x.SynchronizationProcessId);
            });

            modelBuilder.Entity<SynchronizationProcessError>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne<SynchronizationProcess>().WithMany().HasForeignKey(x => x.SynchronizationProcessId);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne<SynchronizationProcess>().WithMany().HasForeignKey(x => x.SynchronizationProcessId);
            });

            modelBuilder.Entity<IgnorePath>(entity =>
            {
                entity.HasKey(x => x.Id);
            });
        }
    }
}
