using Microsoft.EntityFrameworkCore;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data
{
    class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<TeamMemberEntity> Members { get; set; }
        public DbSet<BannedUserEntity> BannedUsers { get; set; }
        public DbSet<MeetingEntity> Meetings { get; set; }
        public DbSet<MeetingParticipantEntity> Participants { get; set; }
        public DbSet<ChatEntity> Chats { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<DiagramEntity> Diagrams { get; set; }
        public DbSet<DrawableEntity> Drawables { get; set; }
        public DbSet<TextEntity> Texts { get; set; }
        public DbSet<BoxEntity> Boxes { get; set; }
        public DbSet<ClassBoxEntity> ClassBoxes { get; set; }
        public DbSet<LineEntity> Lines { get; set; }
        public DbSet<AttributeEntity> Attributes { get; set; }
        public DbSet<MethodEntity> Methods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TeamMemberEntity>()
                .HasKey(tm => new { tm.IdTeam, tm.IdMember });

            modelBuilder.Entity<TeamMemberEntity>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(tm => tm.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMemberEntity>()
                .HasOne(tm => tm.Member)
                .WithMany(u => u.Teams)
                .HasForeignKey(tm => tm.IdMember)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserEntity>
        }
    }
}