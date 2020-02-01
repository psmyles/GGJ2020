using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshUtils
{
    public static void CreateLineMesh(List<Vector3> points, Vector3 upVec, float thickness
        , out List<Vector3> meshVerts, out List<Vector2> meshUV, out List<int> indices)
    {
        meshVerts = null;
        meshUV = null;
        indices = null;

        if (points.Count <= 1)
            return;

        meshVerts = new List<Vector3>();
        meshUV = new List<Vector2>();
        indices = new List<int>();
        Vector3 lastTopLeft = Vector3.zero, lastTopRight = Vector3.zero;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 currPt = points[i];
            Vector3 nextPt = points[i + 1];

            Vector3 distVec = nextPt - currPt;
            float dist = distVec.magnitude;
            Vector3 dirVec = distVec.normalized;

            Vector3 rightVec = Vector3.Cross(dirVec, upVec);
            rightVec.Normalize();

            Vector3 botLeft = currPt - rightVec * thickness * 0.5f;
            Vector3 botRight = currPt + rightVec * thickness * 0.5f;

            if (i != 0)
            {
                botLeft = (botLeft + lastTopLeft) * 0.5f;
                botRight = (botRight + lastTopRight) * 0.5f;

                int lastTopLeftInd = meshVerts.Count - 2;
                int lastTopRightInd = meshVerts.Count - 1;

                meshVerts[lastTopLeftInd] = botLeft;
                meshVerts[lastTopRightInd] = botRight;
            }
           
            Vector3 topLeft = nextPt - rightVec * thickness * 0.5f;
            Vector3 topRight = nextPt + rightVec * thickness * 0.5f;

            lastTopLeft = topLeft;
            lastTopRight = topRight;

            meshVerts.Add(botLeft); meshVerts.Add(botRight);
            meshVerts.Add(topLeft); meshVerts.Add(topRight);

            meshUV.Add(new Vector2(0.0f, 0.0f)); meshUV.Add(new Vector2(1.0f, 0.0f));
            meshUV.Add(new Vector2(0.0f, 1.0f)); meshUV.Add(new Vector2(1.0f, 1.0f));

            int quadInd = i * 4;
            indices.Add(quadInd + 0); indices.Add(quadInd + 1); indices.Add(quadInd + 3);
            indices.Add(quadInd + 0); indices.Add(quadInd + 3); indices.Add(quadInd + 2);
        }
        
    }
}
