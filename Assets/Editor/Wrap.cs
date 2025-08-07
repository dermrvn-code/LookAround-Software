using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class WrapNamespace : EditorWindow
{
    private static string folderPath = "Assets/LookAroundCore/Runtime/Scripts";
    private static string targetNamespace = "LookAroundCore";

    [MenuItem("Tools/Wrap in Namespace/Wrap LookAroundCore Scripts")]
    public static void WrapLookAroundCoreScripts()
    {
        WrapAllCsFiles(folderPath, targetNamespace);
    }

    static void WrapAllCsFiles(string path, string ns)
    {
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file).ToList();

            // Skip if already in a namespace
            if (lines.Any(l => l.TrimStart().StartsWith("namespace ")))
            {
                Debug.Log($"Skipped (already wrapped): {file}");
                continue;
            }

            int insertAfter = 0;
            while (insertAfter < lines.Count && lines[insertAfter].TrimStart().StartsWith("using "))
                insertAfter++;

            // Build new content
            var result = new List<string>();
            result.AddRange(lines.Take(insertAfter));
            result.Add(""); // spacing
            result.Add($"namespace {ns}");
            result.Add("{");

            string indent = "    ";
            for (int i = insertAfter; i < lines.Count; i++)
            {
                result.Add(indent + lines[i]);
            }

            result.Add("}");

            File.WriteAllLines(file, result);
            Debug.Log($"Wrapped: {file}");
        }

        AssetDatabase.Refresh();
    }
}
