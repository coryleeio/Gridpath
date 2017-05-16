using Assets;
using UnityEngine;

public class IsoObj : MonoBehaviour {

    public int x;
    public int y;

    void Update()
    {
        transform.position = new Vector3(this.transform.position.x, this.transform.position.y, IsoUtil.CalculateIsoZBasedOnPosition(transform.position.x, transform.position.y));
    }
}
