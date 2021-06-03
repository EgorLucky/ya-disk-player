using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations.Mq
{
    public record YandexDiskPlayerSynchronization(
        Guid Id,
        string AccessToken,
        string RefreshToken);
}
