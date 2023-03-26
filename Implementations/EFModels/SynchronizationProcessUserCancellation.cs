using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations.EFModels
{
    public class SynchronizationProcessUserCancellation
    {
        public Guid Id { get; set; }
        public DateTimeOffset? CreateDateTime { get; set; }

        public Guid SynchronizationProcessId { get; set; }
    }
}
