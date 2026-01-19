namespace SEP490G69
{
    using System.Linq;
    using UnityEngine;

    public static class InspectorParameterExtentions 
    {
        public static ParameterInspectorData GetParameter(this ParameterInspectorData[] data, string paramName)
        {
            return data.FirstOrDefault(p => p.ParamName.Equals(paramName));
        }
    }
}