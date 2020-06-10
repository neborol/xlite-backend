using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using EliteForce.Data;
using EliteForce.Entities;
using Microsoft.EntityFrameworkCore;

namespace EliteForce.Seeds
{
    public class DbInitializer
    {
        public static void Initialize(EliteDataContext context)
        {
            context.Database.EnsureCreated();
            // Don't put any data seeds in these tables if there is some data in them already
            string path = AppContext.BaseDirectory; // Get the root directory
            string[] ss = Regex.Split(path, "bin"); // Split the path at the bin keyword
            string filePath = ss[0] + @"Seeds\"; // Append the Seeds folder to the rood dir path


            if (!context.Codes.Any())
            {
                var codesList = SeedingHelper.CsvToCodes(filePath + "Codes.csv");
                foreach (Code c in codesList)
                {
                    context.Codes.Add(c);
                }

                context.SaveChanges();
            }


            if (!context.FaqItems.Any())
            {
                var faqList = SeedingHelper.CsvToFaq(filePath + "Faq.csv");
                foreach (Faq f in faqList)
                {
                    context.FaqItems.Add(f);
                }

                context.SaveChanges();
            }


            //if (!context.Members.Any())
            //{
            //    var membersList = SeedingHelper.CsvToMember(filePath + "Members.csv");
            //    foreach (Member m in membersList)
            //    {
            //        context.Members.Add(m);
            //    }

            //    context.SaveChanges();
            //}


            //if (!context.Subscriptions.Any())
            //{
            //    var subscriptionsList = SeedingHelper.CsvToSubscription(filePath + "Subscriptions.csv");
            //    foreach (Subscription s in subscriptionsList)
            //    {
            //        context.Subscriptions.Add(s);
            //    }

            //    context.SaveChanges();
            //}


            //if (!context.Users.Any())
            //{
            //    var usersList = SeedingHelper.CsvToUser(filePath + "Users.csv");
            //    foreach (User u in usersList)
            //    {
            //        context.Users.Add(u);
            //    }

            //    context.SaveChanges();
            //}
        }
    }
}


