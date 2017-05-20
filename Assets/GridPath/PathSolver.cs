using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridPath
{
    public class PathSolver
    {
        private static readonly int _max_open_nodes = 2000;
        private static readonly int _orthogonal_weight = 10;  // 1 * 10          orthogonal weight multiplied by 10 to remain an int
        private static readonly int _diagonal_weight = 14;    // sqrt(2) * 10    diagonal weight multiplied by 10 to remain an int
        private FastPriorityQueue<PathNode> _open = new FastPriorityQueue<PathNode>(_max_open_nodes); // open min-priority queue sorted by lowest F
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
                    if(!neighbor.Walkable)
                    {
                        continue;
                    }

                    var neighborNode = new PathNode();
                    neighborNode.X = neighbor.X;
                    neighborNode.Y = neighbor.Y;

                    var moveWasDiagonal = parentNode.X != neighborNode.X && parentNode.Y != neighborNode.Y;

                    var newGValueForPath = parentNode.G + neighbor.Weight;

                    if(moveWasDiagonal)
                    {
                        if(!grid.AllowCutCorners)
                        {
                            // If we dont allow cutting corners and this is a corner cut, skip this node,
                            // as it cannot be traversed from this position
                        }
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
                        if(openNode.X == neighborNode.X && openNode.Y == neighborNode.Y)
                        {
                            foundInOpen = openNode;
                            break;
                        }
                    }

                    if (foundInOpen != null && foundInOpen.G <= newGValueForPath)
                    {
                        // Found this in open, and its a worse path, skip this node
                        continue;
                    }

                    PathNode foundInClosed = null;
                    foreach (var closedNode in _closed)
                    {
                        if (closedNode.X == neighborNode.X && closedNode.Y == neighborNode.Y)
                        {
                            foundInClosed = closedNode;
                            break;
                        }
                    }

                    if (foundInClosed != null && foundInClosed.G <= newGValueForPath)
                    {
                        // Found in closed, and it is a worse path, skip this node
                        continue;
                    }

                    // New node not in the open list, or 
                    // or a node in the open list for which we have found a better path
                    // so calculate G H and F
                    // and put it in the queue, this is technically a duplicate PathNode, but since its at a lower F value
                    // it does not need to be removed from the priority queue
                    neighborNode.parent = parentNode;
                    neighborNode.G = newGValueForPath;
                    neighborNode.H = (int)(2 * (Math.Pow((neighborNode.X - endX), 2) + Math.Pow((neighborNode.Y - endY), 2)));
                    neighborNode.F = neighborNode.G + neighborNode.H;

                    _open.Enqueue(neighborNode, neighborNode.F);
                }
                _closed.Add(parentNode);
            }

            if(path.Found)
            {
                // Recursively traverse each parent from the endpoint, then reverse the nodes, this is our path
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
