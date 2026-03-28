using Health.Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class WateenDbContext : IdentityDbContext<User, ApplicationRole, Guid>
{
    public WateenDbContext(DbContextOptions<WateenDbContext> options) : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<MedicalTask> MedicalTasks { get; set; }
    public DbSet<HomeServiceRequest> HomeServiceRequests { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasOne(u => u.Patient)
            .WithOne(p => p.User)
            .HasForeignKey<Patient>(p => p.Id);

        builder.Entity<User>()
            .HasOne(u => u.Doctor)
            .WithOne(d => d.User)
            .HasForeignKey<Doctor>(d => d.Id);

        builder.Entity<User>()
            .HasOne(u => u.Nurse)
            .WithOne(n => n.User)
            .HasForeignKey<Nurse>(n => n.Id);
    }
}
