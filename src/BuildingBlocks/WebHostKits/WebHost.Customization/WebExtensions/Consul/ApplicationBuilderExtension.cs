using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Policy = Polly.Policy;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtension
{
    public static void RegisterService(this IApplicationBuilder app,
        IOptions<ServiceRegisterOptions> serviceRegisterOptions,
        IConsulClient consul,
        IHostApplicationLifetime lifetime)
    {
        var serviceId = $"{serviceRegisterOptions.Value.ServiceName}_{serviceRegisterOptions.Value.ServiceAddress.Host}:{serviceRegisterOptions.Value.ServiceAddress.Port}";

        var httpCheck = new AgentServiceCheck()
        {
            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),//服务启动多久后注册
            Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
            HTTP = $"http://{serviceRegisterOptions.Value.ServiceAddress.Host}:{serviceRegisterOptions.Value.ServiceAddress.Port}/api/health",//健康检查地址
        };

        var registration = new AgentServiceRegistration()
        {
            Checks = new[] { httpCheck },
            Address = serviceRegisterOptions.Value.ServiceAddress.Host,
            ID = serviceId,
            Name = serviceRegisterOptions.Value.ServiceName,
            Port = serviceRegisterOptions.Value.ServiceAddress.Port,
            Tags = new[] { $"urlprefix/{serviceRegisterOptions.Value.ServiceName}", $"hostprefix/{serviceRegisterOptions.Value.ServiceAddress.Host}" }
        };

        var retry =
            Policy.Handle<HttpRequestException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15)
                });

        retry.Execute(() => consul.Agent.ServiceRegister(registration).Wait());

        lifetime.ApplicationStopping.Register(() =>
        {
            consul.Agent.ServiceDeregister(serviceId).Wait();//服务停止时取消注册
        });
    }
}

public class ServiceRegisterOptions
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; }
    /// <summary>
    /// 服务IP或者域名
    /// </summary>
    public Uri ServiceAddress { get; set; }
    /// <summary>
    /// consul注册地址
    /// </summary>
    public RegisterOptions Register { get; set; }
}

public class RegisterOptions
{
    public Uri HttpEndpoint { get; set; }
}