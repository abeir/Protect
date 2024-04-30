using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class MenuOperator : MonoBehaviour
    {
        public const float DefaultDuration = 0.5f;

        [SerializeField]
        private RectTransform dialogRoot;       // 菜单的最顶层节点
        [SerializeField]
        private Button addButton;       // 添加按钮
        [SerializeField]
        private Button quitButton;      // 退出按钮

        /// <summary>
        /// 菜单已显示后触发
        /// </summary>
        public UnityAction OnShown;
        /// <summary>
        /// 菜单已隐藏时触发
        /// </summary>
        public UnityAction OnHidden;

        #region 私有变量

        private TMP_Text addText;     // 添加按钮文本
        private TMP_Text quitText;       // 退出按钮文本

        private UnityAction addCallback;
        private UnityAction quitCallback;

        private Vector2 tmpAnchoredPosition = Vector2.zero;
        private TweenerCore<float, float, FloatOptions> tweener;

        #endregion

        private void Awake()
        {
            addText = addButton.GetComponentInChildren<TMP_Text>();
            quitText = quitButton.GetComponentInChildren<TMP_Text>();

            Debug.Log(addText);

            tweener = DOTween.To(() => dialogRoot.anchoredPosition.y, y =>
                {
                    tmpAnchoredPosition.y = y;
                    dialogRoot.anchoredPosition = tmpAnchoredPosition;
                }, 0f,
                DefaultDuration);
            tweener.SetAutoKill(false);
            tweener.SetEase(Ease.OutCubic);
            tweener.OnComplete(OnTweenCompleted);
            tweener.OnRewind(OnTweenCompleted);
            tweener.Pause();
        }

        private void OnDestroy()
        {
            tweener.Kill();
        }

        /// <summary>
        /// 设置添加按钮的文本信息
        /// </summary>
        public MenuOperator SetAddText(string text)
        {
            addText.SetText(text);
            return this;
        }
        /// <summary>
        /// 设置退出按钮的文本信息
        /// </summary>
        public MenuOperator SetQuitText(string text)
        {
            quitText.SetText(text);
            return this;
        }
        /// <summary>
        /// 设置添加按钮按下时的回调
        /// </summary>
        public MenuOperator SetAddListener(UnityAction callback)
        {
            addCallback = callback;

            addButton.onClick.RemoveAllListeners();
            addButton.onClick.AddListener(OnClickAdd);
            return this;
        }
        /// <summary>
        /// 设置退出按钮按下时的回调
        /// </summary>
        public MenuOperator SetQuitListener(UnityAction callback)
        {
            quitCallback = callback;

            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnClickQuit);
            return this;
        }

        /// <summary>
        /// 显示菜单，执行菜单进入的动画，在动画执行完成后触发 <see cref="OnShown"/>
        /// </summary>
        public void Show(float duration)
        {
            if (tweener.IsPlaying())
            {
                return;
            }
            dialogRoot.anchoredPosition = new Vector2(0, -dialogRoot.sizeDelta.y - 100f);

            tweener.ChangeValues(dialogRoot.anchoredPosition.y, 0f, duration);
            tweener.PlayForward();
        }
        /// <summary>
        /// 隐藏菜单，执行菜单退出的动画，在动画执行完成后触发 <see cref="OnHidden"/>
        /// </summary>
        public void Hide()
        {
            if (tweener.IsPlaying())
            {
                return;
            }
            tweener.PlayBackwards();
        }


        private void OnClickAdd()
        {
            addCallback?.Invoke();
        }

        private void OnClickQuit()
        {
            quitCallback?.Invoke();
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