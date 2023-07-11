namespace Account.Api.Setups
{
    public class ServiceSettings
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceHost { get; set; } = string.Empty;
        public int ServicePort { get; set; }
        public string ServiceDiscoveryAddress { get; set; } = string.Empty;
    }
}
