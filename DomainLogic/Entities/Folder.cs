using System.Linq;

namespace DomainLogic.Entities
{
    public record Folder
    {
        public Folder(string path)
        {
            Path = path;
            var folderArray = path.Split("\\");

            Name = folderArray.Last();
            ParentFolder = folderArray
                            .Skip(folderArray.Length - 2)
                            .FirstOrDefault(); 
        }

        public string Path { get; }
        public string Name { get; }
        public string ParentFolder { get; }
    }
}