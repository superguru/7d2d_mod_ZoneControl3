using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportUnity3DBundle
{
    [MenuItem("Assets/Build Unity3D export bundle")]
    static void ExportResource()
    {
        // 1. Get the path to save the asset bundle
        string path = EditorUtility.SaveFilePanel("Save AssetBundle", "", "NewAssetBundle2", "unity3d");
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("AssetBundle export cancelled by user.");
            return;
        }

        // 2. Get the list of selected assets, filtering out scripts
        Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        
        System.Collections.Generic.List<string> assetPaths = new System.Collections.Generic.List<string>();
        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (!assetPath.EndsWith(".cs"))
            {
                assetPaths.Add(assetPath);
            }
            else
            {
                Debug.LogWarning($"Skipping script asset: {assetPath}");
            }
        }
        
        if (assetPaths.Count == 0)
        {
            Debug.LogError("No valid assets selected to build into an AssetBundle. Scripts cannot be included.");
            return;
        }

        // Log the assets that will be included in the bundle
        Debug.Log("Building AssetBundle with the following assets:");
        foreach (string ap in assetPaths)
        {
            Debug.Log($"- {ap}");
        }

        // 3. Define the AssetBundleBuild object
        AssetBundleBuild build = new AssetBundleBuild
        {
            assetBundleName = Path.GetFileName(path),
            assetNames = assetPaths.ToArray()
        };

        // 4. Create the build map array
        AssetBundleBuild[] buildMap = new AssetBundleBuild[] { build };

        // 5. Ensure the output directory exists
        string outputDir = Path.GetDirectoryName(path);
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // 6. Build the AssetBundles
        BuildPipeline.BuildAssetBundles(outputDir, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log("AssetBundle built successfully to " + path);
    }
}