using Priority_Queue;
using System;

namespace GridPath
{
    public class PathNode : FastPriorityQueueNode, IComparable<PathNode>
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public PathNode parent;

        public int CompareTo(PathNode other)
        {
            if(F < other.F)
            {
                return -1;
            }
            return 1;
        }
    }
}
