#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;


[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute showIf = (ConditionalFieldAttribute)attribute;

        // 부모 경로를 추적하여 올바른 enum 필드를 가져옴
        string parentPath = property.propertyPath.Substring(0, property.propertyPath.LastIndexOf('.'));
        SerializedProperty parentProperty = property.serializedObject.FindProperty(parentPath);
        SerializedProperty enumField = parentProperty?.FindPropertyRelative(showIf.ConditionFieldName);

        if (enumField != null && enumField.enumValueIndex == showIf.ConditionValue)
        {
            // RangeAttribute 체크 및 슬라이더 렌더링
            RangeAttribute range = fieldInfo.GetCustomAttribute<RangeAttribute>();
            if (range != null && property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, range.min, range.max);
            }
            else if (range != null && property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntSlider(position, label, property.intValue, (int)range.min, (int)range.max);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute showIf = (ConditionalFieldAttribute)attribute;

        string parentPath = property.propertyPath.Substring(0, property.propertyPath.LastIndexOf('.'));
        SerializedProperty parentProperty = property.serializedObject.FindProperty(parentPath);
        SerializedProperty enumField = parentProperty?.FindPropertyRelative(showIf.ConditionFieldName);

        if (enumField != null && enumField.enumValueIndex == showIf.ConditionValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        return 0f; // 숨김 처리
    }
}
#endif
