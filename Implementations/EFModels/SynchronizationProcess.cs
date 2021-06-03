using DomainLogic.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations.EFModels
{
    public class SynchronizationProcess
    {
        [Key]
        public Guid Id { get ; set; }
        public DateTimeOffset? CreateDateTime { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? FinishedDateTime { get; set; }
        public int Offset { get; set; }
        public string LastFileId { get; set; }
        public SynchronizationProcessState State { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
