namespace ApiRRHH.Web.Middleware
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string TraceIdentifier { get; set; }
        public string BodyRequest { get; set; }
        public string HeadersRequest { get; set; }
        public string HeadersResponse { get; set; }
        public string BodyResponse { get; set; }
        public string RemoteIP { get; set; }
        public int? RemotePort { get; set; }
        public string Scheme { get; set; }
        public string ApplicationName { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public int StatusCode { get; set; }
        public double Duration { get; set; }
        public string ContentType { get; set; }
        public string UserAgent { get; set; }
        public string MachineName { get; set; }
    }
}
