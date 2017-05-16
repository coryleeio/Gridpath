using GridPath;
using UnityEngine;

public class SimplePath : MonoBehaviour {

	void Start () {
        GridGraph grid = new GridGraph(4, 4);
        PathSolver solver = new PathSolver();
        grid.SetWalkable(1, 0, false);
        grid.SetWalkable(1, 1, false);
        grid.SetWalkable(1, 2, false);
        var path = solver.FindPath(new Point(0, 0), new Point(3, 3), grid);
    }
}
