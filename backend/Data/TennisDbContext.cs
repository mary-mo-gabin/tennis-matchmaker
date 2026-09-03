using Microsoft.EntityFrameworkCore;
using TennisMatchmaker.Models;

namespace TennisMatchmaker.Data
{
    public class TennisDbContext(DbContextOptions<TennisDbContext> options) 
        : DbContext(options)
    {
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupMembership> GroupMemberships => Set<GroupMembership>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<SessionPlayer> SessionPlayers => Set<SessionPlayer>();
        public DbSet<Round> Rounds => Set<Round>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<PairingHistory> PairingHistories => Set<PairingHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for the Player <-> Group join table
            modelBuilder.Entity<GroupMembership>()
                .HasKey(gm => new { gm.PlayerId, gm.GroupId });
            
            modelBuilder.Entity<GroupMembership>()
                .HasOne(gm => gm.Player)
                .WithMany(p => p.GroupMemberships)
                .HasForeignKey(gm => gm.PlayerId);

            modelBuilder.Entity<GroupMembership>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.PlayerMemberships)
                .HasForeignKey(gm => gm.GroupId);

            // Composite key for the Session <-> Player join table
            modelBuilder.Entity<SessionPlayer>()
                .HasKey(sp => new { sp.SessionId, sp.PlayerId });

            modelBuilder.Entity<SessionPlayer>()
                .HasOne(sp => sp.Session)
                .WithMany(s => s.SelectedPlayers)
                .HasForeignKey(sp => sp.SessionId);
            
            modelBuilder.Entity<SessionPlayer>()
                .HasOne(sp => sp.Player)
                .WithMany()
                .HasForeignKey(sp => sp.PlayerId);

            // One row per unordered player pair - enforce uniqueness 
            modelBuilder.Entity<PairingHistory>()
                .HasIndex(ph => new { ph.PlayerAId, ph.PlayerBId })
                .IsUnique();
            
            modelBuilder.Entity<Player>()
                .Property(p => p.SkillLevel)
                .HasPrecision(3, 1);

        }
    }
}