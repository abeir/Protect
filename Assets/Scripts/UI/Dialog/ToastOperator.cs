using Common.Helper;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialog
{
    public class ToastOperator : MonoBehaviour
    {
        /// <summary>
        /// toast 默认显示时长
        /// </summary>
        public const float DefaultDuration = 1f;

        [SerializeField]
        private RectTransform toastRoot;   // toast 根元素
        [SerializeField]
        private Image background;       // 背景图片
        [SerializeField]
        private TMP_Text text;       // toast 信息

        [Title("Toast Settings")]
        [SerializeField]
        private float toastHorizontalMargin = 40f;
        [SerializeField]
        private float backgroundOpacity = 0.6f;        // 背景的不透明度
        [SerializeField]
        private float fadeDuration = 0.2f; // 淡入淡出的时长
        [SerializeField]
        private float visibleDuration = DefaultDuration;      // 可见状态的持续时间


        #region 内部变量

        private float horizontalPadding;
        private float verticalPadding;

        private float changedVisibleDuration;       // 修改的可见状态的持续时间
        private Color backgroundVisibleColor;      // 背景图片可见时的颜色
        private Color textVisibleColor;            // 文本可见时的颜色

        private bool needResetTweenSequence;

        private Sequence tweenSequence;

        #endregion


        private void Awake()
        {
            var size = text.GetComponent<RectTransform>().sizeDelta;
            horizontalPadding = Mathf.Ceil(Mathf.Abs(size.x) / 2f);
            verticalPadding = Mathf.Ceil(Mathf.Abs(size.y) / 2f);

            // 初始化时，设置背景和文本透明
            backgroundVisibleColor = background.color;
            background.color = new Color(backgroundVisibleColor.r, backgroundVisibleColor.g, backgroundVisibleColor.b, 0f);
            textVisibleColor = text.color;
            text.color = new Color(textVisibleColor.r, textVisibleColor.g, textVisibleColor.a, 0f);
        }

        private void OnDestroy()
        {
            tweenSequence.Kill();
        }


        public ToastOperator SetMessage(string msg)
        {
            text.SetText(msg);
            return this;
        }

        public ToastOperator SetDuration(float duration)
        {
            if (Maths.Near(changedVisibleDuration, duration))
            {
                return this;
            }
            changedVisibleDuration = duration;
            needResetTweenSequence = true;
            return this;
        }

        public void Show()
        {
            if (needResetTweenSequence)
            {
                needResetTweenSequence = false;
                ResetTweenSequence();
            }
            tweenSequence.Restart();

            IntervalPerFrame.Create(this)
                .SetTimes(2)
                .SetCallback(() =>
                {
                    toastRoot.sizeDelta = ComputeToastSize();
                })
                .Start();
        }

        private Vector2 ComputeToastSize()
        {
            // toast 允许的最大宽度
            var maxToastWidth = Systems.Instance.ReferenceResolution.x - 2 * toastHorizontalMargin;
            // 计算 toast 预期的宽高
            var expectToastWidth = text.preferredWidth + 2 * horizontalPadding;
            var expectToastHeight = text.preferredHeight + 2 * verticalPadding;

            var toastSize = Vector2.zero;
            toastSize.x = Mathf.Ceil(Mathf.Min(maxToastWidth, expectToastWidth));
            toastSize.y = Mathf.Ceil(expectToastHeight);
            return toastSize;
        }

        private void ResetTweenSequence()
        {
            tweenSequence?.Kill();

            changedVisibleDuration = changedVisibleDuration > 0 ? changedVisibleDuration : visibleDuration;

            tweenSequence = DOTween.Sequence();
            tweenSequence.Append(background.DOFade(backgroundOpacity, fadeDuration));
            tweenSequence.Join(text.DOFade(1f, fadeDuration));
            tweenSequence.AppendInterval(changedVisibleDuration);
            tweenSequence.Append(background.DOFade(0f, fadeDuration));
            tweenSequence.Join(text.DOFade(0f, fadeDuration));

            tweenSequence.SetAutoKill(false)
                .OnComplete(OnTweenSequenceComplete)
                .Pause();
        }

        private void OnTweenSequenceComplete()
        {
            gameObject.SetActive(false);
        }
    }
}