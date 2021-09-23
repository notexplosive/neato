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
    }
}
