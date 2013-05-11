using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HelperLib;

namespace Uservoice_autovote
{
    internal class Program
    {
        //Global Variables
        private static uint _counter;
        private static Settings _settings;

        /// <summary>
        ///     The entry-point with an infinite loop.
        /// </summary>
        private static void Main(string[] args)
        {
            _settings = new Settings(Int32.Parse(args[0]), args[1], args[2], args[3]);
            var proxies = IO.ReadFileIntoList("proxy.txt");

            Parallel.ForEach(proxies, delegate(string s)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vote(s);
                }
            });
        }

        /// <summary>
        ///     Automates votes on Uservoice.com.
        /// </summary>
        private static void Vote(string s)
        {
            #region Vote
            //Get the identifiers
            var cookies = new CookieContainer();
            var request = (HttpWebRequest)WebRequest.Create(_settings.UrlOfTheVote);
            request.Proxy = new WebProxy(s);
            request.CookieContainer = cookies;

            string csrfToken;

            using (WebResponse response = request.GetResponse())
            {
                var regex = new Regex("<meta name=\"csrf-token\" content=\"(.*)\"/>");
                csrfToken = regex.Match((new StreamReader(response.GetResponseStream())).ReadToEnd()).Groups[1].Value;
            }

            //Authenticate the session
            request = (HttpWebRequest)WebRequest.Create(_settings.UrlOfTheSessions);
            request.CookieContainer = cookies;
            var random = new Random();
            string data = string.Format("site2=1&forum_id={0}&display_name=str{1}&email={1}%40hotmail.com",
                                        _settings.SuggestionNumber, random.Next(100000, 999999));
            byte[] encodedData = Encoding.UTF8.GetBytes(data);

            request.Method = "POST";
            request.ContentLength = encodedData.Length;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Headers.Add("X-CSRF-Token", csrfToken);

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(encodedData, 0, encodedData.Length);
            }

            request.GetResponse().Dispose();


            //Submit the vote
            request = (HttpWebRequest)WebRequest.Create(_settings.UrlOfTheAjaxRequest);
            request.CookieContainer = cookies;
            data = "to=3&oauth_signature_method=HMAC-SHA1&oauth_consumer_key=1aEpNly5pjcYOpC7L4FXag";
            encodedData = Encoding.UTF8.GetBytes(data);

            request.Method = "POST";
            request.ContentLength = encodedData.Length;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Headers.Add("X-CSRF-Token", csrfToken);

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(encodedData, 0, encodedData.Length);
            }

            request.GetResponse().Dispose();
            #endregion

            _counter++;
            Console.Write("Vote #{0} in done", _counter);
        }

        /// <summary>
        ///     Settings struct
        /// </summary>
        internal struct Settings
        {
            internal Settings(int suggestionNumber, string urlOfTheAjaxRequest, string urlOfTheSessions, string urlOfTheVote)
            {
                SuggestionNumber = suggestionNumber;
                UrlOfTheAjaxRequest = urlOfTheAjaxRequest;
                UrlOfTheSessions = urlOfTheSessions;
                UrlOfTheVote = urlOfTheVote;
            }

            /// <summary>
            ///     The number of the suggestion (first number in the url above)
            /// </summary>
            internal readonly int SuggestionNumber;

            /// <summary>
            ///     Url used by Uservoice to increase the number of vote (just substitute the numbers from the previous url)
            /// </summary>
            internal readonly string UrlOfTheAjaxRequest;

            /// <summary>
            ///     Url for managing sessions
            /// </summary>
            internal readonly string UrlOfTheSessions;

            /// <summary>
            ///     Url corresponding to the public page for voting
            /// </summary>
            internal readonly string UrlOfTheVote;
        }
    }
}