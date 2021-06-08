using System;
using System.Collections.Generic;

namespace DomainLogic.Entities
{
    public record File(string Name, string Path)
    {
        public string ParentFolderPath { get; internal set; }
        public string ParentFolder { get; internal set; }
        public string Type { get; internal set; }
        public string YandexUserId { get; set; }
        public string ResourceId { get; set; }
        public Guid SynchronizationProcessId  { get; set; }
    }
}