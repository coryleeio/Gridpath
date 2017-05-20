using System.Collections.Generic;

namespace GridPath
{
    public class GraphNode
    {
        public int X;
        public int Y;
        public bool Walkable = true;
        public int Weight = 0;
        public List<GraphNode> Neighbors;

        private GraphNode()
        {}

        public GraphNode(int numNeighbors)
        {
            Neighbors = new List<GraphNode>(numNeighbors);
        }
    }
}
