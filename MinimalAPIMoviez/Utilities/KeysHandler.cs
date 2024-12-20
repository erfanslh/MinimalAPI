using Microsoft.IdentityModel.Tokens;

namespace MinimalAPIMoviez.Utilities
{
    public class KeysHandler
    {
        public const string OurIssuer = "our-app";

        private const string KeysSection = "Authentication:Schemes:Bearer:SigningKeys";
        private const string KeysSection_Issuer = "Issuer";
        private const string KeysSection_Value = "Value";

        public static IEnumerable<SecurityKey> GetKey(IConfiguration configuration)
                => GetKey(configuration, OurIssuer);

        public static IEnumerable<SecurityKey> GetKey(IConfiguration configuration, string issuer)
        {
            // This get the specific element from Enumerable in Secret.json file
            var signinKey = configuration.GetSection(KeysSection)
                .GetChildren()
                .SingleOrDefault(key => key[KeysSection_Issuer]== issuer);

            if (signinKey is not null && signinKey[KeysSection_Value] is string secretKey)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
            }

        }
        // Get All Keys, to Validate token against mutiple sign in keys
        public static IEnumerable<SecurityKey> GetAllKeys(IConfiguration configuration)
        {
            var signinKeys = configuration.GetSection(KeysSection).GetChildren();
            foreach ( var key in signinKeys )
            {
                if (key[KeysSection_Value] is string secretKey)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
                }
            }
        }
    }
}
