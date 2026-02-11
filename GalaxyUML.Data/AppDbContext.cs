using Microsoft.EntityFrameworkCore;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data
{
    public class AppDbContext : DbContext
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

        public DbSet<BoxEntity> Boxes { get; set; }
        public DbSet<ClassBoxEntity> ClassBoxes { get; set; }
        public DbSet<TextEntity> Texts { get; set; }
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

            modelBuilder.Entity<BannedUserEntity>()
                .HasKey(b => new { b.IdUser, b.IdTeam });

            modelBuilder.Entity<BannedUserEntity>()
                .HasOne(b => b.User)
                .WithMany(u => u.BannedTeams)
                .HasForeignKey(b => b.IdUser)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BannedUserEntity>()
                .HasOne(b => b.Team)
                .WithMany(t => t.BannedUsers)
                .HasForeignKey(b => b.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeetingParticipantEntity>()
                .HasKey(mp => new { mp.IdMeeting, mp.IdParticipant });

            modelBuilder.Entity<MeetingParticipantEntity>()
                .HasOne(mp => mp.Meeting)
                .WithMany(m => m.Participants)
                .HasForeignKey(mp => mp.IdMeeting)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeetingParticipantEntity>()
                .HasOne(mp => mp.Participant)
                .WithMany(p => p.Meetings)
                .HasForeignKey(mp => mp.IdParticipant)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeetingEntity>()
                .HasOne(m => m.Board)
                .WithOne(b => b.Meeting)
                .HasForeignKey<DiagramEntity>(b => b.IdMeeting)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeetingEntity>()
                .HasOne(m => m.Chat)
                .WithOne(c => c.Meeting)
                .HasForeignKey<ChatEntity>(m => m.IdMeeting)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageEntity>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.IdChat);

            modelBuilder.Entity<DiagramEntity>()
                .HasOne(d => d.Meeting)
                .WithOne(m => m.Board)
                .HasForeignKey<DiagramEntity>(d => d.IdMeeting)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiagramEntity>()
                .HasOne(d => d.Parent)
                .WithMany(p => p.Objects)
                .HasForeignKey(d => d.IdParent)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LineEntity>()
                .HasOne(l => l.Box1)
                .WithMany(b => b.Lines)
                .HasForeignKey(l => l.IdBox1)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LineEntity>()
                .HasOne(l => l.Box2)
                .WithMany(b => b.Lines)
                .HasForeignKey(l => l.IdBox2)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AttributeEntity>()
                .HasOne(a => a.ClassBox)
                .WithMany(c => c.Attributes)
                .HasForeignKey(a => a.IdClassBox)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MethodEntity>()
                .HasOne(m => m.ClassBox)
                .WithMany(c => c.Methods)
                .HasForeignKey(m => m.IdClassBox)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}