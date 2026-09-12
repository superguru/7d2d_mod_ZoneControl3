using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneControl.Game.ZoneClaim;

internal static class ZoneClaimBoundsHelper
{
    private const float Height = 10000f;

    private sealed class Entry
    {
        public Vector3i BlockPos;
        public Vector3 Center;
        public Transform Helper;

        public Entry(Vector3i _blockPos, Vector3 _center, Transform _helper)
        {
            BlockPos = _blockPos;
            Center = _center;
            Helper = _helper;
            Origin.OriginChanged = (Action<Vector3>)Delegate.Combine(Origin.OriginChanged, new Action<Vector3>(OnOriginChanged));
        }

        private void OnOriginChanged(Vector3 _newOrigin)
        {
            Helper.localPosition = Center - Origin.position;
        }

        public void Remove()
        {
            Origin.OriginChanged = (Action<Vector3>)Delegate.Remove(Origin.OriginChanged, new Action<Vector3>(OnOriginChanged));
        }
    }

    private static Transform s_goRoot;
    private static Transform s_goPool;
    private static readonly List<Entry> s_entries = [];

    internal static Transform GetBoundsHelper(Vector3i _blockPos, Vector3 _center, Vector3 _size, Material _material)
    {
        InitHelpers();

        var entry = FindEntry(_blockPos);
        if (entry == null)
        {
            entry = new Entry(_blockPos, _center, CreateHelper());
            s_entries.Add(entry);
        }

        entry.Center = _center;
        entry.Helper.localPosition = _center - Origin.position;
        entry.Helper.localScale = new Vector3(_size.x, Height, _size.z);
        ApplyMaterial(entry.Helper, _material);
        return entry.Helper;
    }

    internal static void RemoveBoundsHelper(Vector3i _blockPos)
    {
        for (int i = 0; i < s_entries.Count; i++)
        {
            if (s_entries[i].BlockPos != _blockPos)
            {
                continue;
            }

            var entry = s_entries[i];
            entry.Remove();
            entry.Helper.parent = s_goPool;
            entry.Helper.localPosition = Vector3.zero;
            entry.Helper.gameObject.SetActive(false);
            s_entries.RemoveAt(i);
            return;
        }
    }

    internal static void CleanupHelpers()
    {
        for (int i = 0; i < s_entries.Count; i++)
        {
            s_entries[i].Remove();
            UnityEngine.Object.Destroy(s_entries[i].Helper.gameObject);
        }
        s_entries.Clear();
    }

    private static Entry FindEntry(Vector3i _blockPos)
    {
        for (int i = 0; i < s_entries.Count; i++)
        {
            if (s_entries[i].BlockPos == _blockPos)
            {
                return s_entries[i];
            }
        }
        return null;
    }

    private static Transform CreateHelper()
    {
        if (s_goPool.childCount > 0)
        {
            var pooled = s_goPool.GetChild(0);
            pooled.parent = s_goRoot;
            return pooled;
        }

        var root = new GameObject("ZoneClaimBoundary");
        var cube = new GameObject("Cube");
        cube.transform.parent = root.transform;
        cube.transform.localScale = Vector3.one;
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localRotation = Quaternion.identity;

        var filter = cube.AddComponent<MeshFilter>();
        filter.mesh = CreateDoubleSidedCubeMesh();
        cube.AddComponent<MeshRenderer>();

        root.transform.parent = s_goRoot;
        return root.transform;
    }

    private static Mesh CreateDoubleSidedCubeMesh()
    {
        var template = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var source = template.GetComponent<MeshFilter>().sharedMesh;

        var sourceVertices = source.vertices;
        var sourceNormals = source.normals;
        var sourceUvs = source.uv;
        var sourceTriangles = source.triangles;

        int vertexCount = sourceVertices.Length;

        // Front half uses the original vertices; back half duplicates them with flipped normals.
        var vertices = new Vector3[vertexCount * 2];
        var normals = new Vector3[vertexCount * 2];
        var uvs = new Vector2[vertexCount * 2];

        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = sourceVertices[i];
            normals[i] = sourceNormals[i];
            uvs[i] = sourceUvs[i];

            vertices[i + vertexCount] = sourceVertices[i];
            normals[i + vertexCount] = -sourceNormals[i];
            uvs[i + vertexCount] = sourceUvs[i];
        }

        var triangles = new int[sourceTriangles.Length * 2];

        int outputIndex = 0;
        for (int i = 0; i < sourceTriangles.Length; i += 3)
        {
            int a = sourceTriangles[i];
            int b = sourceTriangles[i + 1];
            int c = sourceTriangles[i + 2];

            // Front face (outward winding, outward normals).
            triangles[outputIndex++] = a;
            triangles[outputIndex++] = b;
            triangles[outputIndex++] = c;

            // Back face (inward winding, inward normals).
            triangles[outputIndex++] = a + vertexCount;
            triangles[outputIndex++] = c + vertexCount;
            triangles[outputIndex++] = b + vertexCount;
        }

        var mesh = new Mesh
        {
            vertices = vertices,
            normals = normals,
            uv = uvs,
            triangles = triangles
        };
        mesh.RecalculateBounds();

        UnityEngine.Object.Destroy(template);
        return mesh;
    }

    private static void ApplyMaterial(Transform _helper, Material _material)
    {
        if (_material == null)
        {
            return;
        }

        var renderers = _helper.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = _material;
        }
    }

    private static void InitHelpers()
    {
        if (s_goRoot == null)
        {
            s_goRoot = new GameObject("ZoneClaimHelpers").transform;
            s_goPool = new GameObject("Pool").transform;
            s_goPool.parent = s_goRoot;
            s_goPool.localPosition = new Vector3(9999f, 9999f, 9999f);
        }
    }
}
