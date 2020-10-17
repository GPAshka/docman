namespace Docman.API.Application.Commands
{
    public class AddFileCommand
    {
        public string FileName { get; }
        public string FileDescription { get; }
        
        public AddFileCommand(string fileName, string fileDescription)
        {
            FileName = fileName;
            FileDescription = fileDescription;
        }
    }
}