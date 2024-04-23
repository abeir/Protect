using System.Collections.Generic;
using Common.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class DialogManager : UnityPersistentSingleton<DialogManager>
    {
        [Title("Menu Dialog")]
        [SerializeField]
        private RectTransform menuDialogRoot;   // 菜单对话框

        [Title("Alert Dialog")]
        [SerializeField]
        private RectTransform alertDialogRoot;

        [Title("Controlled")]
        [SerializeField]
        private List<CanvasGroup> controlledCanvasGroups;


        private MenuController menuController;
        private AlertController alertController;

        private void Start()
        {
            InitMenuDialog();
            InitAlertDialog();
        }

        public void OpenMenu(MenuDialogParameter param)
        {
            menuController.gameObject.SetActive(true);
            menuController.Open(param);

            // 阻止其他canvas响应点击
            foreach (var grp in controlledCanvasGroups)
            {
                grp.blocksRaycasts = false;
            }
        }

        public void CloseMenu()
        {
            menuController.Close();
            // 恢复其他canvas响应点击
            foreach (var grp in controlledCanvasGroups)
            {
                grp.blocksRaycasts = true;
            }
        }

        public string OpenAlert(AlertDialogParameter param)
        {
            alertController.gameObject.SetActive(true);
            return alertController.Open(param);
        }

        public void CloseAlert(string id)
        {
            alertController.Close(id);
        }

        private void InitAlertDialog()
        {
            alertController = alertDialogRoot.GetComponent<AlertController>();
            alertDialogRoot.gameObject.SetActive(false);
        }


        private void InitMenuDialog()
        {
            menuController = menuDialogRoot.GetComponent<MenuController>();
            menuController.gameObject.SetActive(false);
        }

    }
}