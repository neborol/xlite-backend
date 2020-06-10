using Microsoft.EntityFrameworkCore;
using EliteForce.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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
        public DbSet<Code> Codes { get; set; }

        public DbSet<Faq> FaqItems { get; set; }
        public DbSet<News> NewsItems { get; set; }

        public DbSet<ScrollingNews> ScrollingNewsItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Subscription>()
                .HasOne<User>(s => s.User)
                .WithMany(m => m.Subscriptions)
                .HasForeignKey(s => s.UserId);

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




//namespace EliteForce.Data
//{
//    public class EliteDataContext : ApiAuthorizationDbContext<User>
//    {
//        public EliteDataContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
//        {
//        }

//        // public DbSet<User> Users { get; set; }
//        // public DbSet<Member> Members { get; set; }
//        public DbSet<Subscription> Subscriptions { get; set; }
//        public DbSet<MissionPhoto> MissionPhotos { get; set; }
//        public DbSet<Code> Codes { get; set; }

//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            builder.Entity<Subscription>()
//                .HasOne<User>(s => s.User)
//                .WithMany(m => m.Subscriptions)
//                .HasForeignKey(s => s.UserId);

//            //builder.Entity<Subscription>()
//            //    .Property(s => s.SubscriptionId)
//            //    .ValueGeneratedNever();


//            builder.Entity<User>()
//                .Property(t => t.FirstName)
//                .HasMaxLength(60);


//            //builder.Entity<Subscription>()
//            //    .Property(s => s.Member)
//            //    .IsRequired();


//            //builder.Entity<User>()
//            //    .Property(u => u.UserId)
//            //    .ValueGeneratedNever();

//            //builder.Entity<MissionPhoto>()
//            //    .Property(m => m.MissionPhotoId)
//            //    .ValueGeneratedNever();


//            base.OnModelCreating(builder);
//        }
//    }
//}