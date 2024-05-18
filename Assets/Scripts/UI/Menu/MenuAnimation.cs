using Common.Structure;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Menu
{

    [RequireComponent(typeof(RectMask2D))]
    public class MenuAnimation : MonoBehaviour
    {
        public UnityAction OnDisplayed;
        public UnityAction OnHidden;


        private float duration;

        private RectMask2D rectMask;
        private Vector4 maskPadding;
        private TweenerCore<Vector4, Vector4, VectorOptions> tweener;
        private bool isDoHide;

        private void Awake()
        {
            rectMask = GetComponent<RectMask2D>();
        }

        private void OnDestroy()
        {
            tweener?.Kill();
        }

        public void SetMaskPadding(SizeF menuSize)
        {
            maskPadding = new Vector4(0, menuSize.height, 0,0 );
            rectMask.padding = maskPadding;

        }

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }

        public void Show()
        {
            isDoHide = false;
            Play(Vector4.zero);
        }

        public void Hide()
        {
            isDoHide = true;
            Play(maskPadding);
        }


        private void Play(Vector4 end)
        {
            tweener = DOTween.To(() => rectMask.padding, v => rectMask.padding = v, end, duration);
            tweener.SetAutoKill(true)
                .OnComplete(OnTweenCompleted)
                .Play();
        }

        private void OnTweenCompleted()
        {
            if (isDoHide)
            {
                OnHidden?.Invoke();
            }
            else
            {
                OnDisplayed?.Invoke();
            }

        }
    }
}