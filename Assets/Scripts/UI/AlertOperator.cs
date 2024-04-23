using Common.Helper;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
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
                FlyInDirection.FromLeft => new Vector2(-Systems.ReferenceResolution.x, 0),
                FlyInDirection.FromRight => new Vector2(Systems.ReferenceResolution.x, 0),
                FlyInDirection.FromUp => new Vector2(0, Systems.ReferenceResolution.y),
                FlyInDirection.FromBottom => new Vector2(0, -Systems.ReferenceResolution.y),
                _ => dialogRoot.anchoredPosition
            };
            tweener.SetEase(Ease.InOutBounce);
            tweener.ChangeValues(dialogRoot.anchoredPosition, Vector2.zero, duration);
            tweener.PlayForward();
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