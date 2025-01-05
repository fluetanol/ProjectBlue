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
        // 대상 클래스의 enum 필드 값을 가져옴
        SerializedProperty enumField = property.serializedObject.FindProperty(showIf.ConditionFieldName);

        if (enumField != null && enumField.enumValueIndex == showIf.ConditionValue)
        {
            // RangeAttribute 체크
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
        SerializedProperty enumField = property.serializedObject.FindProperty(showIf.ConditionFieldName);

        if (enumField != null && enumField.enumValueIndex == showIf.ConditionValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        // 필드 숨김 시 높이를 0으로 설정
        return 0f;
    }
}
#endif
