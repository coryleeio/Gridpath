using System.Collections.Generic;

namespace Gridpath
{ 
    public class GridGraph
    {
        private GraphNode[,] _grid;


        public enum DiagonalOptions
        {
            DiagonalsWithoutCornerCutting,
            NoDiagonals,
            DiagonalsWithCornerCutting
        }

        public DiagonalOptions DiagonalSetting;
        public List<Point> DiagonalOffsets = new List<Point>();
        public List<Point> OrthogonalOffsets = new List<Point>();
        public List<Point> NeighborOffsets = new List<Point>();

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

        public GridGraph(int sizeX, int sizeY, DiagonalOptions diagonalSetting)
        {
            _grid = new GraphNode[sizeX, sizeY];
            DiagonalSetting = diagonalSetting;
            var numNeighbors = 4;
            if (DiagonalSetting == DiagonalOptions.DiagonalsWithCornerCutting || DiagonalSetting == DiagonalOptions.DiagonalsWithoutCornerCutting)
            {
                numNeighbors = 8;
            }
            else if(DiagonalSetting == DiagonalOptions.NoDiagonals)
            {
                numNeighbors = 4;
            }
            else
            {
                throw new System.Exception("Not implemented");
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
            return NodeAt(x, y).Neighbors;
        }

        private void CalculateAllNeighbors(int sizeX, int sizeY)
        {
            OrthogonalOffsets.Clear();
            DiagonalOffsets.Clear();
            NeighborOffsets.Clear();

            var up = new Point(1, 0);
            var down = new Point(0, -1);
            var left = new Point(-1, 0);
            var right = new Point(0, 1);

            OrthogonalOffsets.Add(up);
            OrthogonalOffsets.Add(down);
            OrthogonalOffsets.Add(left);
            OrthogonalOffsets.Add(right);
            NeighborOffsets.Add(up);
            NeighborOffsets.Add(down);
            NeighborOffsets.Add(left);
            NeighborOffsets.Add(right);

            if (DiagonalSetting == DiagonalOptions.DiagonalsWithCornerCutting || DiagonalSetting == DiagonalOptions.DiagonalsWithoutCornerCutting)
            {
                var upRight = new Point(1, 1);
                var upLeft = new Point(-1, 1);
                var downLeft = new Point(-1, -1);
                var downRight = new Point(1, -1);
                DiagonalOffsets.Add(upRight);
                DiagonalOffsets.Add(upLeft);
                DiagonalOffsets.Add(downLeft);
                DiagonalOffsets.Add(downRight);
                NeighborOffsets.Add(upRight);
                NeighborOffsets.Add(upLeft);
                NeighborOffsets.Add(downLeft);
                NeighborOffsets.Add(downRight);
            }
            if(DiagonalSetting != DiagonalOptions.DiagonalsWithCornerCutting && DiagonalSetting != DiagonalOptions.DiagonalsWithoutCornerCutting && DiagonalSetting != DiagonalOptions.NoDiagonals)
            {
                throw new System.Exception("Not implemented");
            }

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    NodeAt(x, y).Neighbors.Clear();
                    foreach (var offset in NeighborOffsets)
                    {
                        var neighborX = x + offset.x;
                        var neighborY = y + offset.y;
                        if (NodeInGrid(neighborX, neighborY))
                        {
                            NodeAt(x, y).Neighbors.Add(NodeAt(neighborX, neighborY));
                        }
                    }
                }
            }
        }

        public void SetWalkable(int x, int y, bool walkable)
        {
            if(NodeInGrid(x, y))
            {
                _grid[x, y].Walkable = walkable;
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
                _grid[x, y].Weight = weight;
            }
            else
            {
                throw new System.Exception("Cannot set weight on node outside of grid");
            }
        }
    }

}
