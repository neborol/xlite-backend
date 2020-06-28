using Microsoft.EntityFrameworkCore;
using EliteForce.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EliteForce.Data
{
    public class EliteDataContext : IdentityDbContext<User>
    {
        public EliteDataContext(DbContextOptions options) : base(options)
        {
        }

        // public DbSet<User> Users { get; set; }
        // public DbSet<Member> Members { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MissionPhoto> MissionPhotos { get; set; }
        public DbSet<MissionVideo> MissionVideos { get; set; }
        public DbSet<Code> Codes { get; set; }

        public DbSet<Faq> FaqItems { get; set; }
        public DbSet<News> NewsItems { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<About> AboutStatements { get; set; }
        public DbSet<Home> HomeStatements { get; set; }
        public DbSet<EventObj> EventItems { get; set; }
        public DbSet<ScrollingNews> ScrollingNewsItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.Entity<Subscription>()
            //    .HasOne<User>(s => s.User)
            //    .WithMany(m => m.Subscriptions)
            //    .HasForeignKey(s => s.UserId);

            //builder.Entity<Subscription>()
            //    .Property(s => s.SubscriptionId)
            //    .ValueGeneratedNever();


            builder.Entity<User>()
                .Property(t => t.FirstName)
                .HasMaxLength(60);

            //builder.Entity<Subscription>()
            //    .Property(s => s.Member)
            //    .IsRequired();


            //builder.Entity<User>()
            //    .Property(u => u.UserId)
            //    .ValueGeneratedNever();

            //builder.Entity<MissionPhoto>()
            //    .Property(m => m.MissionPhotoId)
            //    .ValueGeneratedNever();

            base.OnModelCreating(builder);
        }
    }
}

