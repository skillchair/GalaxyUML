using GalaxyUML.Core.Models; // ObjectType
using GalaxyUML.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<TeamEntity> Teams => Set<TeamEntity>();
    public DbSet<TeamMemberEntity> TeamMembers => Set<TeamMemberEntity>();
    public DbSet<BannedUserEntity> BannedUsers => Set<BannedUserEntity>();
    public DbSet<MeetingEntity> Meetings => Set<MeetingEntity>();
    public DbSet<MeetingParticipantEntity> MeetingParticipants => Set<MeetingParticipantEntity>();
    public DbSet<ChatEntity> Chats => Set<ChatEntity>();
    public DbSet<MessageEntity> Messages => Set<MessageEntity>();
    public DbSet<DiagramElementEntity> DiagramElements => Set<DiagramElementEntity>();
    public DbSet<DiagramEntity> Diagrams => Set<DiagramEntity>();
    public DbSet<TextEntity> Texts => Set<TextEntity>();
    public DbSet<BoxEntity> Boxes => Set<BoxEntity>();
    public DbSet<ClassBoxEntity> ClassBoxes => Set<ClassBoxEntity>();
    public DbSet<LineEntity> Lines => Set<LineEntity>();
    public DbSet<ClassAttributeEntity> ClassAttributes => Set<ClassAttributeEntity>();
    public DbSet<ClassMethodEntity> ClassMethods => Set<ClassMethodEntity>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // TEAM MEMBER
        b.Entity<TeamMemberEntity>(e =>
        {
            e.HasIndex(x => new { x.TeamId, x.UserId }).IsUnique();
            e.HasOne(x => x.Team).WithMany(t => t.Members)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TEAM
        b.Entity<TeamEntity>(e =>
        {
            e.HasMany(t => t.BannedUsers).WithOne(bu => bu.Team)
                .HasForeignKey(bu => bu.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(t => t.Meetings).WithOne(m => m.Team)
                .HasForeignKey(m => m.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BANNED USER
        b.Entity<BannedUserEntity>(e =>
        {
            e.HasIndex(x => new { x.TeamId, x.UserId }).IsUnique();
            e.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MEETING
        b.Entity<MeetingEntity>(e =>
        {
            e.HasOne(m => m.Team).WithMany(t => t.Meetings)
                .HasForeignKey(m => m.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.OrganizedBy).WithMany()
                .HasForeignKey(m => m.OrganizedById)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.Board).WithOne()
                .HasForeignKey<MeetingEntity>(m => m.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Chat).WithOne(c => c.Meeting)
                .HasForeignKey<MeetingEntity>(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MEETING PARTICIPANT
        b.Entity<MeetingParticipantEntity>(e =>
        {
            e.HasIndex(x => new { x.MeetingId, x.TeamMemberId }).IsUnique();
            e.HasOne(x => x.Meeting).WithMany(m => m.Participants)
                .HasForeignKey(x => x.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.TeamMember).WithMany(tm => tm.Meetings)
                .HasForeignKey(x => x.TeamMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CHAT / MESSAGE
        b.Entity<ChatEntity>(e =>
        {
            e.HasOne(c => c.Meeting).WithOne(m => m.Chat)
                .HasForeignKey<ChatEntity>(c => c.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<MessageEntity>(e =>
        {
            e.HasOne(m => m.Chat).WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Sender).WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // DIAGRAM TPH
        b.Entity<DiagramElementEntity>().ToTable("DiagramElements")
            .HasDiscriminator<ObjectType>("ObjectType")
            .HasValue<DiagramEntity>(ObjectType.Diagram)
            .HasValue<TextEntity>(ObjectType.Text)
            .HasValue<BoxEntity>(ObjectType.Box)
            .HasValue<ClassBoxEntity>(ObjectType.ClassBox)
            .HasValue<LineEntity>(ObjectType.Line);

        // self-parent
        b.Entity<DiagramElementEntity>()
            .HasMany(d => d.Children).WithOne(c => c.Parent)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Box - Line (start/end)
        b.Entity<LineEntity>()
            .HasOne(l => l.StartBox).WithMany(bx => bx.Outgoing)
            .HasForeignKey(l => l.StartBoxId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<LineEntity>()
            .HasOne(l => l.EndBox).WithMany(bx => bx.Incoming)
            .HasForeignKey(l => l.EndBoxId)
            .OnDelete(DeleteBehavior.Restrict);

        // ClassBox owns attributes/methods
        b.Entity<ClassAttributeEntity>()
            .HasOne(a => a.ClassBox).WithMany(cb => cb.Attributes)
            .HasForeignKey(a => a.ClassBoxId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<ClassMethodEntity>()
            .HasOne(m => m.ClassBox).WithMany(cb => cb.Methods)
            .HasForeignKey(m => m.ClassBoxId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
