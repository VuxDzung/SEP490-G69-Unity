namespace SEP490G69
{
    using System.Collections.Generic;
    using UnityEngine;

    [DefaultExecutionOrder(-100)]
    public class GameUIManager : GlobalSingleton<GameUIManager>
    {
        #region Serialize Fields
        [SerializeField] private string starterFrameId;
        [SerializeField] private Transform frameContainer;
        [SerializeField] private List<GameUIFrame> framePrefabList;
        #endregion

        #region Private fields
        private readonly Dictionary<string, GameUIFrame> frameUILookUp = new Dictionary<string, GameUIFrame>();
        private readonly Dictionary<string, GameUIFrame> activeFrameLookUp = new Dictionary<string, GameUIFrame>();
        #endregion

        #region Unity Life Cycle methods
        /// <summary>
        /// When the SimpleUIManager initialized, 
        /// it spawns all the UI Frame prefab to scene by default and 
        /// then display the default/starter frame.
        /// </summary>
        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();
            foreach (var frame in framePrefabList)
            {
                GameUIFrame frameUI = Instantiate(frame, frameContainer);
                frameUILookUp.Add(frame.FrameId, frameUI);
                frameUI.gameObject.SetActive(false);
                frameUI.SetManager(this);
            }

            if (!string.IsNullOrEmpty(starterFrameId))
            {
                ShowFrame(starterFrameId);
            }
        }

        #endregion

        #region UI Frame Management methods
        /// <summary>
        /// Show the frame by id
        /// </summary>
        /// <param name="frameId">The frame id you want to show</param>
        /// <returns>The SimpleUIFrame base class of the frame</returns>
        public GameUIFrame ShowFrame(string frameId)
        {
            if (activeFrameLookUp.ContainsKey(frameId))
            {
                return activeFrameLookUp[frameId];
            }
            if (frameUILookUp.ContainsKey(frameId))
            {
                GameUIFrame frame = frameUILookUp[frameId];
                activeFrameLookUp.Add(frameId, frame);
                frame.Show();
                return frame;
            }
            return null;
        }

        /// <summary>
        /// Hide the frame by id
        /// </summary>
        /// <param name="frameId">The frame id you want to hide</param>
        /// <returns>The SimpleUIFrame base class of the frame</returns>
        public GameUIFrame HideFrame(string frameId)
        {
            if (activeFrameLookUp.ContainsKey(frameId))
            {
                GameUIFrame frame = activeFrameLookUp[frameId];
                activeFrameLookUp.Remove(frameId);
                frame.Hide();
                return frame;
            }
            return null;
        }

        /// <summary>
        /// Hide all active frames.
        /// </summary>
        public void HideAll()
        {
            foreach (var frame in activeFrameLookUp)
            {
                frame.Value.gameObject.SetActive(false);
            }
            activeFrameLookUp.Clear();
        }

        /// <summary>
        /// Get an active frames.
        /// </summary>
        /// <param name="frameId"></param>
        /// <returns></returns>
        public GameUIFrame GetActiveFrame(string frameId)
        {
            if (activeFrameLookUp.TryGetValue(frameId, out var frame))
            {
                return frame;
            }
            Debug.LogError($"There's no active frame {frameId} at the moment.");
            return null;
        }

        /// <summary>
        /// Get a frame by id.
        /// </summary>
        /// <param name="frameId"></param>
        /// <returns></returns>
        public GameUIFrame GetFrame(string frameId)
        {
            if (frameUILookUp.TryGetValue(frameId, out var frame))
            {
                return frame;
            }
            Debug.LogError($"Frame {frameId} has not been initialized yet!");
            return null;
        }
        #endregion
    }
}