using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Vector2IntConditionAttribute))]
public class Vector2IntAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Debug log to check if the drawer is being called
        Debug.Log("Vector2IntConditionDrawer.OnGUI called");

        // Ensure the property is of type Vector2Int
        if (property.propertyType == SerializedPropertyType.Vector2Int)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get the current values of x and y
            SerializedProperty xProp = property.FindPropertyRelative("x");
            SerializedProperty yProp = property.FindPropertyRelative("y");
            

            Vector2Int currentValue = new Vector2Int(xProp.intValue, yProp.intValue);

            // Draw the default Vector2Int field
            Vector2Int newValue = EditorGUI.Vector2IntField(position, label, currentValue);

            // Check if y >= x
            if (newValue.y >= newValue.x)
            {
                // If valid, update the property values
                xProp.intValue = newValue.x;
                yProp.intValue = newValue.y;
            }
            else
            {
                // Display a warning message
                EditorGUI.HelpBox(position, "y must be greater or equal than x", MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use Vector2IntCondition with Vector2Int.");
        }
    }
}
