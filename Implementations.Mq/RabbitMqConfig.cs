using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations.Mq
{
    public record RabbitMqConfig(
        string Host,
        string Username,
        string Password,
        string VirtualHost);
}
