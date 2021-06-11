using System;
using System.ComponentModel.DataAnnotations;

namespace Implementations.EFModels
{
    public class IgnorePath
    {
        [Key]
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string YandexUserId { get; set; }
    }
}