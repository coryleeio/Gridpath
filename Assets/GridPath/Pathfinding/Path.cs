using System.Collections.Generic;

namespace Gridpath
{
    public class Path
    {
        public PathNode StartNode;
        public List<PathNode> Nodes;
        public List<string> Errors = new List<string>();
        public bool HasErrors
        {
            get
            {
                return Errors.Count > 0;
            }
        }
        public bool Found;

        public void Reset()
        {
            StartNode = null;
            Found = false;
            if(Nodes == null)
            {
                Nodes = new List<PathNode>();
            }
            Errors.Clear();
            Nodes.Clear();
        }
    }
}
