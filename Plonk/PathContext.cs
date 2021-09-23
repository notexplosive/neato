using System.IO;

namespace Neato
{
    public class PathContext
    {
        private readonly PathType pathType;
        private readonly string path;

        public PathContext(PathType pathType, string path)
        {
            this.pathType = pathType;
            this.path = path;
        }

        public string CalculatePath(string workingDirectory)
        {
            if (this.pathType == PathType.Relative)
            {
                return Path.Join(workingDirectory, this.path);
            }
            else
            {
                return this.path;
            }
        }
    }
}
