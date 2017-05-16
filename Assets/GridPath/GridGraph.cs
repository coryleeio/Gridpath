using System.Collections.Generic;

namespace GridPath
{
    public class GridGraph
    {
        private GraphNode[,] _grid;
        private List<Point> _neighborOffsets = new List<Point>();

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
                recalculateNeighborOffsets();
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
                recalculateNeighborOffsets();
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

        private void recalculateNeighborOffsets()
        {
            _neighborOffsets.Clear();
            if (AllowDiagnals)
            {
                _neighborOffsets.Add(new Point(0, 1));
                _neighborOffsets.Add(new Point(1, 0));
                _neighborOffsets.Add(new Point(0, -1));
                _neighborOffsets.Add(new Point(-1, 0));

                _neighborOffsets.Add(new Point(1, 1));
                _neighborOffsets.Add(new Point(1, -1));
                _neighborOffsets.Add(new Point(-1, 1));
                _neighborOffsets.Add(new Point(-1, -1));
            }
            else
            {
                _neighborOffsets.Add(new Point(0, 1));
                _neighborOffsets.Add(new Point(1, 0));
                _neighborOffsets.Add(new Point(0, -1));
                _neighborOffsets.Add(new Point(-1, 0));
            }
        }

        private GridGraph()
        {

        }

        public GridGraph(int sizeX, int sizeY)
        {
            _grid = new GraphNode[sizeX, sizeY];
            for(var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    _grid[x, y] = new GraphNode()
                    {
                        X = x,
                        Y = y
                    };
                }
            }
            recalculateNeighborOffsets();
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
            var neighbors = new List<GraphNode>();
            foreach(var offset in _neighborOffsets)
            {
                var neighborX = x + offset.x;
                var neighborY = y + offset.y;
                if (NodeInGrid(neighborX, neighborY))
                {
                    neighbors.Add(NodeAt(neighborX, neighborY));
                }
            }
            return neighbors;
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
