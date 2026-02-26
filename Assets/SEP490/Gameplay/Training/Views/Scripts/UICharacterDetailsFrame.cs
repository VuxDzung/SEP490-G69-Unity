namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterDetailsFrame : GameUIFrame
    {
        [SerializeField] private Button m_Back;

        [SerializeField] private UITextSlider m_VitSlider;
        [SerializeField] private UITextSlider m_PowerSlider;
        [SerializeField] private UITextSlider m_AgiSlider;
        [SerializeField] private UITextSlider m_INTSlider;
        [SerializeField] private UITextSlider m_StaminaSlider;

        [SerializeField] private Button m_PassiveBtnPrefab;

        [SerializeField] private TextMeshProUGUI m_CharacterNameTmp;
        [SerializeField] private Image m_CharacterImg;

        private GameTrainingController _trainingController;

        private GameTrainingController TrainingController
        {
            get
            {
                if (_trainingController == null)
                {
                    ContextManager.Singleton.TryResolveSceneContext(out _trainingController);
                }
                return _trainingController;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_Back.onClick.AddListener(Back);
            LoadCharacterDetails(TrainingController.CharacterData);
            LoadCharacterStats();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_Back.onClick.RemoveListener(Back);
        }

        private void LoadCharacterDetails(CharacterDataHolder characterHolder)
        {
            m_CharacterNameTmp.text = characterHolder.GetCharacterName();
            m_CharacterImg.sprite = characterHolder.GetFullBodyImg();

            LoadPassives(null);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_MAIN_MENU);
        }

        private void LoadPassives(List<string> passiveIdList)
        {
            if (passiveIdList == null || passiveIdList.Count == 0)
            {
                return;
            }
        }

        private void LoadCharacterStats()
        {
            SetVitality(TrainingController.CharacterData.GetVIT(), GameConstants.MAX_STAT_VALUE);
            SetPower(TrainingController.CharacterData.GetPower(), GameConstants.MAX_STAT_VALUE);
            SetINT(TrainingController.CharacterData.GetINT(), GameConstants.MAX_STAT_VALUE);
            SetAgility(TrainingController.CharacterData.GetAgi(), GameConstants.MAX_STAT_VALUE);
            SetStamina(TrainingController.CharacterData.GetStamina(), GameConstants.MAX_STAT_VALUE);
        }


        public void SetVitality(float cur, float max)
        {
            m_VitSlider.SetValue(cur, max);
        }
        public void SetPower(float cur, float max)
        {
            m_PowerSlider.SetValue(cur, max);
        }
        public void SetAgility(float cur, float max)
        {
            m_AgiSlider.SetValue(cur, max);
        }
        public void SetINT(float cur, float max)
        {
            m_INTSlider.SetValue(cur, max);
        }
        public void SetStamina(float cur, float max)
        {
            m_StaminaSlider.SetValue(cur, max);
        }
    }
}