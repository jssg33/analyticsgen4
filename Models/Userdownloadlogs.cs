namespace Enterprise.Models
{
    public class UserDownloadLog
    {
        public int Id { get; set; }
        public int Uid { get; set; }

        public string? Username { get; set; }

        public string? UserEmail { get; set; }

        public string? FileName { get; set; }

        public string? FilePath { get; set; }

        public long FileSizeBytes { get; set; }

        public string? ContentType { get; set; }

        public string? SourceApplication { get; set; }

        public string? DownloadMethod { get; set; }

        public string? UserIPAddress { get; set; }

        public string? UserLocation { get; set; }

        public string? UserAgent { get; set; }

        public DateTime DownloadDateTime { get; set; }

        public bool Successful { get; set; }

        public string? FailureReason { get; set; }
    }
}