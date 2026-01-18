namespace SEP490G69
{
    using System;
    using UnityEngine;

    public enum EParamDataType
    {
        String = 0,
        Integer = 1,
        Float = 2,
        Boolean = 3,
    }

    [System.Serializable]
    public class ParameterInspectorData 
    {
        [SerializeField] private string m_ParameterName;
        [SerializeField] private EParamDataType m_ParameterType;
        [SerializeField] private string m_Value;
        public string ParamName => m_ParameterName;
        public EParamDataType ParameterType => m_ParameterType;
        public bool GetBooleanValue()
        {
            try
            {
                if (m_ParameterType == EParamDataType.Boolean)
                {
                    return bool.Parse(m_Value);
                }
                return false;
            }
            catch { return false; }
        }
        public string GetStringValue()
        {
            return m_Value;
        }
        public int GetIntValue()
        {
            try
            {
                if (m_ParameterType == EParamDataType.Integer)
                {
                    return int.Parse(m_Value);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return 0;
            }
        }
        public float GetFloatValue()
        {
            try
            {
                if (m_ParameterType == EParamDataType.Float)
                {
                    return float.Parse(m_Value);
                }
                return 0f;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return 0f;
            }
        }
    }
}