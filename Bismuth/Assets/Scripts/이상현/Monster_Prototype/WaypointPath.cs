using System;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public int WaypointCount => transform.childCount;
    
    public Transform GetWaypoint(int index) => transform.GetChild(index);
    
    private void OnDrawGizmos()
    {
        if (transform.childCount < 2) return;
        
        Gizmos.color = Color.yellow;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform current =  transform.GetChild(i);
            Gizmos.DrawSphere(current.position, 0.1f);

            if (i < transform.childCount - 1)
            {
                Transform next = transform.GetChild(i + 1);
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }
}
