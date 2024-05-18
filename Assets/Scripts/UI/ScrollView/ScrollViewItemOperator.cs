using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ScrollView
{
    public enum ScrollViewItemState
    {
        Default,
        BeginDrag,
        Dragging,
        EndDrag
    }

    public class ScrollViewItemOperator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField]
        private GameObject line;

        [Title("Content Items")]
        [SerializeField]
        private GameObject content;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text urlText;
        [SerializeField]
        private TMP_Text userText;
        [SerializeField]
        private TMP_Text passwordText;

        [Title("Button Group")]
        [SerializeField]
        private GameObject buttonGroup;
        [SerializeField]
        private Button editButton;
        [SerializeField]
        private Button deleteButton;


        public UnityAction<ScrollViewItemState> OnStateChange;
        public UnityAction<string> OnClick;

        public string ID { get; private set; }
        public ScrollViewItemState State { get; private set; }


        #region 内部变量

        private Vector2 beginDragPosition;

        #endregion


        public void SetAccount(AccountInfo info)
        {
            ID = info.id;
            nameText.SetText(info.name);
            urlText.SetText(info.url);
            userText.SetText(info.user);
            passwordText.SetText(info.password);
        }


        private void ChangeState(ScrollViewItemState state)
        {
            if (State == state)
            {
                return;
            }
            State = state;
            OnStateChange?.Invoke(State);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log($"OnBeginDrag: {eventData.position}");

            ChangeState(ScrollViewItemState.BeginDrag);
            beginDragPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log($"OnDrag {eventData.position}");
            ChangeState(ScrollViewItemState.Dragging);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log($"OnEndDrag {eventData.position}");
            ChangeState(ScrollViewItemState.EndDrag);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (State is ScrollViewItemState.Default or ScrollViewItemState.EndDrag)
            {
                OnClick?.Invoke(ID);
            }
        }
    }
}