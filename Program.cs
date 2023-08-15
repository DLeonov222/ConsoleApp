using ConsoleTables;
using Faker;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    public class Program
    {
        enum Gender
        {
            Male,
            Female
        }
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "1":
                    CreateDb();
                    break;
                case "2":
                    CreateUser(args);
                    break;
                case "3":
                    PrintUnique();
                    break;
                case "4":
                    UserGenerator(1000000);
                    UserGenerator(100,"F");
                    break;
                case "5":
                    PrintUsers("F", Gender.Male);
                    break;
                case "6":
                    AddIndexes();
                    break;
                default:
                    Console.WriteLine("Неверный параметр");
                    break;
            }
        }

        private static void AddIndexes()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Database.ExecuteSqlRaw(@"
                            CREATE INDEX idx_gender ON Users (Gender);
                            CREATE INDEX idx_fullname ON Users (FullName);
                ");
                db.SaveChanges();
            }
        }

        static void PrintData(List<User> users)
        {
            var table = new ConsoleTable("ФИО", "Дата рождения", "Пол", "Кол-во полных лет");
            foreach (var user in users)
            {
                var diff = (DateTime.Now - user.BirthDate).TotalDays;
                table.AddRow(user.FullName,
                    user.BirthDate.ToString("d"),
                    user.Gender,
                    Math.Truncate(diff/365.25));
            }
            table.Write();
        }

        static void CreateDb()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                bool isCreated = db.Database.EnsureCreated();
                if (isCreated) Console.WriteLine("База данных была создана");
                else Console.WriteLine("База данных уже существует");
            }
        }

        static void CreateUser(string[] args)
        {
            
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    User user = new User();
                    user.FullName = args[1];
                    user.BirthDate =
                        DateTime.ParseExact(args[2], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    if (args[3].ToUpper().Equals("F") || args[3].ToUpper().Equals("Ж"))
                    {
                        user.Gender = Gender.Female.ToString();
                    }
                    else
                    {
                        user.Gender = Gender.Male.ToString();
                    }
                    db.Users.Add(user);
                }
                catch (IndexOutOfRangeException exception)
                {
                    Console.WriteLine("Запись не добавлена");
                }
                finally
                {
                    db.SaveChanges();
                }
            }
        }

        static void PrintUnique()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users
                    .GroupBy(u => new {u.FullName,u.BirthDate,u.Gender})
                    .Select(g => new User(){ FullName = g.Key.FullName, BirthDate = g.Key.BirthDate, Gender = g.Key.Gender})
                    .OrderBy(p => p.FullName)
                    .ToList();
                PrintData(users);
            }
        }

        static void PrintUsers(string firstLetter, Gender gender)
        {
            Stopwatch stopwatch = new Stopwatch();
            using (ApplicationContext db = new ApplicationContext())
            {
                stopwatch.Start();
                var users = db.Users
                    //.AsEnumerable()
                    .Where(u => u.FullName.StartsWith(firstLetter)&& u.Gender.Equals(gender.ToString()))
                    .ToList();
                stopwatch.Stop();
                PrintData(users);
            }
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        static void UserGenerator(int count)
        {
            List<User> users = new List<User>();
            Console.WriteLine("Генерация данных");
            for (int i = 0; i < count; i++)
            {
                users.Add(new User()
                {
                    FullName = Faker.Name.FullName(NameFormats.StandardWithMiddle),
                    BirthDate = Faker.Identification.DateOfBirth(),
                    Gender = Faker.Enum.Random<Gender>().ToString()
                });
                Console.WriteLine(i);
            }
            Console.WriteLine("Запись данных в бд");
            using (ApplicationContext db = new ApplicationContext())
            {
                db.AddRange(users);
                db.SaveChanges();
            }
        }

        static void UserGenerator(int count, string firstLetter)
        {
            List<User> users = new List<User>();
            Console.WriteLine("Генерация данных");
            while (users.Count < count)
            {
                string fullName = Faker.Name.FullName(NameFormats.StandardWithMiddle);
                if (!fullName.StartsWith(firstLetter))
                {
                    continue;
                }
                else
                {
                    Console.WriteLine(users.Count);
                    users.Add(new User()
                    {
                        FullName = fullName,
                        BirthDate = Faker.Identification.DateOfBirth(),
                        Gender = Gender.Male.ToString()
                    });
                }
            }
            Console.WriteLine("Запись данных в бд");
            using (ApplicationContext db = new ApplicationContext())
            {
                db.AddRange(users);
                db.SaveChanges();
            }
        }

    }
}