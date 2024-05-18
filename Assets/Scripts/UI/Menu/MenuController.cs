using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Menu
{

    public class MenuParameter
    {
        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType Type;
        /// <summary>
        /// 点击菜单项后执行的回调
        /// </summary>
        public UnityAction<int> OnClickItem;
    }

    public class MenuController : SerializedMonoBehaviour
    {
        [SerializeField]
        private MenuOperator menuOperator;
        [SerializeField]
        private ModalController modalController;        // 菜单的模态层，用于阻止穿透到背后，并且点击模态层后关闭菜单

        [SerializeField]
        private Dictionary<MenuType, MenuItems> menus;


        private MenuParameter menuParameter;

        private void Start()
        {
            menuOperator.SetStateChangeListener(OnMenuStateChanged)
                .SetClickListener(OnMenuItemClicked);

            modalController.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            modalController.OnClick += OnModalClicked;
        }

        private void OnDisable()
        {
            modalController.OnClick -= OnModalClicked;
        }


        /// <summary>
        /// 打开菜单，菜单在未关闭前不会创建多个菜单
        /// </summary>
        public void Open(MenuParameter param)
        {
            if (param == null)
            {
                return;
            }
            if (menuParameter == param)
            {
                menuOperator.Show();
                return;
            }
            if (!menus.TryGetValue(param.Type, out var items))
            {
                return;
            }
            menuParameter = param;

            menuOperator.SetItems(items).Show();
        }

        /// <summary>
        /// 关闭菜单
        /// </summary>
        public void Close()
        {
            menuOperator.Hide();
        }


        private void OnModalClicked(PointerEventData eventData, object userData)
        {
            Close();
        }

        private void OnMenuItemClicked(int index)
        {
            Close();
            menuParameter.OnClickItem?.Invoke(index);
        }

        private void OnMenuStateChanged(MenuOperator.State state)
        {
            if (state == MenuOperator.State.Displaying)
            {
                modalController.gameObject.SetActive(true);
            }
            else if (state == MenuOperator.State.Hiding)
            {
                modalController.gameObject.SetActive(false);
            }
        }
    }
}