namespace SEP490G69
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class DialogEvent : IEvent
    {
        public string Receiver { get; set; }
        public string Action { get; set; }
        public ParameterInspectorData[] Parameters { get; set; }
    }

    public class NarrativeManager : MonoBehaviour, IGameContext
    {
        [SerializeField] private float m_AutoDeltaTime = 0.5f;
        [SerializeField] private DialogTreeConfigSO m_Config;

        private NarrativeEventDispatcher _eventDispatcher;

        /// <summary>
        /// A reference of EventManager use to publish, listen to event(s).
        /// </summary>
        private EventManager _eventManager;

        private DialogTreeSO _currentTree;
        private BaseDialogNodeSO _currentNode;

        private bool _auto;
        private Coroutine _autoCoroutine;

        private void OnDisable()
        {
            _eventManager.Unsubscribe<NextDialogEvent>(DispatchNextEvent);
            _eventManager.Unsubscribe<AutoPlayDialogEvent>(DispatchAutoEvent);
            _eventManager.Unsubscribe<SkipDialogEvent>(DispatchSkipEvent);
            _eventManager.Unsubscribe<SelectChoice>(DispatchChoiceSelectionEvent);
        }

        public void SetManager(ContextManager manager)
        {
            _eventManager = manager.ResolveGameContext<EventManager>();

            if (manager == null) return;

            _eventDispatcher = new NarrativeEventDispatcher(new INarrativeActionHandler[]
            {
                new FadeInHandler(manager),
                new FadeIn2OutHandler(manager),
                //new FadeOutHandler(dialogManager),
                // new PlayCutsceneHandler(...)
            });

            _eventManager.Subscribe<NextDialogEvent>(DispatchNextEvent);
            _eventManager.Subscribe<AutoPlayDialogEvent>(DispatchAutoEvent);
            _eventManager.Subscribe<SkipDialogEvent>(DispatchSkipEvent);
            _eventManager.Subscribe<SelectChoice>(DispatchChoiceSelectionEvent);
        }

        private void DispatchNextEvent(NextDialogEvent nextEvent)
        {
            Next();
        }
        private void DispatchAutoEvent(AutoPlayDialogEvent autoEvent)
        {
            ToggleAuto();
        }
        private void DispatchSkipEvent(SkipDialogEvent skipEvent)
        {
            Skip();
        }
        private void DispatchChoiceSelectionEvent(SelectChoice choiceEvent)
        {
            string choiceID = choiceEvent.ChoiceID;

            // Select choice event.
            SelectChoice(choiceID);
        }

        public void SelectChoice(string choiceID)
        {
            if (_currentNode is ChoiceNodeSO choiceNode)
            {
                BaseDialogNodeSO nextNode = choiceNode.GetChoice(choiceID)?.NextNode;
                _currentNode = nextNode;
                RenderNode(_currentNode);
            }
        }

        /// <summary>
        /// Start play the dialog tree 
        /// </summary>
        /// <param name="treeId">The id of dialog tree</param>
        /// <param name="starterNodeId">The id of the dialog which you want to start from</param>
        public void StartTree(string treeId, string starterNodeId = "")
        {
            _currentTree = m_Config.GetTree(treeId);

            if (_currentTree == null)
            {
                LoggerUtils.Logging("DIALOG_MANAGER_ERROR", $"Dialog tree {treeId} is missing", TextColor.Red);
                return;
            }

            _currentNode = string.IsNullOrEmpty(starterNodeId)
                ? _currentTree.GetStarterNode()
                : _currentTree.GetNode(starterNodeId);

            if (_currentNode == null)
            {
                Debug.LogError("Starter node not found");
                return;
            }

            GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_DIALOG);

            ResolveEventChain();
            RenderNode(_currentNode);
        }

        /// <summary>
        /// Display/Go to the next dialog node
        /// If the current and its next node are event node, loop and trigger a sequence of event until the next node is not event node.
        /// </summary>
        public void Next()
        {
            if (_currentNode == null)
                return;

            // Không cho Next khi đang ở ChoiceNode
            if (_currentNode is ChoiceNodeSO)
                return;

            if (_currentNode is LinearNodeSO linearNode)
            {
                _currentNode = linearNode.NextNode;
            }

            // Resolve all consecutive event nodes
            ResolveEventChain();

            RenderNode(_currentNode);
        }

        /// <summary>
        /// Toggle auto go to next node
        /// </summary>
        public void ToggleAuto()
        {
            _auto = !_auto;

            if (_auto)
            {
                _autoCoroutine = StartCoroutine(AutoPlay());
            }
            else if (_autoCoroutine != null)
            {
                StopCoroutine(_autoCoroutine);
            }
        }

        /// <summary>
        /// Skip to the end/branch node.
        /// </summary>
        public void Skip()
        {
            while (_currentNode != null)
            {
                if (_currentNode is LinearNodeSO linear && linear.NextNode != null)
                {
                    _currentNode = linear.NextNode;
                    ResolveEventChain();
                    continue;
                }

                if (_currentNode is ChoiceNodeSO)
                    break;

                break;
            }

            RenderNode(_currentNode);
        }

        private IEnumerator AutoPlay()
        {
            while (_auto)
            {
                yield return new WaitForSeconds(m_AutoDeltaTime);
                Next();
            }
        }

        /// <summary>
        /// Resolve consecutive event nodes starting from current node.
        /// Trigger all event nodes until reaching a non-event node.
        /// </summary>
        private void ResolveEventChain()
        {
            while (_currentNode is EventNodeSO eventNode)
            {
                DialogEvent _event = new DialogEvent
                {
                    Receiver = eventNode.Receiver,
                    Action = eventNode.Action,
                    Parameters = eventNode.Parameters
                };
                _eventManager.Publish(_event);
                _eventDispatcher.Dispatch(_event);

                _currentNode = eventNode.NextNode;

                if (_currentNode == null)
                    break;
            }
        }

        private void EndDialog()
        {
            _auto = false;
            if (_autoCoroutine != null)
                StopCoroutine(_autoCoroutine);

            GameUIManager.Singleton.HideFrame(GameConstants.FRAME_ID_DIALOG);
        }

        private void RenderNode(BaseDialogNodeSO node)
        {
            if (node == null)
            {
                EndDialog();
                return;
            }

            UIVSDialogFrame frame = GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_DIALOG)
                                   .AsFrame<UIVSDialogFrame>()
                                   .RenderDialog(node.SpeakerID, node.DialogID, node.BackgroundImage);

            if (node is ChoiceNodeSO choiceNode)
            {
                frame.ShowChoices(choiceNode.Choices);
            }
        }

        public bool HasChoice(BaseDialogNodeSO node)
        {
            return node is ChoiceNodeSO;
        }
    }
}