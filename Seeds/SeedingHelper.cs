using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Seeds
{
    public class SeedingHelper
    {        
        public static List<Subscription> CsvToSubscription(string csvFile)
        {
            FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            List<String> lst = new List<string>();
            while (!sr.EndOfStream)
            {
                lst.Add(sr.ReadLine());
            }

            string[] fields = lst[0].Split(new char[] { ',' }); // Get the first row and extract the coma separated column names
            var res = new List<Subscription>();

            for (int i = 1; i < lst.Count; i++)
            {
                fields = lst[i].Split(',');
                res.Add(new Subscription
                {
                    SubscriptionId = Convert.ToInt32(fields[0]),
                    Name = fields[1],
                    Comment = fields[2],
                    DateEntered = DateTime.UtcNow,
                    Amount = Convert.ToInt32(fields[4]),
                    UserId = fields[5]
                });
            }

            return res;
        }


        public static List<User> CsvToUser(string csvFile)
        {
            FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            List<String> lst = new List<string>();
            while (!sr.EndOfStream)
            {
                lst.Add(sr.ReadLine());
            }

            string[] fields = lst[0].Split(new char[] { ',' }); // Get the first row and extract the coma separated column names
            var res = new List<User>();

            for (int i = 1; i < lst.Count; i++)
            {
                fields = lst[i].Split(',');
                res.Add(new User
                {
                    Id = fields[0],
                    UserName = fields[1],
                    PasswordHash = null,
                    //PasswordSalt = null,

                    CodeNr = null,
                    FirstName = fields[2],
                    LastName = fields[3],
                    Email = fields[4],
                    PhoneNumber = fields[5],
                    City = fields[6],
                    Status = fields[7]
                });
            }

            return res;
        }


        public static List<Code> CsvToCodes(string csvFile)
        {
            FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            List<String> lst = new List<string>();
            while (!sr.EndOfStream) lst.Add(sr.ReadLine());
            // string[] fields = lst[0].Split(new char[] { ',' });
            var res = new List<Code>();

            for (int i = 1; i < lst.Count; i++)
            {
                res.Add(new Code
                {
                    CodeNr = lst[i]
                });
            }

            return res;
        }



        public static List<Faq> CsvToFaq(string csvFile)
        {
            FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            List<String> lst = new List<string>();
            while (!sr.EndOfStream)
            {
                lst.Add(sr.ReadLine());
            }

            string[] fields = lst[0].Split(new char[] { ',' }); // Get the first row and extract the coma separated column names
            var res = new List<Faq>();

            for (int i = 1; i < lst.Count; i++)
            {
                fields = lst[i].Split(',');
                res.Add(new Faq
                {
                    FaqId = fields[0],
                    FaqQuestion = fields[1],
                    FaqAnswer = fields[2]
                });
            }

            return res;
        }

    }
}
