using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cw1 {

    internal static class HttpEmailCrawler {

        // RFC 5322 Official Standard e-mail regex
        private const string EmailPattern = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""" +
                                            @"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b" +
                                            @"\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9]" +
                                            @"(?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)" +
                                            @"\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[" +
                                            @"\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-" +
                                            @"\x7f])+)\])";

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