using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Infrastructure.EventBus
{
    public interface IEventBus {
        Task PublishAsync<T>(T @event);
        Task SubscribeAsync<T, TH>() where T : class where TH : IEventHandler<T>;
    }
}
