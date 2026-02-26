namespace SEP490G69.Training
{
    using SEP490G69.Addons.Localization;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIExerciseElement : MonoBehaviour, IPooledObject
    {
        private Action<string> _onClick;

        [SerializeField] private Image m_Image;
        [SerializeField] private TextMeshProUGUI m_LevelTmp;
        [SerializeField] private TextMeshProUGUI m_ExerciseNameTmp;
        [SerializeField] private Button m_Btn;

        private string _id;

        private LocalizationManager _localizeManager;
        private LocalizationManager LocalizeManager
        {
            get
            {
                if (_localizeManager == null)
                {
                    _localizeManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();  
                }
                return _localizeManager;
            }
        }

        public void Spawn()
        {
            m_Btn.onClick.AddListener(Click);
        }

        public void Despawn()
        {
            m_Btn.onClick.RemoveListener(Click);
        }

        public UIExerciseElement SetOnClick(Action<string> onClick)
        {
            _onClick = onClick;
            return this;
        }

        public void SetContent(string id, Sprite image, string nameKey, int level)
        {
            _id  = id;
            m_Image.sprite = image;
            m_LevelTmp.text = $"LV: {level}";
            Debug.Log(nameKey);
            m_ExerciseNameTmp.text = LocalizeManager.GetText("ExerciseNames", nameKey);
        }

        private void Click()
        {
            _onClick?.Invoke(_id);
        }
    }
}