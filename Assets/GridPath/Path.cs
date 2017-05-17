using GridPath;
using System.Collections.Generic;

namespace Assets.GridPath
{
    public class Path
    {
        public PathNode startNode;
        public List<PathNode> nodes;
        public bool found;

        public void Reset()
        {
            startNode = null;
            found = false;
            if(nodes == null)
            {
                nodes = new List<PathNode>();
            }
            nodes.Clear();
        }
    }
}
