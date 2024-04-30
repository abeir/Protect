using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{

    [Serializable]
    public class MenuItem
    {
        public string text;
    }

    public class Menu2Operator : MonoBehaviour
    {
        [SerializeField]
        private float menuWidth;
        [SerializeField]
        private RectTransform itemPrefab;
        [SerializeField]
        private float itemHeight;
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private RectTransform arrow;
        [SerializeField]
        private float arrowHeight;


        #region 内部变量

        private RectTransform menuRoot;

        private List<MenuItem> menuItems = new();

        private UnityAction<int> onItemClick;

        #endregion


        [Button]
        private void DoTest()
        {
            var i = new MenuItem
            {
                text = "Hello"
            };
            var arr = new List<MenuItem> { i };

            SetItems(arr);
            CreateAllItems();
            ComputeSize();
        }


        private void Awake()
        {
            menuRoot = GetComponent<RectTransform>();
        }


        public void SetItems(List<MenuItem> items)
        {
            menuItems = items;
        }

        public void SetClickListener(UnityAction<int> callback)
        {
            onItemClick = callback;
        }


        private void ComputeSize()
        {
            var menuHeight = arrowHeight + menuItems.Count * itemHeight;
            // root
            menuRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, menuWidth);
            menuRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, menuHeight);
        }

        private void CreateAllItems()
        {
            for (var i=0; i<menuItems.Count; i++)
            {
                CreateItem(i, menuItems[i]);
            }
        }

        private void CreateItem(int index, MenuItem mi)
        {
            var item = Instantiate(itemPrefab, content);
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);

            var itemButton = item.GetComponent<Button>();
            itemButton.onClick.AddListener(() => OnItemClicked(index));

            // var image = item.Find("ItemContent/Image") as RectTransform;

            var text = item.Find("ItemContent/Text") as RectTransform;
            text?.GetComponent<TMP_Text>().SetText(mi.text);
        }

        private void OnItemClicked(int index)
        {
            onItemClick?.Invoke(index);
        }

    }
}