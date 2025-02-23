using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Infrastructure.EventBus
{
    public interface IEventHandler<T>
    {
        Task Handle(T @event);
    }
}

