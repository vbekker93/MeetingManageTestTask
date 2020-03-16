using MeetingService.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingService
{
    /// <summary>
    /// Контекст "Сервис встреч"
    /// </summary>
    public class MeetingBaseContext : DbContext
    {
        public MeetingBaseContext(DbContextOptions<MeetingBaseContext> options)
            : base(options)
        {
            /// Создание базы, если не инициализирована (Code-First)
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ///Настройка связи "многие-ко-многим"
            builder.Entity<MeetParticipant>().HasKey(x => new { x.MeetId, x.ParticipantId });

            builder.Entity<MeetParticipant>()
                .HasOne(m => m.Meet)
                .WithMany(ma => ma.MeetParticipants)
                .HasForeignKey(m => m.MeetId);

            builder.Entity<MeetParticipant>()
                .HasOne(m => m.Participant)
                .WithMany(ma => ma.MeetParticipants)
                .HasForeignKey(a => a.ParticipantId);

        }

        public DbSet<Meet> Meets { get; set; }

        public DbSet<Participant> Participants { get; set; }

        public DbSet<MeetParticipant> MeetParticipants { get; set; }
    }
}
