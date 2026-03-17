namespace SEP490G69.Battle.Combat
{
    using System;

    public class AnimationBarrier
    {
        private int _remaining;
        private Action _onCompleted;

        public AnimationBarrier(int count, Action onCompleted)
        {
            _remaining = count;
            _onCompleted = onCompleted;
        }

        public void Signal()
        {
            _remaining--;

            if (_remaining <= 0)
            {
                _onCompleted?.Invoke();
            }
        }
    }
}