using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Common
{
    public class ModalController : MonoBehaviour, IPointerClickHandler
    {

        public UnityAction<PointerEventData, object> OnClick;


        public object UserData { set; get; }


        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData, UserData);
        }

        private void OnDestroy()
        {
            if (OnClick == null)
            {
                return;
            }
            foreach (var d in OnClick.GetInvocationList())
            {
                OnClick -= d as UnityAction<PointerEventData, object>;
            }
        }
    }
}