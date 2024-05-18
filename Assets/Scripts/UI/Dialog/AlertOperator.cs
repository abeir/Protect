using Common.Helper;
using Common.Structure;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Dialog
{
    public class AlertOperator : MonoBehaviour
    {
        public const float DefaultFlyInDuration = 0.5f;

        /// <summary>
        /// 对话框飞入的方向
        /// </summary>
        public enum FlyInDirection
        {
            None, FromLeft, FromRight, FromUp, FromBottom
        }

        [SerializeField]
        private TMP_Text title;     // 对话框标题
        [SerializeField]
        private TMP_Text content;       // 对话框内容

        [SerializeField]
        private RectTransform dialogRoot;       // 对话框的最顶层节点
        [SerializeField]
        private Button ok;      // 确认按钮
        [SerializeField]
        private Button cancel;  // 取消按钮

        [Title("Alert Dialog Settings")]
        [SerializeField]
        private SizeF minSize = new SizeF(600f, 480f);  // 对话框的最小宽度

        [SerializeField]
        private float minHorizontalMargin = 40f;        // 对话框最小的水平边距


        /// <summary>
        /// 对话框已显示后触发
        /// </summary>
        public UnityAction OnShown;
        /// <summary>
        /// 对话框已隐藏时触发
        /// </summary>
        public UnityAction OnHidden;

        #region 私有变量

        private string id;
        private TMP_Text okText;
        private TMP_Text cancelText;

        private UnityAction<string> okCallback;
        private UnityAction<string> cancelCallback;

        private TweenerCore<Vector2,Vector2,VectorOptions> tweener;

        private RectTransform contentTransform;
        private RectTransform okButtonTransform;
        private RectTransform cancelButtonTransform;

        private SizeF initContentSize;
        private SizeF initDiffSize;

        #endregion

        private void Awake()
        {
            okText = ok.GetComponentInChildren<TMP_Text>();
            cancelText = cancel.GetComponentInChildren<TMP_Text>();

            tweener = DOTween.To(() => dialogRoot.anchoredPosition, pos => dialogRoot.anchoredPosition = pos, Vector2.zero, DefaultFlyInDuration);
            tweener.SetAutoKill(false);
            tweener.OnComplete(OnTweenCompleted);
            tweener.OnRewind(OnTweenCompleted);
            tweener.Pause();

            // 获取内容区域初始的大小
            contentTransform = content.GetComponent<RectTransform>();
            initContentSize = new SizeF(contentTransform.rect.size);
            // 获取内容区域与对话框边界的差
            initDiffSize = new SizeF(minSize.width - initContentSize.width, minSize.height - initContentSize.height);

            okButtonTransform = ok.GetComponent<RectTransform>();
            cancelButtonTransform = cancel.GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            ok.onClick.RemoveAllListeners();
            cancel.onClick.RemoveAllListeners();

            tweener.Kill();
        }

        /// <summary>
        /// 设置按下确认按钮的回调
        /// </summary>
        public AlertOperator SetOkListener(UnityAction<string> callback)
        {
            okCallback = callback;

            ok.onClick.RemoveAllListeners();
            ok.onClick.AddListener(OnClickOk);
            return this;
        }
        /// <summary>
        /// 按下取消按钮的回调
        /// </summary>
        public AlertOperator SetCancelListener(UnityAction<string> callback)
        {
            cancelCallback = callback;

            cancel.onClick.RemoveAllListeners();
            cancel.onClick.AddListener(OnClickCancel);
            return this;
        }
        /// <summary>
        /// 设置对话框ID
        /// </summary>
        public AlertOperator SetId(string dialogId)
        {
            this.id = dialogId;
            return this;
        }
        /// <summary>
        /// 设置对话框标题
        /// </summary>
        public AlertOperator SetTitle(string titleText)
        {
            title.SetText(titleText);
            return this;
        }
        /// <summary>
        /// 设置对话框内容
        /// </summary>
        public AlertOperator SetContent(string contentText)
        {
            content.SetText(contentText);
            return this;
        }
        /// <summary>
        /// 设置确认按钮的文本信息
        /// </summary>
        public AlertOperator SetOkText(string okTxt)
        {
            okText.SetText(okTxt);
            return this;
        }
        /// <summary>
        /// 设置取消按钮的文本信息
        /// </summary>
        public AlertOperator SetCancelText(string cancelTxt)
        {
            cancelText.SetText(cancelTxt);
            return this;
        }

        /// <summary>
        /// 显示对话框，执行动画让对话框从屏幕外飞入，动画完成时触发 <see cref="OnShown"/>
        /// </summary>
        /// <param name="direct">对话框飞入动画的方向，支持从上下左右四个方向飞入</param>
        /// <param name="duration">动画持续时间（秒）</param>
        public void Show(FlyInDirection direct, float duration)
        {
            if (tweener.IsPlaying())
            {
                return;
            }
            dialogRoot.anchoredPosition = direct switch
            {
                FlyInDirection.FromLeft => new Vector2(-Systems.Instance.ReferenceResolution.x, 0),
                FlyInDirection.FromRight => new Vector2(Systems.Instance.ReferenceResolution.x, 0),
                FlyInDirection.FromUp => new Vector2(0, Systems.Instance.ReferenceResolution.y),
                FlyInDirection.FromBottom => new Vector2(0, -Systems.Instance.ReferenceResolution.y),
                _ => dialogRoot.anchoredPosition
            };
            tweener.SetEase(Ease.InOutBounce);
            tweener.ChangeValues(dialogRoot.anchoredPosition, Vector2.zero, duration);
            tweener.PlayForward();

            // 更新对话框和内容区域的宽高
            IntervalPerFrame.Create(this)
                .SetTimes(2)
                .SetCallback(Resize)
                .Start();
        }

        /// <summary>
        /// 隐藏对话框，执行动画让对话框退出屏幕，动画完成时触发 <see cref="OnHidden"/>
        /// </summary>
        public void Hide()
        {
            if (tweener.IsPlaying())
            {
                return;
            }
            tweener.SetEase(Ease.OutCubic);
            tweener.PlayBackwards();
        }


        private void Resize()
        {
            var (dialogSize, contentSize, buttonSize) = ComputeSize();
            Debug.Log($"dialogSize: {dialogSize}, contentSize: {contentSize}, buttonSize: {buttonSize}");


            dialogRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dialogSize.width);
            dialogRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dialogSize.height);

            contentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentSize.width);
            contentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentSize.height);

            okButtonTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize.width);
            okButtonTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize.height);

            cancelButtonTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize.width);
            cancelButtonTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize.height);
        }

        // 重新计算对话框、内容区域、按键的尺寸
        private (SizeF dialog, SizeF content, SizeF button) ComputeSize()
        {
            // 允许的对话框的最大宽度
            var maxDialogWidth = Systems.Instance.ReferenceResolution.x - 2 * minHorizontalMargin;
            var maxDialogHeight = Systems.Instance.ReferenceResolution.y / 2;
            // 允许的内容的最大宽度
            var maxContentWidth = maxDialogWidth - initDiffSize.width;
            var maxContentHeight = maxDialogHeight - initDiffSize.height;

            var diffDialogWidth = content.preferredWidth - initContentSize.width;
            var diffDialogHeight = content.preferredHeight - initContentSize.height;
            // 计算预期的宽高
            var expectContentWidth = initContentSize.width + diffDialogWidth;
            var expectContentHeight = initContentSize.height + diffDialogHeight;
            var expectDialogWidth = minSize.width + diffDialogWidth;
            var expectDialogHeight = minSize.height + diffDialogHeight;

            var contentSize = SizeF.zero;
            contentSize.width = Mathf.Ceil(Mathf.Clamp(expectContentWidth, initContentSize.width, maxContentWidth));
            contentSize.height = Mathf.Ceil(Mathf.Clamp(expectContentHeight, initContentSize.height, maxContentHeight));
            var dialogSize = SizeF.zero;
            dialogSize.width = Mathf.Ceil(Mathf.Clamp(expectDialogWidth, minSize.width, maxDialogWidth));
            dialogSize.height = Mathf.Ceil(Mathf.Clamp(expectDialogHeight, minSize.height, maxDialogHeight));

            // 计算按键的宽
            var buttonSize = new SizeF(okButtonTransform.rect.size)
            {
                width = dialogSize.width / 2
            };

            return (dialogSize, contentSize, buttonSize);
        }

        private void OnClickOk()
        {
            okCallback?.Invoke(id);
        }

        private void OnClickCancel()
        {
            cancelCallback?.Invoke(id);
        }

        private void OnTweenCompleted()
        {
            if (tweener.IsBackwards())
            {
                OnHidden?.Invoke();
            }
            else
            {
                OnShown?.Invoke();
            }
        }
    }
}