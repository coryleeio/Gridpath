using Assets;
using GridPath;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CartesianPosition))]
public class SeekAI : MonoBehaviour
{
    private Path _path;
    private int _pathIndex = 0;
    private CartesianPosition _targetPosition;
    private CartesianPosition _myPosition;
    private float _repathRate;

    public void Seek(Transform newTarget)
    {
        _myPosition = GetComponent<CartesianPosition>();
        _targetPosition = newTarget.GetComponent<CartesianPosition>();
    }

    IEnumerator Move()
    {
        while(true)
        {
            if (_path != null && _pathIndex < _path.Nodes.Count)
            {
                var nextNode = _path.Nodes[_pathIndex];
                _myPosition.X = nextNode.X;
                _myPosition.Y = nextNode.Y;
                _pathIndex++;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Repath()
    {
        while (true)
        {
            if (_targetPosition != null)
            {
                PathFinder.Instance.StartPath(_myPosition.X, _myPosition.Y, _targetPosition.X, _targetPosition.Y, OnPathComplete);
            }
            yield return new WaitForSeconds(_repathRate); // offset requests
        }
    }

    void Start()
    {
        _repathRate = 4.0f + Random.Range(0.0f, 1.0f);
        StartCoroutine(Move());
        StartCoroutine(Repath());
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Received path!");
        _path = p;
        _pathIndex = 0;
    }
}
