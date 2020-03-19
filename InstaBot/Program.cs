using System;
using System.Threading;
using InstaSharper;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;

namespace InstaBot
{
    class Program
    {


        private static UserSessionData user;
        private static IInstaApi api;
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! \nPlease enter username");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter password:");
            string pass = GetPassword();

            Login(username,pass);

            Thread.Sleep(1000);

            Console.ReadLine();


        }

        public static string GetPassword()
        {
            string pass = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);
            return pass;
        }
        public static async void Login(string username,string password)
        {
            user = new UserSessionData();
            user.UserName = username;
            user.Password = password;
            var delay = RequestDelay.FromSeconds(1, 2);
            api = InstaApiBuilder.CreateBuilder().SetUser(user).UseLogger(new DebugLogger(LogLevel.Exceptions))
                .SetRequestDelay(delay)
                .Build();
            Console.WriteLine(" \nPlease wait...");

            var loginresult = await api.LoginAsync();
            if (loginresult.Succeeded)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                //Console.Clear();
                Console.Beep(800, 200);
                Console.Title =username;
                Console.WriteLine("\nLoggined");
            }
            else
                Console.WriteLine("error: \n" + loginresult.Info.Message);

        }

        public static async void FollowerCount()
        {

        }

    }
}
