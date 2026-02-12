namespace SEP490G69.Shared
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILinearSwitcher : MonoBehaviour
    {
        public Action<int> OnChanged;

        [SerializeField] private TextMeshProUGUI m_ContentTmp;
        [SerializeField] private Button m_PreBtn;
        [SerializeField] private Button m_NextBtn;

        private int _currentIndex;

        private List<string> _contentList = new List<string>();

        public void SetContents(string[] contents, int starterIndex = 0)
        {
            _contentList.Clear();
            _contentList.AddRange(contents);    
            _currentIndex = starterIndex;
        }

        public void Enable()
        {
            m_PreBtn.onClick.AddListener(PreContent);
            m_NextBtn.onClick.AddListener(NextContent);
        }
        public void Disable()
        {
            m_PreBtn.onClick.RemoveListener(PreContent);
            m_NextBtn.onClick.RemoveListener(NextContent);
        }

        private void PreContent()
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _contentList.Count - 1;
            }
            DisplayCurrentContent();
        }
        private void NextContent()
        {
            _currentIndex++;
            if (_currentIndex > _contentList.Count - 1)
            {
                _currentIndex = 0;
            }
            DisplayCurrentContent();
        }

        private void DisplayCurrentContent()
        {
            m_ContentTmp.text = _contentList[_currentIndex].ToString();
        }
    }
}