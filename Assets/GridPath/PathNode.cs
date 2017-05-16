using Priority_Queue;

namespace GridPath
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
