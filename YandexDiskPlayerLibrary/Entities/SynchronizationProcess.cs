using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskPlayerLibrary.Entities
{
    public class SynchronizationProcess
    {
        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public List<Folder> Folders { get; set; }

        public List<File> Files { get; set; }

        public int Offset { get; set; }

        public string LastFileId { get; set; }

        public SynchronizationProcessState State { get; set; }
    }
}
