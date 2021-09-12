using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Planet obj = target as Planet;
        float radius = Planet.GravityRadius(obj);

        obj.GetComponentsInChildren<CircleCollider2D>()[1].radius = radius;
        Planet.GravityGlow(obj).transform.localScale = new Vector3(radius, radius, 1f);

        EditorUtility.SetDirty(obj);
    }
}
