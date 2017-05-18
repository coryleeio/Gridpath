using Assets;
using Assets.GridPath;
using GridPath;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(IsoObj))]
public class SeekAI : MonoBehaviour
{
    private Path _path;
    private int pathIndex = 0;

    public void Seek(Transform newTarget)
    {
        var myIso = GetComponent<IsoObj>();
        if (newTarget != null)
        {
            var targetIso = newTarget.GetComponent<IsoObj>();
            PathFinder.Instance.StartPath(myIso.x, myIso.y, targetIso.x, targetIso.y, OnPathComplete);
        }
    }

    IEnumerator Move()
    {
        while(true)
        {
            if(_path != null && pathIndex < _path.Nodes.Count)
            {
                var nextNode = _path.Nodes[pathIndex];
                transform.position = IsoUtil.CartesianToIso(nextNode.X, nextNode.Y, IsoUtil.IsoType.FLOOR);
                pathIndex++;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void Start()
    {
        StartCoroutine(Move());
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Received path!");
        _path = p;
        pathIndex = 0;
    }
}
