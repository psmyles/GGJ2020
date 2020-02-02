using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsUtils
{
    public static bool RaycastToGround(Vector3 location, out RaycastHit rayHit)
    {
        if (Physics.Raycast(location, Vector3.up * -10, out rayHit, 1.0f, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }

        return false;
    }
}
