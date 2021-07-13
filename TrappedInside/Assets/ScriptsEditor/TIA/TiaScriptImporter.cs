using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

/// <summary>
/// Imports TIA script assets into the editor from files with the .tia extension.
/// </summary>
[ScriptedImporter(1, "tia")]
public class TiaScriptImporter : ScriptedImporter
{
    public const string IconAssetPath = "Assets/Bitmaps/Editor/tia_script_icon.png";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var content = File.ReadAllText(ctx.assetPath);
        var tiaScriptThumbnail = AssetDatabase.LoadAssetAtPath<Texture2D>(IconAssetPath);
        var script = TiaScript.Read(content);
        ctx.AddObjectToAsset("script", script, tiaScriptThumbnail);
        ctx.SetMainObject(script);
    }
}
