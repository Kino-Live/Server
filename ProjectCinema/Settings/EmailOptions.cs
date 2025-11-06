namespace ProjectCinema.Settings
{
    public class EmailOptions
    {
        public string Provider { get; set; } = "Smtp";
        public string From { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // override via env: Email__Password
    }
}


