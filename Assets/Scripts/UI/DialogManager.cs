using System.Collections.Generic;
using Common.Singleton;
using Sirenix.OdinInspector;
using UI.Dialog;
using UI.Menu;
using UnityEngine;

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

        [Title("Toast")]
        [SerializeField]
        private RectTransform toastRoot;


        private Menu.MenuController menuController;
        private AlertController alertController;
        private ToastController toastController;

        private void Start()
        {
            InitMenuDialog();
            InitAlertDialog();
            InitToast();
        }


        #region Menu

        public void OpenMenu(MenuParameter param)
        {
            menuController.Open(param);
        }

        public void CloseMenu()
        {
            menuController.Close();
        }

        private void InitMenuDialog()
        {
            menuController = menuDialogRoot.GetComponent<Menu.MenuController>();
        }

        #endregion

        #region Alert

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

        #endregion


        #region Toast

        public void OpenToast(string message, ToastDuration duration = ToastDuration.Normal)
        {
            toastController.Open(message,duration);
        }

        public void OpenToast(string message, float duration)
        {
            toastController.Open(message,duration);
        }

        private void InitToast()
        {
            toastController = toastRoot.GetComponent<ToastController>();
        }

        #endregion



    }
}