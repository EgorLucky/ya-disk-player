using System;
using System.ComponentModel.DataAnnotations;

namespace Implementations.EFModels
{
    public class SynchronizationProcessError
    {
        [Key]
        public Guid Id { get; set; }
        public string MessageText { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }
        public Guid SynchronizationProcessId { get; set; }
    }
}