using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public enum ToastDuration
    {
        Fast, Normal, Slow
    }

    public class ToastController : MonoBehaviour
    {

        [Title("Prefabs")]
        [SerializeField]
        private RectTransform toastPrefab;


        private ToastOperator toastOperator;

        public void Open(string message, float duration)
        {
            var toast = Create();
            toast.SetMessage(message).SetDuration(duration);

            toast.gameObject.SetActive(true);
            toast.Show();
        }

        public void Open(string message, ToastDuration toastDuration)
        {
            var duration = ConvertEnumToFloat(toastDuration);
            Open(message, duration);
        }

        private ToastOperator Create()
        {
            if (toastOperator == null)
            {
                var toast = Instantiate(toastPrefab, transform);
                toast.gameObject.SetActive(false);
                toastOperator = toast.GetComponent<ToastOperator>();
            }
            return toastOperator;
        }

        private static float ConvertEnumToFloat(ToastDuration duration)
        {
            return duration switch
            {
                ToastDuration.Fast => 0.6f,
                ToastDuration.Normal => ToastOperator.DefaultDuration,
                ToastDuration.Slow => 1.5f,
                _ => throw new ArgumentOutOfRangeException(nameof(duration), duration, null)
            };
        }
    }
}