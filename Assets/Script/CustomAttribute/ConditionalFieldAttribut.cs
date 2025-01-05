using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ConditionFieldName;
    public int ConditionValue;

    public ConditionalFieldAttribute(string conditionFieldName, int conditionValue)
    {
        Debug.Log(conditionFieldName);
        ConditionFieldName = conditionFieldName;
        ConditionValue = conditionValue;
    }
}