using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ProjectileTargetArea : MonoBehaviour
{
    public Vector3 GetRandomPoint()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 halfSize = box.size * 0.5f;

        float x = Random.Range(-halfSize.x, halfSize.x);
        float y = Random.Range(-halfSize.y, halfSize.y);
        float z = Random.Range(-halfSize.z, halfSize.z);

        return transform.TransformPoint(new Vector3(x, y, z));
    }
}
