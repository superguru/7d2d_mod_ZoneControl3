using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MeshDataEntry
{
    public List<float> vertices;
    public List<int> indices;
}

public class MeshJSONImporter
{
    [MenuItem("Assets/Import Meshes JSON")]
    public static void ImportMeshes()
    {
        string jsonPath = "Assets/meshes.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"meshes.json not found at: {jsonPath}");
            return;
        }

        string jsonText = File.ReadAllText(jsonPath);

        // Configure Newtonsoft to handle comments and case-insensitive keys
        var settings = new JsonSerializerSettings
        {
            //CommentHandling = MetalCommentHandling.Skip, // Safely ignore comments
            //PropertyNameCaseInsensitive = true
        };

        // If your Newtonsoft version uses alternative comment handling settings:
        // (Newtonsoft ignores comments by default in standard parsing modes unless strict mode is enabled, 
        // but you can ensure lenient reading like this:)
        settings.MissingMemberHandling = MissingMemberHandling.Ignore;

        Dictionary<string, MeshDataEntry> meshes;
        try
        {
            meshes = JsonConvert.DeserializeObject<Dictionary<string, MeshDataEntry>>(jsonText, settings);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse meshes.json: {ex.Message}");
            return;
        }

        if (meshes == null)
        {
            Debug.LogError("Parsed result is empty.");
            return;
        }

        foreach (var kvp in meshes)
        {
            string meshName = kvp.Key;
            var data = kvp.Value;

            int vertCount = data.vertices.Count / 8; // x,y,z, nx,ny,nz, u,v
            Vector3[] positions = new Vector3[vertCount];
            Vector3[] normals = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];

            for (int i = 0; i < vertCount; i++)
            {
                int baseIdx = i * 8;
                positions[i] = new Vector3(data.vertices[baseIdx], data.vertices[baseIdx + 1], data.vertices[baseIdx + 2]);
                normals[i] = new Vector3(data.vertices[baseIdx + 3], data.vertices[baseIdx + 4], data.vertices[baseIdx + 5]);
                uvs[i] = new Vector2(data.vertices[baseIdx + 6], data.vertices[baseIdx + 7]);
            }

            int[] indices = data.indices.ToArray();

            Mesh mesh = new Mesh();
            bool needsUInt32 = indices.Length > 0 && indices[^1] >= 65535;
            mesh.indexFormat = needsUInt32 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;

            mesh.SetVertices(positions);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            string assetPath = $"Assets/{meshName}.asset";
            AssetDatabase.CreateAsset(mesh, assetPath);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Successfully imported {meshes.Count} meshes from meshes.json");
    }
}