using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InstaSharp;
using InstaSharp.Models.Responses;
using Newtonsoft.Json;

namespace InfluencerAlert.Services
{
    /// <summary>
    /// OAuth authentication
    /// </summary>
    public class NewOAuth
    {
        private readonly InstagramConfig config;

        /// <summary>
        /// Response Type
        /// </summary>
        public enum ResponseType
        {
            /// <summary>
            /// The code
            /// </summary>
            Code,
            /// <summary>
            /// The token
            /// </summary>
            Token
        }

        /// <summary>
        /// Scope
        /// </summary>
        public enum Scope
        {
            /// <summary>
            /// basic
            /// </summary>
            Basic,
            /// <summary>
            /// public_content
            /// </summary>
            Public_Content,
            /// <summary>
            /// follower_list
            /// </summary>
            Follower_List,
            /// <summary>
            /// comments
            /// </summary>
            Comments,
            /// <summary>
            /// relationships
            /// </summary>
            Relationships,
            /// <summary>
            /// likes
            /// </summary>
            Likes
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public NewOAuth(InstagramConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Authentications the link.
        /// </summary>
        /// <param name="instagramOAuthUri">The instagram o authentication URI.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="callbackUri">The callback URI.</param>
        /// <param name="scopes">The scopes.</param>
        /// <param name="responseType">Type of the response.</param>
        /// <returns>The authentication url</returns>
        public static string AuthLink(string instagramOAuthUri, string clientId, string callbackUri, List<Scope> scopes, ResponseType responseType = ResponseType.Token)
        {
            var scopesForUri = BuildScopeForUri(scopes);
            return BuildAuthUri(instagramOAuthUri, clientId, callbackUri, responseType, scopesForUri);
        }

        /// <summary>
        /// Get authentication link.
        /// </summary>
        /// <param name="config">Instagram configuration.</param>
        /// <param name="scopes">The scopes.</param>
        /// <param name="responseType">Type of the response.</param>
        /// <returns>The authentication url</returns>
        public static string AuthLink(InstagramConfig config, List<Scope> scopes, ResponseType responseType = ResponseType.Token)
        {
            var scopesForUri = BuildScopeForUri(scopes);
            return BuildAuthUri(config.OAuthUri.TrimEnd('/') + "/authorize",
                config.ClientId,
                config.RedirectUri,
                responseType,
                scopesForUri);
        }

        /// <summary>
        /// Requests the token.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>An OAuthResponse object</returns>
        public Task<OAuthResponse> RequestToken(string code)
        {
            var client = new HttpClient { BaseAddress = new Uri(config.OAuthUri) };
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(client.BaseAddress, "access_token"));
            //HttpClient client = new HttpClient();
            //var request = new HttpRequestMessage(HttpMethod.Post, "https://api.instagram.com/oauth/access_token");
            var myParameters = string.Format("client_id={0}&client_secret={1}&grant_type={2}&redirect_uri={3}&code={4}",
                config.ClientId.UrlEncode(),
                config.ClientSecret.UrlEncode(),
                "authorization_code".UrlEncode(),
                config.RedirectUri.UrlEncode(),
                code.UrlEncode());

            request.Content = new StringContent(myParameters, Encoding.UTF8, "application/x-www-form-urlencoded");

            return client.ExecuteAsync<OAuthResponse>(request);

        }

        /// <summary>
        /// Build scope string for auth uri.
        /// </summary>
        /// <param name="scopes">List of scopes.</param>
        /// <returns>Comma separated scopes.</returns>
        private static string BuildScopeForUri(List<Scope> scopes)
        {
            var scope = new StringBuilder();

            foreach (var s in scopes)
            {
                if (scope.Length > 0)
                {
                    scope.Append("+");
                }
                scope.Append(s);
            }

            return scope.ToString();
        }

        /// <summary>
        /// Build authentication link.
        /// </summary>
        /// <param name="instagramOAuthUri">The instagram o authentication URI.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="callbackUri">The callback URI.</param>
        /// <param name="scopes">The scopes.</param>
        /// <param name="responseType">Type of the response.</param>
        /// <returns>The authentication uri</returns>
        private static string BuildAuthUri(string instagramOAuthUri, string clientId, string callbackUri, ResponseType responseType, string scopes)
        {
            return string.Format("{0}?client_id={1}&redirect_uri={2}&response_type={3}&scope={4}", new object[] {
                instagramOAuthUri.ToLower(),
                clientId.ToLower(),
                callbackUri,
                responseType.ToString().ToLower(),
                scopes.ToLower()
            });
        }
    }
    
        internal static class StringExtensions
        {
            public static string UrlEncode(this string input)
            {
                return Uri.EscapeDataString(input);
            }

            public static bool ContainsWhiteSpace(this string str)
            {
                var regEx = new Regex("\\s");
                var match = regEx.Match(str);
                return match.Success;
            }
        }

    internal static class HttpRequestMessageExtensions
    {
        public static void AddParameter(this HttpRequestMessage request, string key, IFormattable value)
        {
            if (value != null)
            {
                request.AddParameter(key, value.ToString(null, CultureInfo.InvariantCulture));
            }
        }
        public static void AddParameter(this HttpRequestMessage request, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            var uriBuilder = new UriBuilder(request.RequestUri);
            var queryToAppend = key.UrlEncode() + "=" + value.UrlEncode();
            uriBuilder.Query = uriBuilder.Query.Length > 1 ? uriBuilder.Query.Substring(1) + "&" + queryToAppend
                                                          : queryToAppend;

            request.RequestUri = uriBuilder.Uri;
        }

        public static void AddUrlSegment(this HttpRequestMessage request, string key, string value)
        {
            var uriBuilder = new UriBuilder(request.RequestUri);
            uriBuilder.Path = uriBuilder.Path.Replace("%7B" + key + "%7D", Uri.EscapeUriString(value));
            request.RequestUri = uriBuilder.Uri;
        }
    }
    internal static class HttpClientExtensions
    {
        private const string RateLimitRemainingHeader = "X-Ratelimit-Remaining";
        private const string RateLimitHeader = "X-Ratelimit-Limit";

        public static async Task<T> ExecuteAsync<T>(this HttpClient client, HttpRequestMessage request)
        {
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode == false && response.Content.Headers.ContentType.MediaType != "application/json")
            {
                response.EnsureSuccessStatusCode();
            }

            string resultData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(resultData);

            var endpointResponse = result as Response;

            if (endpointResponse != null)
            {
                if (response.Headers.Contains(RateLimitHeader))
                {
                    endpointResponse.RateLimitLimit =
                        response.Headers
                            .GetValues(RateLimitHeader)
                            .Select(int.Parse)
                            .SingleOrDefault();
                }

                if (response.Headers.Contains(RateLimitRemainingHeader))
                {
                    endpointResponse.RateLimitRemaining =
                        response.Headers
                            .GetValues(RateLimitRemainingHeader)
                            .Select(int.Parse)
                            .SingleOrDefault();
                }
            }

            return result;
        }
    }
}