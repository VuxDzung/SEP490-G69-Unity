namespace SEP490G69
{
    using UnityEngine;

    [System.Serializable]
    public class InspectorPrimtiveVariable
    {
        [SerializeField] private string m_VariableName;
        [SerializeField] private EPrimitiveDataType m_DataType;
        [SerializeField] private string m_StringValue;

        public string VariableName => m_VariableName;
        public EPrimitiveDataType DataType => m_DataType;

        public object GetValue()
        {
            switch (m_DataType)
            {
                case EPrimitiveDataType.String:
                    return m_StringValue;

                case EPrimitiveDataType.Boolean:
                    return bool.Parse(m_StringValue);

                case EPrimitiveDataType.Integer:
                    return int.Parse(m_StringValue);

                case EPrimitiveDataType.Float:
                    return float.Parse(m_StringValue);

                case EPrimitiveDataType.Double:
                    return double.Parse(m_StringValue);

                case EPrimitiveDataType.Long:
                    return long.Parse(m_StringValue);

                default:
                    Debug.LogError($"Unsupported datatype {m_DataType}");
                    return null;
            }
        }

        public T GetValue<T>()
        {
            object value = GetValue();

            if (value is T casted)
                return casted;

            throw new System.InvalidCastException(
                $"Variable {m_VariableName} is {m_DataType} but requested {typeof(T)}"
            );
        }

        public bool TryGetValue<T>(out T value)
        {
            value = default;

            try
            {
                object raw = GetValue();

                if (raw is T casted)
                {
                    value = casted;
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public enum EPrimitiveDataType
    {
        String = 0,
        Boolean = 1,
        Integer = 2,
        Float = 3,  
        Double = 4,
        Long = 5,
    }
}