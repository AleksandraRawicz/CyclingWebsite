namespace CyclingWebsite
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public string JwtKeyEnc { get; set; }
        public string JwtIssuer { get; set; }
        public int ExpirationTime { get; set; }
    }
}