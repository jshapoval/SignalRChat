using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Data.DTO;
using MongoDB.Bson;
using MongoDB.Driver;
using File = ChatServerSignalRWithIdentity.Models.File;
using MongoDB.Driver.GridFS;

namespace ChatServerSignalRWithIdentity.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<AppUser> AspNetUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<UserRelationship> UserRelationships { get; set; }

        public DbSet<Dialog> Dialogs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Message>()
                .HasOne<AppUser>(a => a.Sender)
                .WithMany(d => d.Messages)
                .HasForeignKey(d => d.SenderId);


            builder.Entity<Participant>().HasKey("DialogId", "AppUserId");

         
         //   builder.Entity<AppUser>().HasMany(x => x.Relationships);
        }
        
    }
}
