using UnityEngine;
namespace GridPath
{
    public class PathFinder : MonoBehaviour
    {
        public int gridSizeX;
        public int gridSizeY;
        private GridGraph _grid;
        public GridGraph Grid
        {
            get
            {
                return _grid;
            }
            set
            {
                _grid = value;
            }
        }
        
        void Start()
        {
            GridGraph grid = new GridGraph(gridSizeX, gridSizeY);
            PathSolver solver = new PathSolver();
            var path = solver.FindPath(new Point(0, 0), new Point(3, 3), grid);
        }
    }
}
