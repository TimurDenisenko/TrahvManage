using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrahvManage.Models;


namespace TrahvManage.Services
{
    public static class AccidentGenerator
    {
        private static Random random = new Random();
        private static readonly string[] places = { "Kesklinn", "Lasnamäe", "Mustamäe", "Nõmme", "Kristiine" };
        private static readonly string[] gifUrls =
        {
            "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExbHYyMjY5cGoyc285eGNlaTh1czlrZWk1dWhwaTMyNWIzMmJ3dXp5ciZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/xUNemSRIvn4VOJRuXm/giphy.webp",
            "https://i.giphy.com/kIraBtE4Pq9gBZCG57.webp",
            "https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExMmdvNnpjNTNmbGF4aGxjc2o0NDgzMDF0dXk3dnV5cHh3ZjBsN3psdyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/noXBLqrUi6ub22xXcZ/giphy.webp",
            "https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExczllMzhoczducnQ4eDJ5eGZqdnRlMG5tNDd4cmpodG9nMjI2bXN3bSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/3oriNMP8zgmok4iUZG/giphy.webp",
            "https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExbHJ4aDNxN2w1NzkyaGYzaXYyd241aHl5ZG52eGcwMmc4b2N0YzZyZCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/Qz8rqIlzOtUx0kCpke/giphy.webp",
            "https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExOHV5bXd4ZTZkaW91azV3YTJ3amZ2M292MzZ2MWYyNWV1dXhoMjUxaSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/SwuP4hyvh8vXr9dy83/giphy.webp",
            "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExZW11bTE1dzFmNXM1eG8zdzlqNWU5MDk2bG1iZTRrZzNlOGk2aDI4ayZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/s7LrWWLeWLumc/giphy.webp",
            "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExZDcyanpocHZ4ZncycXp0eWtobzJkZjNscGNpbTJxYWc3dGgyaXVmNSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/ZFhiqsauQe9ierrdHU/giphy.webp",
            "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExczZmN3FkM3Mwa2VtOTk3ajV3YzIyMHVzZnNsZmlobGYwdzV5czUxeCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/6vHQVvI6QENag/giphy.webp",
            "https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExaXMzMHIwdHNjcmpjeDJpY3owNHIzMms5anllbzBsanI0c2Z6ejZnMCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/hLhM83i3eUezES4tlC/giphy.webp",
        };
        private static readonly string[] incidents =
        {
            "kellegi teise sõiduki kahjustamine",
            "kiiruse ületamine",
            "joobes juhtimine",
            "juhiloata juht",
            "sõitmine vales kohas"
        };
        public static void LoadFine(TrahvContext db)
        {
            FineModel fine = GenerateRandomAccident(db);
            if (fine == null)
                return;
            db.Fines.Add(GenerateRandomAccident(db));
            db.SaveChanges();
        }
        private static FineModel GenerateRandomAccident(TrahvContext db)
        {
            if (db.Accounts.Count() < 2)
                return null;
            string pc = db.Accounts.Where(x => x.Role != "Admin").ToArray()[random.Next(db.Accounts.Count()-1)].PersonalCode;
            return new FineModel
            {
                GifUrl = gifUrls[random.Next(gifUrls.Length)],
                PersonalCode = pc,
                AutoNumber = GenerateRandomCarNumber(),
                Incident = incidents[random.Next(incidents.Length)],
                IncidentPlace = places[random.Next(places.Length)],
                IncidentDate = GenerateRandomDate().ToString(),
                FineAmount = (float)random.NextDouble() * 5000,
            };
        }

        private static DateTime GenerateRandomDate()
        {
            DateTime start = DateTime.Now.AddYears(-1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(random.Next(range)).AddHours(random.Next(24)).AddMinutes(random.Next(60));
        }

        private static string GenerateRandomCarNumber()
        {
            int digits = random.Next(100, 1000);

            string letters = $"{(char)random.Next('A', 'Z' + 1)}" +
                             $"{(char)random.Next('A', 'Z' + 1)}" +
                             $"{(char)random.Next('A', 'Z' + 1)}";

            return $"{digits} {letters}";
        }
    }
}