using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskPlayerLibrary.YandexDiskApi.Models
{
    public record ResourcesFileResponse
    {
        public List<ResourcesFileItem> Items { get; init; }

        public int Offset { get; init; }
    }

    public record ResourcesFileItem
    {
        public string Name { get; init; }

        public string Path { get; init; }

        public string File { get; init; }

        public string ResourceId { get; init; }
    }
}
