using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Menu
{
    [Serializable]
    public class MenuItem
    {
        public Sprite icon;
        public string text;
    }

    [CreateAssetMenu(menuName = "Project/Menu Items")]
    public class MenuItems : ScriptableObject
    {
        /// <summary>
        /// 菜单项的预制体
        /// </summary>
        public RectTransform prefab;
        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType type;

        public List<MenuItem> items;
    }
}