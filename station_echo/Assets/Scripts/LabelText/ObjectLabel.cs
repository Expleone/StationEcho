using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ObjectLabel : MonoBehaviour
{
    public string labelText = "Platform";
    public Color labelColor = Color.white;
    public Vector3 labelOffset = new Vector3(0, 2f, 0);
}

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
