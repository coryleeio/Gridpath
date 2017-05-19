using System.Collections.Generic;

namespace GridPath
{
    public class GridGraph
    {
        private GraphNode[,] _grid;

        private bool _allowDiagnals = true;
        public bool AllowDiagnals
        {
            get
            {
                return _allowDiagnals;
            }
            set
            {
                _allowDiagnals = value;
                CalculateAllNeighbors(SizeX, SizeY);
            }
        }

        private bool _allowCutCorners = false;
        public bool AllowCutCorners
        {
            get
            {
                return _allowCutCorners;
            }
            set
            {
                _allowCutCorners = value;
                CalculateAllNeighbors(SizeX, SizeY);
            }
        }

        public int MaxX
        {
            get
            {
                return _grid.GetUpperBound(0);
            }
        }

        public int MaxY
        {
            get
            {
                return _grid.GetUpperBound(1);
            }
        }

        public int SizeX
        {
            get
            {
                return _grid.GetUpperBound(0) + 1;
            }
        }

        public int SizeY
        {
            get
            {
                return _grid.GetUpperBound(1) + 1;
            }
        }

        private GridGraph()
        {

        }

        public GridGraph(int sizeX, int sizeY)
        {
            _grid = new GraphNode[sizeX, sizeY];
            var numNeighbors = 4;
            if(_allowDiagnals)
            {
                numNeighbors = 8;
            }
            for(var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    _grid[x, y] = new GraphNode(numNeighbors)
                    {
                        X = x,
                        Y = y
                    };
                }
            }
            CalculateAllNeighbors(sizeX, sizeY);
        }

        public GraphNode NodeAt(int x, int y)
        {
            return _grid[x, y];
        }

        public bool NodeInGrid(int x, int y)
        {
            return x >= 0 && x <= MaxX && y >= 0 && y <= MaxY;
        }

        public List<GraphNode> Neighbors(int x, int y)
        {
            return NodeAt(x, y).neighbors;
        }

        private void CalculateAllNeighbors(int sizeX, int sizeY)
        {
            var neighborOffsets = new List<Point>();
            if (AllowDiagnals)
            {
                neighborOffsets.Add(new Point(0, 1));
                neighborOffsets.Add(new Point(1, 0));
                neighborOffsets.Add(new Point(0, -1));
                neighborOffsets.Add(new Point(-1, 0));

                neighborOffsets.Add(new Point(1, 1));
                neighborOffsets.Add(new Point(1, -1));
                neighborOffsets.Add(new Point(-1, 1));
                neighborOffsets.Add(new Point(-1, -1));
            }
            else
            {
                neighborOffsets.Add(new Point(0, 1));
                neighborOffsets.Add(new Point(1, 0));
                neighborOffsets.Add(new Point(0, -1));
                neighborOffsets.Add(new Point(-1, 0));
            }

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    NodeAt(x, y).neighbors.Clear();
                    foreach (var offset in neighborOffsets)
                    {
                        var neighborX = x + offset.x;
                        var neighborY = y + offset.y;
                        if (NodeInGrid(neighborX, neighborY))
                        {
                            NodeAt(x, y).neighbors.Add(NodeAt(neighborX, neighborY));
                        }
                    }
                }
            }
        }

        public void SetWalkable(int x, int y, bool walkable)
        {
            if(NodeInGrid(x, y))
            {
                _grid[x, y].walkable = walkable;
            }
            else
            {
                throw new System.Exception("Cannot set walkable on node outside of grid");
            }
        }

        public void SetWeight(int x, int y, int weight)
        {
            if (NodeInGrid(x, y))
            {
                _grid[x, y].weight = weight;
            }
            else
            {
                throw new System.Exception("Cannot set weight on node outside of grid");
            }
        }
    }

}
