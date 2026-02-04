using System;
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
        public DbSet<MeetingEntity> Meetings { get; set; }
        public DbSet<TeamMemberEntity> TeamMembers { get; set; }
        public DbSet<MeetingParticipantEntity> MeetingParticipants { get; set; }
        public DbSet<ChatEntity> Chats { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<DiagramEntity> Diagrams { get; set; }
        public DbSet<BoxEntity> Boxes { get; set; }
        public DbSet<LineEntity> Lines { get; set; }
        public DbSet<TextEntity> Texts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<TeamEntity>()
                .HasOne(t => t.TeamOwner)
                .WithMany(u => u.Teams)
                .HasForeignKey(t => t.IdTeamOwner)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MeetingEntity>()
                .HasOne(m => m.Organizer)
                .WithMany(u => u.OrganizedMeetings)
                .HasForeignKey(m => m.IdOrganizer)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MeetingEntity>()
                .HasOne(m => m.Team)
                .WithMany(t => t.Meetings)
                .HasForeignKey(m => m.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMemberEntity>()
                .HasKey(tm => new { tm.IdTeam, tm.IdMember });

            modelBuilder.Entity<TeamMemberEntity>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.IdTeam)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMemberEntity>()
                .HasOne(tm => tm.Member)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.IdMember)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MeetingParticipantEntity>()
                .HasKey(mp => new { mp.IdMeeting, mp.IdParticipant });

            modelBuilder.Entity<MeetingParticipantEntity>()
                .HasOne(mp => mp.Meeting)
                .WithMany(m => m.MeetingParticipants)
                .HasForeignKey(mp => mp.IdMeeting)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MeetingParticipantEntity>()
                .HasOne(mp => mp.Participant)
                .WithMany(u => u.MeetingParticipants)
                .HasForeignKey(mp => mp.IdParticipant)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatEntity>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.IdChat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageEntity>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.IdSender)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiagramEntity>()
                .HasMany(d => d.Boxes)
                .WithOne(b => b.Diagram)
                .HasForeignKey(b => b.IdDiagram)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiagramEntity>()
                .HasMany(d => d.Lines)
                .WithOne(l => l.Diagram)
                .HasForeignKey(l => l.IdDiagram)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiagramEntity>()
                .HasMany(d => d.Texts)
                .WithOne(t => t.Diagram)
                .HasForeignKey(t => t.IdDiagram)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}