using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// 打开菜单时需要传入的参数
    /// </summary>
    [Serializable]
    public class MenuDialogParameter
    {
        /// <summary>
        /// 动画执行时间，可为空，为空时使用默认值
        /// </summary>
        public float Duration;
        /// <summary>
        /// 添加按钮的文本信息，可为空，为空时使用默认值
        /// </summary>
        public string AddText;
        /// <summary>
        /// 退出按钮的文本信息，可谓空，为空时使用默认值
        /// </summary>
        public string QuitText;
        /// <summary>
        /// 按下添加按钮时触发的回调
        /// </summary>
        public UnityAction AddCallback;
        /// <summary>
        /// 按下退出按钮时触发的回调
        /// </summary>
        public UnityAction QuitCallback;
    }

    public class MenuDialogInfo : IDisposable
    {
        public RectTransform Menu;
        public RectTransform Modal;

        public UnityAction AddCallback;
        public UnityAction QuitCallback;

        public MenuOperator Operator;

        public void Dispose()
        {
            Menu = null;
            Modal = null;
            Operator = null;
            AddCallback = null;
            QuitCallback = null;
        }
    }


    public class MenuController : MonoBehaviour
    {

        [Title("Prefabs")]
        [SerializeField]
        private RectTransform menuPrefab;
        [SerializeField]
        private RectTransform modalPrefab;

        [Title("Default")]
        [SerializeField]
        private string defaultAddText = "Add";
        [SerializeField]
        private string defaultQuitText = "Quit";
        [SerializeField]
        private float defaultDuration = MenuOperator.DefaultDuration;

        #region 私有变量

        private MenuDialogInfo info;

        #endregion

        /// <summary>
        /// 打开菜单，菜单在未关闭前不会创建多个菜单
        /// </summary>
        public void Open(MenuDialogParameter param)
        {
            if (info != null)
            {
                return;
            }

            info = Create(param);
            var menuOperator = info.Menu.GetComponent<MenuOperator>();
            info.Operator = menuOperator;

            menuOperator.SetAddText(string.IsNullOrEmpty(param.AddText) ? defaultAddText : param.AddText)
                .SetQuitText(string.IsNullOrEmpty(param.QuitText) ? defaultQuitText : param.QuitText)
                .SetAddListener(OnClickAdd)
                .SetQuitListener(OnClickQuit);

            menuOperator.OnHidden += Dispose;

            menuOperator.Show(param.Duration > 0 ? param.Duration : defaultDuration);

            info.Menu.gameObject.SetActive(true);
            info.Modal.gameObject.SetActive(true);
        }

        /// <summary>
        /// 关闭菜单
        /// </summary>
        public void Close()
        {
            info?.Operator.Hide();
        }

        private MenuDialogInfo Create(MenuDialogParameter param)
        {
            var modal = Instantiate(modalPrefab, transform);
            modal.gameObject.SetActive(false);

            var menu = Instantiate(menuPrefab, transform);
            menu.gameObject.SetActive(false);

            return new MenuDialogInfo
            {
                Menu = menu,
                Modal = modal,
                AddCallback = param.AddCallback,
                QuitCallback = param.QuitCallback
            };
        }

        private void Dispose()
        {
            Destroy(info.Menu.gameObject);
            Destroy(info.Modal.gameObject);
            info.Dispose();
            info = null;
        }


        private void OnClickAdd()
        {
            info?.AddCallback?.Invoke();
            info?.Operator.Hide();
        }

        private void OnClickQuit()
        {
            info?.QuitCallback?.Invoke();
            info?.Operator.Hide();
        }
    }
}