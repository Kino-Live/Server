namespace ProjectCinema.Settings
{
    public class PasswordResetOptions
    {
        public int TokenTTLMinutes { get; set; } = 30;
        public string DefaultRedirectUrl { get; set; } = string.Empty;
        public string[] AllowedRedirectHosts { get; set; } = new string[0];
    }
}


