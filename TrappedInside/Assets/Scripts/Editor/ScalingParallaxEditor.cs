using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.Tilemaps;

using ObjScale = ScalingParallax.ObjectScale;

[CustomEditor(typeof(ScalingParallax))]
public class ScalingParallaxEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Populate scales from child tilemap renderers"))
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
            if (scaleData.Length > 0)
            {
                var minOrder = scaleData[0].order;
                var maxOrder = scaleData[scaleData.Length - 1].order;
                var deepScale = 0.7f;
                var shallowScale = 1.0f;
                scalingParallax.objectScales = (
                    from x in scaleData
                    let normalizedOrder = (x.order - minOrder) / (float)(maxOrder - minOrder)
                    let scale = deepScale + (shallowScale - deepScale) * normalizedOrder
                    select new ObjScale { obj = x.obj, scale = scale })
                    .ToArray();
            }
        }
    }
}
