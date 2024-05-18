using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI;
using UI.Dialog;
using UI.Menu;
using UnityEngine;

namespace Test
{
    public class DialogManagerTest : MonoBehaviour
    {

        private readonly Stack<string> alertDialogIds = new();

        [BoxGroup("Menu Dialog")]
        [SerializeField]
        private MenuType menuType;
        [BoxGroup("Menu Dialog")]
        [Button]
        private void OpenMenuDialog()
        {
            DialogManager.Instance.OpenMenu(new MenuParameter
            {
                Type = menuType,
                OnClickItem = OnClickMenuItem
            });
        }

        [BoxGroup("Menu Dialog")]
        [Button]
        private void CloseMenuDialog()
        {
            DialogManager.Instance.CloseMenu();
        }

        [BoxGroup("Alert Dialog", centerLabel: true)]
        [SerializeField]
        private bool useAlertDefault = true;
        [BoxGroup("Alert Dialog")]
        [SerializeField, HideIf("useAlertDefault")]
        private AlertDialogParameter alertDialogParameter;

        [BoxGroup("Alert Dialog")]
        [Button]
        private void OpenAlertDialog()
        {
            string id;
            if (useAlertDefault)
            {
                id = DialogManager.Instance.OpenAlert(new AlertDialogParameter
                {
                    Title = "This is default title",
                    Content = "This is default content",
                    OkCallback = OnClickAlertOk,
                    CancelCallback = OnClickAlertCancel
                });
            }
            else
            {
                alertDialogParameter.OkCallback = OnClickAlertOk;
                alertDialogParameter.CancelCallback = OnClickAlertCancel;
                id = DialogManager.Instance.OpenAlert(alertDialogParameter);
            }
            alertDialogIds.Push(id);
        }

        [BoxGroup("Alert Dialog")]
        [Button]
        private void CloseAlertDialog()
        {
            if (alertDialogIds.TryPop(out var id))
            {
                DialogManager.Instance.CloseAlert(id);
            }
        }

        [BoxGroup("Toast", centerLabel: true)]
        [SerializeField]
        private bool useToastDurationEnum = true;
        [BoxGroup("Toast")]
        [SerializeField, PropertyRange(0f, 3f), LabelText("Toast Duration"), HideIf("useToastDurationEnum")]
        private float toastDurationValue;
        [BoxGroup("Toast")]
        [SerializeField, LabelText("Toast Duration"), ShowIf("useToastDurationEnum")]
        private ToastDuration toastDurationEnum;
        [BoxGroup("Toast")]
        [Button]
        private void OpenToast()
        {
            var msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (useToastDurationEnum)
            {
                DialogManager.Instance.OpenToast(msg, toastDurationEnum);
            }
            else
            {
                DialogManager.Instance.OpenToast(msg, toastDurationValue);
            }
        }

        private void OnClickAlertOk()
        {
            Debug.Log("DialogManagerTest.OnClickAlertOk");
        }

        private void OnClickAlertCancel()
        {
            Debug.Log("DialogManagerTest.OnClickAlertCancel");
        }

        private void OnClickMenuItem(int index)
        {
            Debug.Log($"DialogManagerTest.OnClickMenuItem index:{index}");
        }
    }
}