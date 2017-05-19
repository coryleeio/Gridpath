using System;
using GridPath;
using System.Collections.Generic;

namespace Assets.GridPath
{
    public class NaivePathSolver : IPathSolver
    {
        private static readonly int _orthogonal_weight = 10;
        private static readonly int _diagonal_weight = 14;
        private List<PathNode> _open = new List<PathNode>();
        private List<PathNode> _closed = new List<PathNode>();

        public Path FindPath(int startX, int startY, int endX, int endY, GridGraph grid)
        {
            _open.Clear();
            _closed.Clear();
            var path = new Path();
            path.Reset();

            var startNode = new PathNode();
            startNode.G = 0;
            startNode.H = 2;
            startNode.F = startNode.G + startNode.H;
            startNode.X = startX;
            startNode.Y = startY;
            startNode.parent = null;
            path.StartNode = startNode;
            _open.Add(startNode);

            while (_open.Count > 0)
            {
                var current = _open[0];
                _open.Remove(current);
                _closed.Add(current);
                if (current.X == endX && current.Y == endY)
                {
                    path.Found = true;
                    break;
                }

                foreach (var neighbor in grid.Neighbors(current.X, current.Y))
                {
                    if (!neighbor.walkable)
                    {
                        continue;
                    }

                    PathNode foundInClosed = null;
                    foreach (var closedNode in _closed)
                    {
                        if (neighbor.X == closedNode.X && neighbor.Y == closedNode.Y)
                        {
                            foundInClosed = closedNode;
                        }
                    }

                    if (foundInClosed != null)
                    {
                        continue;
                    }

                    PathNode foundInOpen = null;
                    foreach (var openNode in _open)
                    {
                        if (neighbor.X == openNode.X && neighbor.Y == openNode.Y)
                        {
                            foundInOpen = openNode;
                        }
                    }

                    var moveWasDiagonal = current.X != neighbor.X && current.Y != neighbor.Y;
                    var newGValueForPath = current.G + neighbor.weight;

                    if (moveWasDiagonal)
                    {
                        newGValueForPath += _diagonal_weight;
                    }
                    else
                    {
                        newGValueForPath += _orthogonal_weight;
                    }


                    if (foundInOpen == null)
                    {
                        // not in open
                        PathNode neighborPathNode = new PathNode()
                        {
                            X = neighbor.X,
                            Y = neighbor.Y
                        };

                        neighborPathNode.parent = current;
                        neighborPathNode.H = 2 * (Math.Abs(neighborPathNode.X - endX) + Math.Abs(neighborPathNode.Y - endY));
                        neighborPathNode.G = newGValueForPath;
                        neighborPathNode.F = neighborPathNode.G + neighborPathNode.H;
                        _open.Add(neighborPathNode);
                    }
                    else
                    {
                        // in open already, update value if necessary
                        if (newGValueForPath >= foundInOpen.G)
                        {
                            continue;
                        }
                        foundInOpen.parent = current;
                        foundInOpen.G = newGValueForPath;
                        foundInOpen.F = foundInOpen.G + foundInOpen.H;
                        _open.Sort();
                    }
                }
            }
            if (path.Found)
            {
                PathNode nextNodeInPath = null;
                for (var i = _closed.Count - 1; i >= 0; i--)
                {
                    var node = _closed[i];
                    if (endX == node.X && endY == node.Y)
                    {
                        nextNodeInPath = node;
                        path.Nodes.Add(nextNodeInPath);
                        break;
                    }
                }
                while (nextNodeInPath.parent != null)
                {
                    path.Nodes.Add(nextNodeInPath);
                    nextNodeInPath = nextNodeInPath.parent;
                }
                path.Nodes.Reverse();
                return path;
            }
            return path;
        }
    }
}
