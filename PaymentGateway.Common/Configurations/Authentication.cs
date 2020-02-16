namespace PaymentGateway.Common.Configurations
{
    public class Authentication
    {
        public bool RequireHttpsMetadata { get; set; }
        public string Authority { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public string ClientId { get; set; }
    }
}
