namespace examination_system.Migrations
{
    using examination_system.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Xml.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<examination_system.Models.DB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(examination_system.Models.DB db)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            IdentityRole role = new IdentityRole
            {
                Name = "superadmin",
                Id = "superadmin",
            };
            db.Roles.AddOrUpdate(role);
            role = new IdentityRole
            {
                Name = "admin",
                Id = "admin",
            };
            db.Roles.AddOrUpdate(role);
            role = new IdentityRole
            {
                Name = "student",
                Id = "student",
            };
            db.Roles.AddOrUpdate(role);
            role = new IdentityRole
            {
                Name = "professor",
                Id = "professor",
            };
            db.Roles.AddOrUpdate(role);
            db.SaveChanges();
            db=new DB();
            AspNetUsers NewUser = new AspNetUsers { Id = "20acd432-a32f-4a6c-bc1f-cd16e4976581", UserType = UserType.admin, ImgFileName = "20acd432-a32f-4a6c-bc1f-cd16e4976581.jpg", ConfirmPassword = "AH9l6fLCliOf+96CHUgcx5vYNWwGUvuW/U6udEg8UpvWCj6FUVzWBWJSorcGMzOF0w==", PasswordHash = "AH9l6fLCliOf+96CHUgcx5vYNWwGUvuW/U6udEg8UpvWCj6FUVzWBWJSorcGMzOF0w==", Email = "ayymanmohamed32@gmail.com", SecurityStamp = "a6f1c78b-8068-4cff-aebd-f5a41acdad3c", UserName = "mohamed ayman aly" };
            db.Users.AddOrUpdate(NewUser);
            UserStore<AspNetUsers> UserStore = new UserStore<AspNetUsers>(db);
            UserManager<AspNetUsers> userManager = new UserManager<AspNetUsers>(UserStore);
            userManager.AddToRole(NewUser.Id, "superadmin");
            db.SaveChanges();
        }
    }
}
