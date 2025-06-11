using Microsoft.EntityFrameworkCore;
using FitnessTracking.Models;

namespace FitnessTracking.Contexts
{
    public class FitnessContext : DbContext
    {
        public FitnessContext(DbContextOptions<FitnessContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<WorkoutModel> Workouts { get; set; }
        public DbSet<UserWorkOutPlanModel> UserWorkOutPlans { get; set; }
        public DbSet<WorkOutPlanModel> WorkOutPlans { get; set; }
        public DbSet<ProgressModel> ProgressUpdates { get; set; }
        public DbSet<ProgressImageModel> ProgressImage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserModel - WorkoutModel: One-to-Many
            modelBuilder.Entity<WorkoutModel>()
                .HasOne(w => w.User)
                .WithMany(u => u.Workouts)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkOutPlanModel - WorkoutModel: One-to-Many
            modelBuilder.Entity<WorkoutModel>()
                .HasOne(w => w.WorkOutPlan)
                .WithMany(p => p.Workouts)
                .HasForeignKey(w => w.WorkOutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserModel - UserWorkOutPlanModel: One-to-Many
            modelBuilder.Entity<UserWorkOutPlanModel>()
                .HasOne(uwp => uwp.User)
                .WithMany(u => u.UserWorkOutPlans)
                .HasForeignKey(uwp => uwp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkOutPlanModel - ProgressModel: One-to-Many
            modelBuilder.Entity<ProgressModel>()
                .HasOne(p => p.WorkOutPlan)
                .WithMany(w => w.ProgressUpdates)
                .HasForeignKey(p => p.WorkOutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProgressModel - ProgressImageModel: One-to-Many
            modelBuilder.Entity<ProgressModel>()
                .HasMany(p => p.ProgressImage)
                .WithOne(pi => pi.Progress)
                .HasForeignKey(pi => pi.ProgressId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
