using System.IO;

namespace Plonk
{
    public class PathContext
    {
        public readonly PathType pathType;
        public readonly string path;

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
