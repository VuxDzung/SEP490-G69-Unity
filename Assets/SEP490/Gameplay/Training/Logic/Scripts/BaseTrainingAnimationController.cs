namespace SEP490G69.Training
{
    using System;
    using UnityEngine;

    public abstract class BaseTrainingAnimationController : MonoBehaviour
    {
        /// <summary>
        /// Kích hoạt chuỗi hoạt ảnh tập luyện dựa trên loại bài tập
        /// </summary>
        public abstract void PlayTrainingAnim(ETrainingType trainingType, Action onComplete);

        /// <summary>
        /// Ép buộc dừng và dọn dẹp toàn bộ hoạt ảnh/tween đang chạy
        /// </summary>
        public abstract void StopAllAnimations();
    }
}