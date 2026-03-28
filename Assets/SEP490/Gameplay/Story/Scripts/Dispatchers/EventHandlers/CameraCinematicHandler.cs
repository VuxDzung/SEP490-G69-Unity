namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class PendingCinematicData
    {
        public string BackgroundId {  get; set; }
        public string CinematicType { get; set; }
        public ParameterInspectorData[] ParamArray { get; set; }
    }

    public class CameraCinematicHandler : NarrativeActionHandlerBase
    {
        private const string VAR_CINEMATIC_TYPE = "cinematicType";
        private const string VAR_CINEMATIC_DURATION = "duration";
        private const string VAR_MOVE_MAGNITUDE = "magnitude";
        private const string VAR_FADE_DURATION = "fadeDuration";
        private const string VAR_ORTH_SIZE = "orthSize";
        private const string VAR_X_POSITION = "xPosition";

        private readonly ContextManager _contextManager;
        private readonly Queue<PendingCinematicData> _pendingCinematics;
        private bool _isPlaying;
        public CameraCinematicHandler(ContextManager contextManager) : base(contextManager)
        {
            _contextManager = contextManager;
            _pendingCinematics = new Queue<PendingCinematicData>();
        }

        private SceneBGSetter _bgSetter;

        private SceneBGSetter BGSetter
        {
            get
            {
                if (_bgSetter == null)
                {
                    _bgSetter = _contextManager.GetSceneContext<SceneBGSetter>();
                }
                return _bgSetter;
            }
        }

        public override string ActionId => GameConstants.ACTION_CINEMATIC_CAMERA;

        public override void Execute(DialogEvent ev)
        {
            if (ev.Action != ActionId) return;

            string type = ev.Parameters ?.FirstOrDefault(p => p.ParamName == VAR_CINEMATIC_TYPE)?.GetStringValue();

            if (string.IsNullOrEmpty(type))
            {
                Debug.LogWarning("[CameraCinematicHandler] Missing cinematicType");
                return;
            }

            _pendingCinematics.Enqueue(new PendingCinematicData
            {
                BackgroundId = ev.BackgroundId,
                CinematicType = type,
                ParamArray = ev.Parameters
            });

            TryPlayNext();
        }

        private void TryPlayNext()
        {
            if (_isPlaying == true || _pendingCinematics.Count == 0)
            {
                return;
            }

            var data = _pendingCinematics.Dequeue();
            _isPlaying = true;

            PlayCinematic(data, () =>
            {
                _isPlaying = false;
                TryPlayNext();
            });
        }

        private void PlayCinematic(PendingCinematicData data, System.Action onComplete)
        {
            var cam = CinematicCameraController.Instance;

            if (cam == null)
            {
                Debug.LogError("[CameraCinematicHandler] CameraController is null");
                onComplete?.Invoke();
                return;
            }

            float duration = GetFloat(data, VAR_CINEMATIC_DURATION, 1f);
            float magnitude = GetFloat(data, VAR_MOVE_MAGNITUDE, 2f);
            float fadeDuration = GetFloat(data, VAR_FADE_DURATION, 0.2f);
            float xPosition = GetFloat(data, VAR_X_POSITION, 0f);

            switch (data.CinematicType)
            {
                // Zoom.
                case "zoom":
                    FadingController.Singleton.FadeIn2Out(fadeDuration, fadeDuration, () =>
                    {
                        BGSetter.SetBgById(data.BackgroundId);
                    });
                    float zoomOrthSize = GetFloat(data, VAR_ORTH_SIZE, 1f);
                    cam.DOZoom(zoomOrthSize, duration, onComplete);
                    cam.SetCameraXPosition(xPosition);
                    break;

                // Move
                case "moveLeft":
                    FadingController.Singleton.FadeIn2Out(fadeDuration, fadeDuration, () =>
                    {
                        BGSetter.SetBgById(data.BackgroundId);
                        cam.DOMove(Vector3.left * magnitude, duration, onComplete);
                    }, null, true);
                    break;
                case "moveRight":
                    FadingController.Singleton.FadeIn2Out(fadeDuration, fadeDuration, () =>
                    {
                        BGSetter.SetBgById(data.BackgroundId);
                        cam.DOMove(Vector3.right * magnitude, duration, onComplete);
                    }, null, true);
                    break;
                case "moveUp":
                    FadingController.Singleton.FadeIn2Out(fadeDuration, fadeDuration, () =>
                    {
                        BGSetter.SetBgById(data.BackgroundId);
                        cam.DOMove(Vector3.up * magnitude, duration, onComplete);
                    }, null, true);
                    break;
                case "moveDown":
                    FadingController.Singleton.FadeIn2Out(fadeDuration, fadeDuration, () =>
                    {
                        BGSetter.SetBgById(data.BackgroundId);
                        cam.DOMove(Vector3.down * magnitude, duration, onComplete);
                    }, null, true);
                    break;
                default:
                    Debug.Log($"<color=yellow>[Warning]</color> Unsupported cinematic: {data.CinematicType}");
                    onComplete?.Invoke();
                    break;
            }
        }

        #region Helper
        private float GetFloat(PendingCinematicData data, string key, float defaultValue)
        {
            return data.ParamArray
                ?.FirstOrDefault(p => p.ParamName == key)
                ?.GetFloatValue() ?? defaultValue;
        }
        #endregion
    }
}