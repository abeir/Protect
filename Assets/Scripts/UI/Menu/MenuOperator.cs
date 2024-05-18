using Common.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Menu
{


    [RequireComponent(typeof(MenuAnimation))]
    public class MenuOperator : MonoBehaviour
    {
        public enum State
        {
            Displaying,
            Displayed,
            Hiding,
            Hidden
        }

        /// <summary>
        /// 菜单宽度
        /// </summary>
        [SerializeField]
        private float menuWidth;
        /// <summary>
        /// 每个菜单项的高度
        /// </summary>
        [SerializeField]
        private float itemHeight;

        /// <summary>
        /// 是否在最后一项中显示横线
        /// </summary>
        [SerializeField]
        private bool displayLineInLast;
        /// <summary>
        /// 箭头区域的引用
        /// </summary>
        [SerializeField]
        private RectTransform arrow;
        /// <summary>
        /// 箭头的高度
        /// </summary>
        [SerializeField]
        private float arrowHeight;
        /// <summary>
        /// 内容区域的引用
        /// </summary>
        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private float fadeDuration = 0.2f;



        #region 内部变量
        // 改变菜单项时会设置为true
        private bool menuItemChanged;
        private MenuItems menuItems;
        // 菜单的根节点
        private RectTransform menuRoot;
        // 内容区域中垂直布局的 top padding
        private float contentTopPadding;
        // 内容区域中垂直布局的 bottom padding
        private float contentBottomPadding;
        // 点击菜单项执行的回调，参数为菜单项的下标
        private UnityAction<int> onItemClick;
        // 菜单状态变换的时候调用
        private UnityAction<State> onStateChange;
        // 菜单状态
        private State state = State.Hidden;

        private MenuAnimation menuAnimation;

        #endregion


        private void Awake()
        {
            menuRoot = GetComponent<RectTransform>();

            menuAnimation = GetComponent<MenuAnimation>();
            menuAnimation.OnDisplayed += OnDisplayed;
            menuAnimation.OnHidden += OnHidden;

            var contentLayoutPadding = content.GetComponent<VerticalLayoutGroup>().padding;
            contentTopPadding = contentLayoutPadding.top;
            contentBottomPadding = contentLayoutPadding.bottom;
        }

        private void Start()
        {
            SetMenuActive(false);
        }


        public MenuOperator SetItems(MenuItems items)
        {
            if (items == null || items.items.Count == 0)
            {
                return this;
            }
            if (menuItems == items)
            {
                return this;
            }
            menuItems = items;
            menuItemChanged = true;
            return this;
        }

        public MenuOperator SetClickListener(UnityAction<int> callback)
        {
            onItemClick = callback;
            return this;
        }

        public MenuOperator SetStateChangeListener(UnityAction<State> callback)
        {
            onStateChange = callback;
            return this;
        }

        public State GetState()
        {
            return state;
        }

        public void Toggle()
        {
            switch (state)
            {
                case State.Displayed:
                    Hide();
                    break;
                case State.Hidden:
                    Show();
                    break;
                case State.Displaying:
                case State.Hiding:
                default:
                    break;
            }
        }

        public void Show()
        {
            if (state != State.Hidden)
            {
                return;
            }
            ChangeState(State.Displaying);

            SetMenuActive(true);

            // 菜单项修改后才会重新生成子项
            if (menuItemChanged)
            {
                menuItemChanged = false;
                RemoveAllItems();
                CreateAllItems();
            }
            var size = ComputeSize();

            menuAnimation.SetMaskPadding(size);
            menuAnimation.SetDuration(fadeDuration);
            menuAnimation.Show();
        }

        public void Hide()
        {
            if (state != State.Displayed)
            {
                return;
            }
            ChangeState(State.Hiding);

            menuAnimation.Hide();
        }



        private void SetMenuActive(bool active)
        {
            arrow.gameObject.SetActive(active);
            content.gameObject.SetActive(active);
        }

        private void ChangeState(State newState)
        {
            state = newState;
            onStateChange?.Invoke(state);
        }

        /// <summary>
        /// 计算并设置整个菜单的尺寸 <br/>
        /// <p>高度 = 箭头高度 + 菜单项数量 * 菜单项高度 + 内容布局上下padding</p>
        /// <p>宽度 = 菜单宽度</p>
        /// </summary>
        private SizeF ComputeSize()
        {
            var menuHeight = arrowHeight + menuItems.items.Count * itemHeight + contentTopPadding + contentBottomPadding;
            // root
            menuRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, menuWidth);
            menuRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, menuHeight);
            return new SizeF(menuWidth, menuHeight);
        }

        private void CreateAllItems()
        {
            var itemCount = menuItems.items.Count;
            for (var i=0; i<itemCount; i++)
            {
                CreateItem(i, menuItems.items[i], itemCount == (i+1));
            }
        }

        private void RemoveAllItems()
        {
            foreach (var c in content)
            {
                var child = c as Transform;
                if (child == null)
                {
                    continue;
                }
                Destroy(child.gameObject);
            }
        }

        private void CreateItem(int index, MenuItem mi, bool isLast)
        {
            var item = Instantiate(menuItems.prefab, content);
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);

            var itemButton = item.GetComponent<Button>();
            itemButton.onClick.AddListener(() => OnItemClicked(index));

            var image = item.Find("ItemContent/Image")?.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = mi.icon;
            }

            var text = item.Find("ItemContent/Text") as RectTransform;
            text?.GetComponent<TMP_Text>().SetText(mi.text);

            if (isLast && !displayLineInLast)
            {
                var line = item.Find("Line");
                line?.gameObject.SetActive(false);
            }
        }

        private void OnItemClicked(int index)
        {
            onItemClick?.Invoke(index);
        }


        private void OnDisplayed()
        {
            ChangeState(State.Displayed);
        }

        private void OnHidden()
        {
            ChangeState(State.Hidden);
            SetMenuActive(false);
        }
    }
}