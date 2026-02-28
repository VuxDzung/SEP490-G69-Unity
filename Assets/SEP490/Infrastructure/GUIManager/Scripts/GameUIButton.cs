namespace SEP490G69
{
    using UnityEngine;
    using UnityEngine.UI;

    public class GameUIButton : MonoBehaviour
    {
        [SerializeField] private Button m_ButtonRef;
        [SerializeField] private string m_SFXId;
        [SerializeField] private Vector2 m_NormalScale = Vector2.one;
        [SerializeField] private Vector2 m_HighlightedScale = Vector2.one;
        [SerializeField] private Vector2 m_SelectedScale = Vector2.one;

        private void OnEnable()
        {
            if (m_ButtonRef != null)
            {
                
            }
        }
        private void OnDisable()
        {
            
        }
    }
}