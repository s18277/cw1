using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cw1 {

    internal static class HttpEmailCrawler {

        private const string EmailPattern = "[\\w-.]+@[\\w-.]+";

        static void Main(string[] args) {
            foreach (var url in args) {
                ShowAllEmailsInUrl(url);
            }
        }

        private static void ShowAllEmailsInUrl(string url) {
            Console.WriteLine($"Looking for email addresses in {url}:");
            var httpGetResponse = GetResponseFromUrl(url).Result;
            var emails = GetEmailsFromResponse(httpGetResponse);
            foreach (var email in emails) {
                Console.WriteLine(email);
            }
            Console.WriteLine();
        }

        private static async Task<string> GetResponseFromUrl(string url) {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        private static MatchCollection GetEmailsFromResponse(string httpGetResponse) {
            var emailRegex = new Regex(EmailPattern);
            return emailRegex.Matches(httpGetResponse);
        }

    }

}