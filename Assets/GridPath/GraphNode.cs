using System.Collections.Generic;

namespace GridPath
{
    public class GraphNode
    {
        public int X;
        public int Y;
        public bool walkable = true;
        public int weight = 0;
        public List<GraphNode> neighbors;

        private GraphNode()
        {

        }

        public GraphNode(int numNeighbors)
        {
            neighbors = new List<GraphNode>(numNeighbors);
        }
    }
}
