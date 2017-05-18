using Assets;
using UnityEngine;
using GridPath;
using Point = GridPath.Point;

[RequireComponent(typeof(PathFinder))]
public class TilePopulator : MonoBehaviour
{
    public Mesh Mesh;
    public GameObject FloorTile1;
    public GameObject FloorTile2;
    public GameObject CollidableTile1;
    public GameObject SeekerPrefab;
    public GameObject TargetPrefab;
    public int GridSizeX;
    public int GridSizeY;
    public int NumberOfSeekers;
    public int Seed;
    private GameObject Target;

    void Start()
    {
        Random.seed = Seed;
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
            if (node.walkable)
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
        var isoPosition = IsoUtil.CartesianToIso(point.x, point.y, IsoUtil.IsoType.FLOOR);
        Target = Instantiate(TargetPrefab, isoPosition, Quaternion.identity);
        setIsoPosition(Target, point.x, point.y);
    }

    public void SpawnSeekers()
    {
        for(var i = 0; i < NumberOfSeekers; i++)
        {
            var point = FindWalkable();
            var isoPosition = IsoUtil.CartesianToIso(point.x, point.y,IsoUtil.IsoType.FLOOR);
            var go = Instantiate(SeekerPrefab, isoPosition, Quaternion.identity);
            setIsoPosition(go, point.x,point.y);
            var seekAI = go.GetComponent<SeekAI>();
            seekAI.Seek(Target.transform);
        }
    }

    public void setIsoPosition(GameObject ob, int x, int y)
    {
        var iso = ob.GetComponent<IsoObj>();
        iso.x = x;
        iso.y = y;
    }

    private void BuildMap()
    {
        var pathFinder = PathFinder.Instance;
        pathFinder.BuildGrid(GridSizeX, GridSizeY);
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                Vector3 drawLocation = IsoUtil.CartesianToIso(x, y, IsoUtil.IsoType.TILE);
                if (Random.Range(0, 5) == 4)
                {
                    var go = GameObject.Instantiate(CollidableTile1, drawLocation, Quaternion.identity);
                    setIsoPosition(go, x, y);
                    pathFinder.Grid.SetWalkable(x, y, false);
                }
                else if ((x % 2 == 0 && y % 2 == 0) || x % 2 == 1 && y % 2 == 1)
                {
                    var go = GameObject.Instantiate(FloorTile2, drawLocation, Quaternion.identity);
                    setIsoPosition(go, x, y);
                }
                else
                {
                    var go = GameObject.Instantiate(FloorTile1, drawLocation, Quaternion.identity);
                    setIsoPosition(go, x, y);
                }
            }
        }
    }
}
