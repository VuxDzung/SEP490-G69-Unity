namespace SEP490G69
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class TimerManager : GlobalSingleton<TimerManager>
    {
        private Action onFixedUpdate;
        private Action onUpdate;
        private Action onLateUpdate;

        private List<Timer> preRegisterTimerList = new List<Timer>();
        private List<Timer> activeTimerList = new List<Timer>();
        private List<Timer> preRemovedTimerList = new List<Timer>();

        public ulong Tick { get; private set; }
        public float DeltaTick => Time.fixedDeltaTime;

        #region Timer
        public static void AddTimer(Timer timer)
        {
            if (Singleton.preRemovedTimerList.Contains(timer) && Singleton.activeTimerList.Contains(timer))
            {
                Singleton.preRemovedTimerList.Remove(timer);
            }
            if (!Singleton.activeTimerList.Contains(timer) && !Singleton.preRegisterTimerList.Contains(timer))
            {
                Singleton.preRegisterTimerList.Add(timer);
            }
        }
        public static void RemoveTimer(Timer timer)
        {
            if (Singleton.activeTimerList.Contains(timer) && !Singleton.preRemovedTimerList.Contains(timer))
            {
                Singleton.preRemovedTimerList.Add(timer);
            }
        }
        #endregion

        #region Subcriptions
        public static void SubscribeFixedUpdate(Action action) => Singleton.onFixedUpdate += action;
        public static void SubscribeUpdate(Action action) => Singleton.onUpdate += action;
        public static void SubscribeLateUpdate(Action action) => Singleton.onLateUpdate += action;

        public static void UnsubscribeFixedUpdate(Action action) => Singleton.onFixedUpdate -= action;
        public static void UnsubscribeUpdate(Action action) => Singleton.onUpdate -= action;
        public static void UnsubscribeLateUpdate(Action action) => Singleton.onLateUpdate -= action;
        #endregion

        #region Unity Life Cycle
        protected virtual void FixedUpdate()
        {
            if (preRegisterTimerList.Count > 0)
            {
                activeTimerList.AddRange(preRegisterTimerList);
                preRegisterTimerList.Clear();
            }

            activeTimerList.ForEach(timer => timer.Update(DeltaTick));

            if (preRemovedTimerList.Count > 0)
            {
                foreach (var timer in preRemovedTimerList)
                {
                    activeTimerList.Remove(timer);
                }
            }
            Tick++;
            Tick %= ulong.MaxValue;
            onFixedUpdate?.Invoke();
        }

        protected virtual void Update()
        {
            onUpdate?.Invoke();
        }

        protected virtual void LateUpdate()
        {
            onLateUpdate?.Invoke();
        }
        #endregion
    }
}