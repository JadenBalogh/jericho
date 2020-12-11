using UnityEditor;
using UnityEngine;

[System.Serializable]
public class FloatRange
{
    public float min;
    public float max;
}

// IntRangeDrawer
[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangeDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Create a new label and add a tooltip - for use in PrefixLabel only
        GUIContent prefixLabel = new GUIContent(label.text, "A range between floats [min, max]");

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), prefixLabel);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rect
        var rect = new Rect(position.x, position.y, position.width, position.height);

        // Calculate contents
        GUIContent[] contents = { new GUIContent(), new GUIContent() };

        // Draw MultiPropertyField
        EditorGUI.MultiPropertyField(rect, contents, property.FindPropertyRelative("min"));

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
