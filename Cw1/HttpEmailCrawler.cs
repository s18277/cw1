using System;
using System.Collections.Generic;
using System.Linq;
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

        private static void Main(string[] args) {
            VerifyProvidedArguments(args);
            args.ToList().ForEach(ShowAllEmailsInUrl);
        }

        private static void VerifyProvidedArguments(string[] args) {
            if (args.Length < 1) throw new ArgumentNullException(nameof(args), "Nie podano żadnego URL do spawdzenia!");
            args.ToList().ForEach(VerifyStringAsUrl);
        }

        private static void VerifyStringAsUrl(string url) {
            var isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                             && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!isValidUrl) throw new ArgumentException($"{url} nie jest poprawnym URL!");
        }

        private static void ShowAllEmailsInUrl(string url) {
            Console.WriteLine($"Szukam adresów e-mail w {url}:");
            try {
                var httpGetResponseTask = GetResponseFromUrl(url);
                HandleSuccessfulGetResponseTask(httpGetResponseTask.Result, url);
            }
            catch (AggregateException) {
                Console.WriteLine($"Błąd w czasie pobierania strony {url}!");
            }

            Console.WriteLine();
        }

        private static async Task<HttpResponseMessage> GetResponseFromUrl(string url) {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            httpClient.Dispose();
            return response;
        }

        private static async void HandleSuccessfulGetResponseTask(HttpResponseMessage httpGetResponse, string url) {
            if (httpGetResponse.IsSuccessStatusCode)
                await ShowAllEmailsFromHttpResponse(httpGetResponse);
            else
                Console.WriteLine($"Błąd w czasie pobierania strony {url} " +
                                  $"- kod: {(int) httpGetResponse.StatusCode} - {httpGetResponse.StatusCode}!");
        }

        private static async Task ShowAllEmailsFromHttpResponse(HttpResponseMessage httpGetResponse) {
            var emails = GetEmailsFromResponse(await httpGetResponse.Content.ReadAsStringAsync());
            if (emails.Count == 0)
                Console.WriteLine("Nie znaleziono adresów e-mail!");
            else
                emails.ToList().ForEach(Console.WriteLine);
        }

        private static List<string> GetEmailsFromResponse(string httpGetResponse) {
            return new Regex(EmailPattern).Matches(httpGetResponse).Select(match => match.Value).Distinct().ToList();
        }
    }
}