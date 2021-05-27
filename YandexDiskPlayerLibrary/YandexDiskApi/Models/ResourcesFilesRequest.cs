using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskPlayerLibrary.YandexDiskApi.Models
{
    public record ResourcesFilesRequest
    {
        public int Limit { get; init; }
        public int Offset { get; init; }
        public string MediaType { get; init; }
        public List<string> Fields { get; init; }
    }
}
