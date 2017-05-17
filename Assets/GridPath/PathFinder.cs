using Assets.GridPath;
using Assets.Game;
using UnityEngine;
namespace GridPath
{
    public class PathFinder : Singleton<PathFinder>
    {
        public int GridSizeX;
        public int GridSizeY;
        public bool showGraph;
        private PathSolver _solver = new PathSolver();
        private Path mostRecentPath;
        private GridGraph _grid;
        public GridGraph Grid
        {
            get
            {
                return _grid;
            }
            private set
            {
                _grid = value;
            }
        }

        void Start()
        {
            Grid = new GridGraph(GridSizeX, GridSizeY);
        }

        private Vector3 rotateForDraw(float x, float y)
        {
            return new Vector3(x, -y, 0.0f);
        }

        public delegate void OnPathComplete(Path path);

        public void StartPath(int startX, int startY, int endX, int endY, OnPathComplete handler)
        {
            Debug.Log("started path!");
            mostRecentPath = _solver.FindPath(new Point(startX, startY), new Point(endX, endY), Grid);
            handler(mostRecentPath);
        }

        private void DrawSquare(float x, float y)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(rotateForDraw(x - 0.5f, y + 0.5f), rotateForDraw(x + 0.5f, y + 0.5f));
            Gizmos.DrawLine(rotateForDraw(x + 0.5f, y  + 0.5f), rotateForDraw(x + 0.5f,y - 0.5f));
            Gizmos.DrawLine(rotateForDraw(x + 0.5f, y - 0.5f), rotateForDraw(x - 0.5f, y - 0.5f));
            Gizmos.DrawLine(rotateForDraw(x - 0.5f, y - 0.5f), rotateForDraw(x - 0.5f, y + 0.5f));
        }
        private void DrawX(float x, float y)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rotateForDraw(x - 0.5f, y + 0.5f), rotateForDraw(x + 0.5f, y - 0.5f));
            Gizmos.DrawLine(rotateForDraw(x + 0.5f, y + 0.5f), rotateForDraw(x - 0.5f, y - 0.5f));
        }

        void OnDrawGizmosSelected()
        {
            if (Grid != null && showGraph)
            {
                var sizeX = Grid.SizeX;
                var sizeY = Grid.SizeY;
                for (var y = 0; y < sizeY; y++)
                {
                    for (var x = 0; x < sizeX; x++)
                    {
                        DrawSquare(x, y);
                        if (!Grid.NodeAt(x, y).walkable)
                        {
                            DrawX(x, y);
                        }
                    }
                }

                if(mostRecentPath != null && mostRecentPath.found)
                {
                    if(mostRecentPath.nodes.Count > 0)
                    {
                        var nextNode = mostRecentPath.nodes[0];
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(rotateForDraw(mostRecentPath.startNode.X, mostRecentPath.startNode.Y), rotateForDraw(nextNode.X, nextNode.Y));
                    }
                    for (var i = 0; i < mostRecentPath.nodes.Count; i++)
                    {
                        var node = mostRecentPath.nodes[i];
                        PathNode nextNode = null;
                        if (i + 1 < mostRecentPath.nodes.Count)
                        {
                            nextNode = mostRecentPath.nodes[i + 1];
                        }
                        if (nextNode != null)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(rotateForDraw(node.X, node.Y), rotateForDraw(nextNode.X, nextNode.Y));
                        }
                    }
                }

            }
        }
    }
}
