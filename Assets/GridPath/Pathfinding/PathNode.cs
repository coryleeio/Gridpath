using Priority_Queue;

namespace Gridpath
{
    public class PathNode : FastPriorityQueueNode
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public PathNode parent;
    }
}
