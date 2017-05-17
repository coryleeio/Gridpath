using Assets;
using UnityEngine;
using GridPath;
using Point = GridPath.Point;

[RequireComponent(typeof(PathFinder))]
public class TilePopulator : MonoBehaviour
{
    public Mesh mesh;
    public GameObject tile1;
    public GameObject tile2;
    public GameObject collidableTile;
    public GameObject seekerPrefab;
    public GameObject targetPrefab;
    public int gridSizeX;
    public int gridSizeY;
    public int numSeekers;
    private GameObject target;

    void Start()
    {
        BuildMap();
        SpawnTarget();
        SpawnSeekers();
    }

    public Point FindWalkable()
    {
        var hasFound = false;
        while (hasFound == false)
        {
            var x = Random.Range(0, gridSizeX);
            var y = Random.Range(0, gridSizeY);
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
        var isoPosition = IsoUtil.ToFloor(IsoUtil.CartesianToIso(point.x, point.y));
        target = Instantiate(targetPrefab, isoPosition, Quaternion.identity);
        setIsoPosition(target, point.x, point.y);
    }

    public void SpawnSeekers()
    {
        for(var i = 0; i < numSeekers; i++)
        {
            var point = FindWalkable();
            var isoPosition = IsoUtil.ToFloor(IsoUtil.CartesianToIso(point.x, point.y));
            var go = Instantiate(seekerPrefab, isoPosition, Quaternion.identity);
            setIsoPosition(go, point.x,point.y);
            var seekAI = go.GetComponent<SeekAI>();
            seekAI.Seek(target.transform);
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
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 drawLocation = IsoUtil.CartesianToIso(x, y);
                if (Random.Range(0, 5) == 4)
                {
                    var go = GameObject.Instantiate(collidableTile, drawLocation, Quaternion.identity);
                    setIsoPosition(go, x, y);
                    pathFinder.Grid.SetWalkable(x, y, false);
                }
                else if ((x % 2 == 0 && y % 2 == 0) || x % 2 == 1 && y % 2 == 1)
                {
                    var go = GameObject.Instantiate(tile2, drawLocation, Quaternion.identity);
                    setIsoPosition(go, x, y);
                }
                else
                {
                    var go = GameObject.Instantiate(tile1, drawLocation, Quaternion.identity);
                    setIsoPosition(go, x, y);
                }
            }
        }
    }
}
