using Microsoft.IdentityModel.Tokens;

namespace MinimalAPIsWithASPNetEF.Utilities
{
    public class KeysHandler
    {
        public const string OurIssuer = "myapp-user-jwts"; // same name in user secrets
        private const string KeySection = "Authentication:Schemes:Bearer:SigningKeys"; // in secrets
        private const string KeySection_Issuer = "Issuer"; // same in secrets
        private const string KeySection_Value = "Value";

        public static IEnumerable<SecurityKey> GetKey(IConfiguration cfg) => GetKey(cfg, OurIssuer);

        public static IEnumerable<SecurityKey> GetKey(IConfiguration cfg, string issuer)
        {
            var signingKey = cfg.GetSection(KeySection)
                .GetChildren()
                .SingleOrDefault(key => key[KeySection_Issuer] == issuer);
            if (signingKey is not null && signingKey[KeySection_Value] is string secretKey)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(secretKey)); // get the key/value from the 
            }
        }

        public static IEnumerable<SecurityKey> GetAllKeys(IConfiguration cfg)
        {
            var signingKeys = cfg.GetSection(KeySection)
                .GetChildren();
            foreach (var signingKey in signingKeys)
            {
                if (signingKey[KeySection_Value] is string secretKey)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
                }
            }
        }
    }
}
