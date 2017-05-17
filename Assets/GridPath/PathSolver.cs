using Assets.GridPath;
using Priority_Queue;
using System;
using System.Collections.Generic;

namespace GridPath
{
    public class PathSolver
    {
        private static readonly int _max_open_nodes = 2000;
        private static readonly int _orthogonal_weight = 10;
        private static readonly int _diagonal_weight = 14;
        private FastPriorityQueue<PathNode> _open = new FastPriorityQueue<PathNode>(_max_open_nodes);
        private List<PathNode> _closed = new List<PathNode>();

        public Path FindPath(Point start, Point end, GridGraph grid)
        {
            var path = new Path(); // it would be more efficient to pool this
            path.Reset(); // have the pool do this on check-in

            var parentNode = new PathNode();
            parentNode.G = 0;
            parentNode.H = 2;
            parentNode.F = parentNode.G + parentNode.H;
            parentNode.X = start.x;
            parentNode.Y = start.y;
            parentNode.parent = null;
            path.startNode = parentNode;
            _open.Enqueue(parentNode, parentNode.F);

            while(_open.Count > 0)
            {
                parentNode = _open.Dequeue();
                if (parentNode.X == end.x && parentNode.Y == end.y)
                {
                    path.found = true;
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
                    newNode.H = 2 * (Math.Abs(newNode.X - end.x) + Math.Abs(newNode.Y - end.y));
                    newNode.F = newNode.G + newNode.H;

                    _open.Enqueue(newNode, newNode.F);
                }
                _closed.Add(parentNode);
            }
            if(path.found)
            {
                while (parentNode.parent != null)
                {
                    path.nodes.Add(parentNode);
                    parentNode = parentNode.parent;
                }
                path.nodes.Reverse();
                return path;
            }
            _closed.Clear();
            return path;
        }
    }
}
