using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Consul;
using Microsoft.Extensions.Options;
using System.Linq;

namespace ConsulBase6
{
    // consul服务注册扩展类
    public static class ConsulRegistrationExtensions
    {
        public static void AddConsul(this IServiceCollection service)
        {
            // 读取服务配置文件
            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            //Action<ConsulServiceOptions> consuloption =  config.GetSection("ConsulServiceOptions") as Action<ConsulServiceOptions>;
            service.Configure<ConsulServiceOptions>(options => {
                options.ConsulAddress = config.GetSection("ConsulServiceOptions:ConsulAddress").Value;
                options.ServiceName = config.GetSection("ConsulServiceOptions:ServiceName").Value;
                options.HealthCheck = config.GetSection("ConsulServiceOptions:HealthCheck").Value;
                options.ServiceAddress = config.GetSection("ConsulServiceOptions:ServiceAddress").Value;
            });
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            // 获取主机生命周期管理接口
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            // 获取服务配置项
            var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;

            // 服务ID必须保证唯一
            serviceOptions.ServiceId = Guid.NewGuid().ToString();

            var consulClient = new ConsulClient(configuration =>
            {
                //服务注册的地址，集群中任意一个地址
                configuration.Address = new Uri(serviceOptions.ConsulAddress);
            });

            // 获取当前服务地址和端口，配置方式
            var uri = new Uri(serviceOptions.ServiceAddress);

            // 节点服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = serviceOptions.ServiceId,
                Name = serviceOptions.ServiceName,// 服务名
                Address = uri.Host,
                Port = uri.Port, // 服务端口
                Check = new AgentServiceCheck
                {
                    // 注册超时
                    Timeout = TimeSpan.FromSeconds(5),
                    // 服务停止多久后注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 健康检查地址
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceOptions.HealthCheck}",
                    // 健康检查时间间隔
                    Interval = TimeSpan.FromSeconds(10),
                }
            };
            var agentServices = consulClient.Agent.Services().Result.Response;
            bool isExit = agentServices.Where(s => s.Value.Address.Equals(uri.Host) && s.Value.Port == uri.Port && s.Value.Service.Equals(serviceOptions.ServiceName)).Any();
            if (!isExit)
            {
                // 注册服务
                consulClient.Agent.ServiceRegister(registration).Wait();
            }
            // 应用程序终止时，注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceOptions.ServiceId).Wait();
            });

            return app;
        }
    }
}
