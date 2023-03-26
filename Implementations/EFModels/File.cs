using System;
using System.ComponentModel.DataAnnotations;

namespace Implementations.EFModels
{
    public class File
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ParentFolderPath { get; set; }
        public string ParentFolder { get; set; }
        public string Type { get; set; }
        public string YandexResourceId { get; set; }
        public string YandexUserId { get; set; }
        public Guid SynchronizationProcessId { get; set; }
        public DateTimeOffset? CreateDateTime { get; set; }
        public DateTimeOffset? LastUpdateDateTime { get; set; }
    }
}