using Microsoft.EntityFrameworkCore;
using GalaxyUML.Data.Entities;
using System.Linq;

namespace GalaxyUML.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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

            // --- 1. GLOBAL CASCADE RESET ---
            // This runs first, but explicit configs below will override where needed
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            // --- 2. USER ---
            // No outgoing FKs, only referenced by others

            // --- 3. TEAM MEMBER ---
            modelBuilder.Entity<TeamMemberEntity>(entity =>
            {
                entity.HasKey(tm => tm.Id);
                entity.HasIndex(tm => new { tm.IdTeam, tm.IdMember }).IsUnique();

                // TeamMember -> Team (configured below on TeamEntity side)
                entity.HasOne(tm => tm.Team)
                    .WithMany(t => t.Members)
                    .HasForeignKey(tm => tm.IdTeam)
                    .OnDelete(DeleteBehavior.NoAction);

                // TeamMember -> User
                entity.HasOne(tm => tm.Member)
                    .WithMany(u => u.Teams)
                    .HasForeignKey(tm => tm.IdMember)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- 4. TEAM ---
            modelBuilder.Entity<TeamEntity>(entity =>
            {
                // Team -> TeamOwner (TeamMember)
                entity.HasOne(t => t.TeamOwner)
                    .WithMany()
                    .HasForeignKey(t => t.IdTeamOwner)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- 5. BANNED USERS ---
            modelBuilder.Entity<BannedUserEntity>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.HasIndex(b => new { b.IdUser, b.IdTeam }).IsUnique();

                entity.HasOne(b => b.User)
                    .WithMany(u => u.BannedTeams)
                    .HasForeignKey(b => b.IdUser)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(b => b.Team)
                    .WithMany(t => t.BannedUsers)
                    .HasForeignKey(b => b.IdTeam)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- 6. MEETING ---
            modelBuilder.Entity<MeetingEntity>(entity =>
            {
                // Meeting -> Organizer (MeetingParticipant)
                entity.HasOne(m => m.Organizer)
                    .WithMany()
                    .HasForeignKey(m => m.IdOrganizer)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- 7. MEETING PARTICIPANT ---
            modelBuilder.Entity<MeetingParticipantEntity>(entity =>
            {
                // MeetingParticipant -> Meeting (Cascade: deleting meeting removes participants)
                entity.HasOne(mp => mp.Meeting)
                    .WithMany(m => m.Participants)
                    .HasForeignKey(mp => mp.IdMeeting)
                    .OnDelete(DeleteBehavior.Cascade);

                // MeetingParticipant -> TeamMember
                entity.HasOne(mp => mp.Participant)
                    .WithMany(tm => tm.Meetings)
                    .HasForeignKey(mp => mp.IdParticipant)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- 8. CHAT ---
            modelBuilder.Entity<ChatEntity>(entity =>
            {
                // Chat <-> Meeting (one-to-one)
                entity.HasOne(c => c.Meeting)
                    .WithOne(m => m.Chat)
                    .HasForeignKey<ChatEntity>(c => c.IdMeeting)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- 9. MESSAGE ---
            modelBuilder.Entity<MessageEntity>(entity =>
            {
                // Message -> Chat (Cascade: deleting chat removes messages)
                entity.HasOne(m => m.Chat)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.IdChat)
                    .OnDelete(DeleteBehavior.Cascade);

                // Message -> Sender (MeetingParticipant)
                entity.HasOne(m => m.Sender)
                    .WithMany()
                    .HasForeignKey(m => m.IdSender)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- 10. DIAGRAM (TPH hierarchy) ---

            // Self-referencing parent/child
            modelBuilder.Entity<DiagramEntity>()
                .HasOne(d => d.Parent)
                .WithMany(p => p.Objects)
                .HasForeignKey(d => d.IdParent)
                .OnDelete(DeleteBehavior.NoAction);

            // Diagram <-> Meeting (Board, one-to-one)
            modelBuilder.Entity<DiagramEntity>()
                .HasOne(d => d.Meeting)
                .WithOne(m => m.Board)
                .HasForeignKey<DiagramEntity>(d => d.IdMeeting)
                .OnDelete(DeleteBehavior.NoAction);

            // Line -> Box1 / Box2
            modelBuilder.Entity<LineEntity>()
                .HasOne(l => l.Box1)
                .WithMany(b => b.LinesAsStart)
                .HasForeignKey(l => l.IdBox1)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LineEntity>()
                .HasOne(l => l.Box2)
                .WithMany(b => b.LinesAsEnd)
                .HasForeignKey(l => l.IdBox2)
                .OnDelete(DeleteBehavior.NoAction);

            // ClassBox -> Attributes (Cascade: deleting class removes its attributes)
            modelBuilder.Entity<AttributeEntity>()
                .HasOne(a => a.ClassBox)
                .WithMany(c => c.Attributes)
                .HasForeignKey(a => a.IdClassBox)
                .OnDelete(DeleteBehavior.Cascade);

            // ClassBox -> Methods (Cascade: deleting class removes its methods)
            modelBuilder.Entity<MethodEntity>()
                .HasOne(m => m.ClassBox)
                .WithMany(c => c.Methods)
                .HasForeignKey(m => m.IdClassBox)
                .OnDelete(DeleteBehavior.Cascade);

            // TextEntity has a collection of TeamMembers — this seems like a design issue,
            // but we configure it explicitly to prevent shadow FKs
            modelBuilder.Entity<TextEntity>()
                .HasMany(t => t.TeamMembers)
                .WithOne()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}