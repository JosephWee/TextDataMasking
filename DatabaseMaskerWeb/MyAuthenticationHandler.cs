using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DatabaseMaskerWeb
{
    public class MyAuthenticationHandler :
        AuthenticationHandler<MyAuthenticationOptions>
    {
        public MyAuthenticationHandler(
            IOptionsMonitor<MyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
            //{
            //    string message = $"Missing header: {Options.TokenHeaderName}";
            //    return AuthenticateResult.Fail(message);
            //}

            //string token =
            //    Request.Headers[Options.TokenHeaderName]!;

            //byte[] expectedBytes =
            //    System.Text.Encoding.ASCII.GetBytes("my custom token");

            //if (token != Convert.ToBase64String(expectedBytes))
            //{
            //    string message = $"Invalid token.";
            //    return AuthenticateResult.Fail(message);
            //}

            //var headerKeys = this.Request.Headers.Keys.ToList();
            //for (int i = 0; i < this.Request.Headers.Count; i++)
            //{
            //    var key = headerKeys.ElementAt(i);
            //    var val = this.Request.Headers[key];
            //}

            //var a = await this.Request.BodyReader.ReadAsync();
            //byte[] bytes = a.Buffer.ToArray();
            //string bodyText = System.Text.Encoding.UTF8.GetString(bytes);

            var claims = new List<Claim>()
            {
                new Claim("UserIdentifier", "SomeIdentifier")
            };

            var claimsIdentity =
                new ClaimsIdentity(claims, this.Scheme.Name);

            var claimsPrincipal =
                new ClaimsPrincipal(claimsIdentity);

            return AuthenticateResult.Success(
                new AuthenticationTicket(
                    claimsPrincipal,
                    this.Scheme.Name)
                );
        }
    }
}
