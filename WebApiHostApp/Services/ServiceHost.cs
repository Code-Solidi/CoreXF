using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

namespace CoreXF.WebApiHostApp.Services
{
    public class ServiceHost : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}