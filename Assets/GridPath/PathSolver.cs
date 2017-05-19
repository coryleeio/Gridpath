using Priority_Queue;
using System;
using System.Collections.Generic;

namespace GridPath
{
    public class PathSolver : IPathSolver
    {
        private static readonly int _max_open_nodes = 2000;
        private static readonly int _orthogonal_weight = 10;
        private static readonly int _diagonal_weight = 14;
        private FastPriorityQueue<PathNode> _open = new FastPriorityQueue<PathNode>(_max_open_nodes);
        private List<PathNode> _closed = new List<PathNode>();

        public Path FindPath(int startX, int startY, int endX, int endY, GridGraph grid)
        {
            _open.Clear();
            _closed.Clear();
            var path = new Path();
            path.Reset();

            var parentNode = new PathNode();
            parentNode.G = 0;
            parentNode.H = 2;
            parentNode.F = parentNode.G + parentNode.H;
            parentNode.X = startX;
            parentNode.Y = startY;
            parentNode.parent = null;
            path.StartNode = parentNode;
            _open.Enqueue(parentNode, parentNode.F);

            while(_open.Count > 0)
            {
                parentNode = _open.Dequeue();
                if (parentNode.X == endX && parentNode.Y == endY)
                {
                    path.Found = true;
                    break;
                }

                var neighbors = grid.Neighbors(parentNode.X, parentNode.Y);
                foreach (var neighbor in neighbors)
                {
                    if(!neighbor.walkable)
                    {
                        continue;
                    }

                    var newNode = new PathNode();
                    newNode.X = neighbor.X;
                    newNode.Y = neighbor.Y;

                    var moveWasDiagonal = parentNode.X != newNode.X && parentNode.Y != newNode.Y;

                    var newGValueForPath = parentNode.G + neighbor.weight;

                    if(moveWasDiagonal)
                    {
                        newGValueForPath += _diagonal_weight;
                    }
                    else
                    {
                        newGValueForPath += _orthogonal_weight;
                    }

                    if (newGValueForPath == parentNode.G)
                    {
                        continue;
                    }

                    PathNode foundInOpen = null;
                    foreach(var openNode in _open)
                    {
                        if(openNode.X == newNode.X && openNode.Y == newNode.Y)
                        {
                            foundInOpen = openNode;
                            break;
                        }
                    }

                    if (foundInOpen != null && foundInOpen.G <= newGValueForPath)
                    {
                        continue;
                    }

                    PathNode foundInClosed = null;
                    foreach (var closedNode in _closed)
                    {
                        if (closedNode.X == newNode.X && closedNode.Y == newNode.Y)
                        {
                            foundInClosed = closedNode;
                            break;
                        }
                    }

                    if (foundInClosed != null && foundInClosed.G <= newGValueForPath)
                    {
                        continue;
                    }

                    newNode.parent = parentNode;
                    newNode.G = newGValueForPath;
                    newNode.H = (int)(2 * (Math.Pow((newNode.X - endX), 2) + Math.Pow((newNode.Y - endY), 2)));
                    newNode.F = newNode.G + newNode.H;

                    _open.Enqueue(newNode, newNode.F);
                }
                _closed.Add(parentNode);
            }

            if(path.Found)
            {
                while (parentNode.parent != null)
                {
                    path.Nodes.Add(parentNode);
                    parentNode = parentNode.parent;
                }
                path.Nodes.Reverse();
                return path;
            }
            _closed.Clear();
            return path;
        }
    }
}
