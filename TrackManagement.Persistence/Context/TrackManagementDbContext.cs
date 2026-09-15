using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Domain.Entities;

namespace TrackManagement.Persistence.Context
{
    public class TrackManagementDbContext : DbContext
    {
        public TrackManagementDbContext(DbContextOptions<TrackManagementDbContext> options) : base(options) { }

        public DbSet<Artist> Artists => Set<Artist>();

        public DbSet<Track> Tracks => Set<Track>();

        public DbSet<Dsp> Dsps => Set<Dsp>();

        public DbSet<TrackDistribution> TrackDistributions => Set<TrackDistribution>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrackManagementDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
