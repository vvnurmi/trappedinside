using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using ObjScale = ScalingParallax.ObjectScale;

[CustomEditor(typeof(ScalingParallax))]
public class ScalingParallaxEditor : Editor
{
    private const float MinScale = 0.01f;
    private const float MaxScale = 10.0f;

    private float farScale = 0.7f;
    private float nearScale = 1.1f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        farScale = ScaleField("Far Scale", farScale);
        nearScale = ScaleField("Near Scale", nearScale);

        if (GUILayout.Button("Populate scales from child tilemap renderers"))
            PopulateScales();
    }

    private float ScaleField(string name, float oldScale)
    {
        var newScale = EditorGUILayout.FloatField("Far Scale", oldScale);
        return Mathf.Clamp(newScale, MinScale, MaxScale);
    }

    private void PopulateScales()
    {
        ScalingParallax scalingParallax = (ScalingParallax)target;

        var children = scalingParallax.transform.Cast<Transform>();
        var scaleData = (
            from child in children
            let obj = child.gameObject
            let renderer = obj.GetComponent<TilemapRenderer>()
            where renderer != null
            let order = renderer.sortingOrder
            orderby order ascending
            select (obj, order))
            .ToArray();
        if (scaleData.Length == 0) return;

        var minOrder = scaleData[0].order;
        var maxOrder = scaleData[scaleData.Length - 1].order;
        scalingParallax.objectScales = (
            from x in scaleData
            let normalizedOrder = (x.order - minOrder) / (float)(maxOrder - minOrder)
            let scale = farScale + (nearScale - farScale) * normalizedOrder
            select new ObjScale { obj = x.obj, scale = scale })
            .ToArray();
    }
}
