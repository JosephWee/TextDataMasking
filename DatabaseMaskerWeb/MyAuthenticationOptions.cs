using Microsoft.AspNetCore.Authentication;

namespace DatabaseMaskerWeb
{
    public class MyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "MyAuthenticationScheme";
        public string TokenHeaderName { get; set; } = "MyBearerToken";
    }
}
