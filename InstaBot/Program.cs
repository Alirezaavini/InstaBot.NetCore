using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using InstaSharper;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using Newtonsoft.Json.Linq;

namespace InstaBot
{
    class Program
    {


        private static UserSessionData user;
        private static IInstaApi api;
        static void Main(string[] args)
        {
            MainMethod();
        }


        public static async void MainMethod()
        {
            Console.WriteLine("Welcome! \nPlease enter username");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter password:");
            string pass = GetPassword();

            Login(username, pass);

         
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
        public static async void Login(string username, string password)
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
                //Console.Clear();
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                Console.Beep(800, 200);
                Console.Title = username;
                Console.WriteLine("\nLoggined");
                AllMedia(username);
            }
            else
                Console.WriteLine("error: \n" + loginresult.Info.Message);

        }

        public static async void FollowerCount(string username)
        {
            var followers = await api.GetUserFollowersAsync(username,
                PaginationParameters.MaxPagesToLoad(5));
            Console.WriteLine("f = {0}", followers.Value.Count);
        }

        public static void FollowFollowers(string username)
        {
            var followers = api.GetUserFollowersAsync("alireza_avini",
                PaginationParameters.MaxPagesToLoad(5));            
           
            Console.WriteLine("followers count is: {0}", followers.Result.Value.Count);
            Console.WriteLine("please wait we are following user folowers");
            foreach (var item in followers.Result.Value)
            {
                string json = GetRequest("https://www.instagram.com/" + item.UserName + "/?__a=1");
                JObject rss = JObject.Parse(json);
                long userId = (long)rss["graphql"]["user"]["id"];
                Console.WriteLine(userId);
                api.FollowUserAsync(userId);
            }
            Console.WriteLine("done");
        }

        public static async void AllMedia(string username)
        {
            var mediaList = new InstaMediaList();
            var result = await api.GetUserMediaAsync(username, PaginationParameters.Empty);          

        }

        public static string GetRequest(string url)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                //httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var resultString = streamReader.ReadToEnd();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return resultString;
                    }
                    return "";

                }
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }

    }
}
