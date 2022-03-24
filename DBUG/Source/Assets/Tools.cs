using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class Tools : MonoBehaviour
{
    [MenuItem("GameObject/Tools/Merge")]
    static void Merge(MenuCommand menuCommand)
    {
        if (menuCommand.context != Selection.activeObject) { return; }
        GameObject go = new GameObject("New Object", typeof(MeshFilter), typeof(MeshRenderer));
        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        mesh = new Mesh(); mesh.name = "Mesh";

        List<long> offsets = new List<long>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvmaps = new List<Vector2>();
        List<List<int>> triangles = new List<List<int>>();
        List<Material> materials = new List<Material>();
        List<SubMeshDescriptor> submeshes = new List<SubMeshDescriptor>();

        offsets.Add(0);
        mesh.subMeshCount = 0;
        int last = Array.IndexOf(Selection.transforms, Selection.activeTransform);
        for (int t = 0; t < Selection.transforms.Length; ++t)
        {
            MeshRenderer subrenderer = Selection.transforms[t].GetComponent<MeshRenderer>();
            MeshFilter subfilter = Selection.transforms[t].GetComponent<MeshFilter>();
            if ((subrenderer == null) || (subfilter == null)) { continue; }
            mesh.subMeshCount += subfilter.sharedMesh.subMeshCount;
            for (int i = 0; i < subfilter.sharedMesh.subMeshCount; ++i)
            {
                submeshes.Add(subfilter.sharedMesh.GetSubMesh(i));
            }
            foreach (Material m in subrenderer.sharedMaterials)
            {
                materials.Add(new Material(m));
            }
            List<Vector3> verts = new List<Vector3>(subfilter.sharedMesh.vertices);
            for (int i = 0; i < verts.Count; ++i)
            {
                verts[i] = Selection.transforms[last].InverseTransformPoint(Selection.transforms[t].TransformPoint(verts[i]));
            }
            vertices.AddRange(verts);
            offsets.Add(vertices.Count);
            uvmaps.AddRange(new List<Vector2>(subfilter.sharedMesh.uv));
            for (int i = 0; i < subfilter.sharedMesh.subMeshCount; ++i)
            {
                List<int> tris = new List<int>();
                subfilter.sharedMesh.GetTriangles(tris, i);
                for (int j = 0; j < tris.Count; ++j) { tris[j] += (int)offsets[t]; }
                triangles.Add(tris);
            }
        }
        renderer.materials = materials.ToArray();
        mesh.vertices = vertices.ToArray();
        mesh.uv  = uvmaps.ToArray();
        for (int i = 0; i < mesh.subMeshCount; ++i)
        {
            mesh.SetTriangles(triangles[i], i);
        }
        mesh.RecalculateNormals();

        go.GetComponent<MeshFilter>().sharedMesh = mesh;
        go.transform.parent = Selection.transforms[last].parent;
        go.transform.localPosition = Selection.transforms[last].localPosition;
        go.transform.localRotation = Selection.transforms[last].localRotation;
        go.transform.localScale = Selection.transforms[last].localScale;
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Tools/Ripple Reparent")]
    static void RippleReparent(MenuCommand menuCommand)
    {
        Transform grandParent = Selection.transforms[0].parent.parent;
        List<Transform> rippleSelect = new List<Transform>();
        foreach (Transform parent in grandParent)
        {
            rippleSelect.Add(parent.GetChild(0));
        }
        foreach (Transform child in rippleSelect)
        {
            Transform tmp = child.parent;
            child.parent = grandParent;
            GameObject.DestroyImmediate(tmp.gameObject);
        }
    }

    [MenuItem("GameObject/Tools/Flip Normals")]
    static void FlipNormals(MenuCommand menuCommand)
    {
        List<Transform> undoList = new List<Transform>();
        foreach (Transform t in Selection.transforms)
        {
            undoList.Add(t);
        }
        Undo.RegisterCompleteObjectUndo(undoList.ToArray(), "Flip Normals");

        foreach (Transform t in Selection.transforms)
        {
            MeshFilter filter = t.GetComponent<MeshFilter>();
            if (filter != null)
            {  
                List<Vector3> normals = new List<Vector3>();
                filter.sharedMesh.GetNormals(normals);
                for (int i = 0; i < normals.Count; ++i)
                {
                    Debug.Log(normals[i]);
                    normals[i] = new Vector3(-1.0f * normals[i].x,
                                             -1.0f * normals[i].y,
                                             -1.0f * normals[i].z);
                }
                filter.sharedMesh.SetNormals(normals, 0, normals.Count);
            }
        }
    }

    [MenuItem("GameObject/Tools/Trim")]
    static void Trim(MenuCommand menuCommand)
    {
        foreach (Transform t in Selection.transforms)
        {
            Undo.RegisterCompleteObjectUndo(t, t.name + " Trim");
            long count = RecursiveTrim(t);
            Debug.Log("[TRIM]: " + t.name + " | Trimmed " + count.ToString() + " objects");
        }
    }

    static long RecursiveTrim(Transform t, long index = 0, long level = 0)
    {
        long i = 0;
        long count = 0;
        foreach (Transform subt in t)
        {
            count += RecursiveTrim(subt, i, level + 1);
            ++i;
        }
        if ((i == 0) && (index != 0))
        {
            Debug.Log("[TRIM]: " + t.name);
            GameObject.DestroyImmediate(t.gameObject);
            ++count;
        }
        return count;
    }
}

#endif
