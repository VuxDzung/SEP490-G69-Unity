namespace SEP490G69
{
    using UnityEngine;

    public class EntityStatus 
    {
        private float _maxValue;
        private float _currentValue;
        private EStatusType _status;

        public EntityStatus(float maxValue, EStatusType status)
        {
            _maxValue = maxValue;
            _currentValue = maxValue;
            _status = status;
        }
    }
}