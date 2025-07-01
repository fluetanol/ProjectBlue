using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ConditionFieldName;
    public int ConditionValue;

    public ConditionalFieldAttribute(string conditionFieldName, int conditionValue)
    {
        ConditionFieldName = conditionFieldName;
        ConditionValue = conditionValue;
    }
}