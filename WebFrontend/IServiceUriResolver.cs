namespace Microsoft.Extensions.ServiceDiscovery
{
    public interface IServiceUriResolver
    {
        Uri Resolve(string serviceName);
    }
}
