using Assets;
using UnityEngine;
using GridPath;
using Point = GridPath.Point;

[RequireComponent(typeof(PathFinder))]
public class TilePopulator : MonoBehaviour
{
    public Mesh Mesh;
    public GameObject LightTile;
    public GameObject DarkTile;
    public GameObject CollidableTile;
    public GameObject SeekerPrefab;
    public GameObject TargetPrefab;
    public int GridSizeX;
    public int GridSizeY;
    public int NumberOfSeekers;
    public int Seed;
    private GameObject Target;

    void Start()
    {
        Random.InitState(Seed);
        BuildMap();
        SpawnTarget();
        SpawnSeekers();
    }

    public Point FindWalkable()
    {
        var hasFound = false;
        while (hasFound == false)
        {
            var x = Random.Range(0, GridSizeX);
            var y = Random.Range(0, GridSizeY);
            var node = PathFinder.Instance.Grid.NodeAt(x, y);
            if (node.Walkable)
            {
                hasFound = true;
                return new Point(x, y);
            }
        }
        return null;
    }

    public void SpawnTarget()
    {
        var point = FindWalkable();
        var isoPosition = IsometricDrawUtility.CartesianToIsometricDraw(point.x, point.y, IsometricDrawUtility.DrawType.FLOOR);
        Target = Instantiate(TargetPrefab, isoPosition, Quaternion.identity);
        SetCartesianPosition(Target, point.x, point.y);
    }

    public void SpawnSeekers()
    {
        for(var i = 0; i < NumberOfSeekers; i++)
        {
            var point = FindWalkable();
            var isoPosition = IsometricDrawUtility.CartesianToIsometricDraw(point.x, point.y,IsometricDrawUtility.DrawType.FLOOR);
            var go = Instantiate(SeekerPrefab, isoPosition, Quaternion.identity);
            SetCartesianPosition(go, point.x,point.y);
            var seekAI = go.GetComponent<SeekAI>();
            seekAI.Seek(Target.transform);
        }
    }

    public void SetCartesianPosition(GameObject ob, int x, int y)
    {
        var pos = ob.GetComponent<CartesianPosition>();
        pos.X = x;
        pos.Y = y;
    }

    private void BuildMap()
    {
        var pathFinder = PathFinder.Instance;
        pathFinder.BuildGrid(GridSizeX, GridSizeY);
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                Vector3 drawLocation = IsometricDrawUtility.CartesianToIsometricDraw(x, y, IsometricDrawUtility.DrawType.TILE);
                if (Random.Range(0, 5) == 4)
                {
                    var go = GameObject.Instantiate(CollidableTile, drawLocation, Quaternion.identity);
                    SetCartesianPosition(go, x, y);
                    pathFinder.Grid.SetWalkable(x, y, false);
                }
                else if ((x % 2 == 0 && y % 2 == 0) || x % 2 == 1 && y % 2 == 1)
                {
                    var go = GameObject.Instantiate(DarkTile, drawLocation, Quaternion.identity);
                    SetCartesianPosition(go, x, y);
                }
                else
                {
                    var go = GameObject.Instantiate(LightTile, drawLocation, Quaternion.identity);
                    SetCartesianPosition(go, x, y);
                }
            }
        }
    }
}
