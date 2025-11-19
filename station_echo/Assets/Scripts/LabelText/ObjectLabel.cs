using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ObjectLabel : MonoBehaviour
{
    public string labelText = "Platform";
    public Color labelColor = Color.white;
    public Vector3 labelOffset = new Vector3(0, 2f, 0);
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectLabel))]
public class ObjectLabelEditor : Editor
{
    void OnSceneGUI()
    {
        ObjectLabel obj = (ObjectLabel)target;
        Handles.color = obj.labelColor;
        Handles.Label(
            obj.transform.position + obj.labelOffset,
            obj.labelText,
            new GUIStyle
            {
                fontSize = 12,
                normal = { textColor = obj.labelColor },
                fontStyle = FontStyle.Bold
            }
        );
    }
}
#endif