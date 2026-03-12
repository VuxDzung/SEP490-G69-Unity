namespace SEP490G69.Training
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIFacilityElement : MonoBehaviour, IPooledObject
    {
        private Action<string> _onClick;

        [SerializeField] private TextMeshProUGUI m_FacilityNameTmp;
        [SerializeField] private TextMeshProUGUI m_LevelTmp;
        [SerializeField] private Button m_BtnRef;

        private string _id;

        public void Spawn()
        {
            if (m_BtnRef != null) m_BtnRef.onClick.AddListener(Click);
        }
        public void Despawn()
        {
            if (m_BtnRef != null) m_BtnRef.onClick.RemoveListener(Click);
        }

        public UIFacilityElement SetOnClickCallback(Action<string> action)
        {
            _onClick = action;
            return this;
        }

        public UIFacilityElement SetContent(string facilityId, string facilityName, int level)
        {
            _id = facilityId;
            m_FacilityNameTmp.text = facilityName;
            m_LevelTmp.text = $"Lv {level}";
            return this;
        }

        private void Click()
        {
            _onClick?.Invoke(_id);
        }
    }
}