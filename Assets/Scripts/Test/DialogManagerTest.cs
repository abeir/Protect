using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace Test
{
    public class DialogManagerTest : MonoBehaviour
    {

        private readonly Stack<string> alertDialogIds = new();


        [Title("Menu Dialog")]
        [SerializeField]
        private bool useMenuDefault = true;
        [SerializeField, HideIf("useMenuDefault")]
        private MenuDialogParameter menuDialogParameter;

        [Title("Menu Dialog")]
        [Button]
        private void OpenMenuDialog()
        {
            if (useMenuDefault)
            {
                DialogManager.Instance.OpenMenu(new MenuDialogParameter
                {
                    AddCallback = OnClickMenuAdd,
                    QuitCallback = OnClickMenuQuit
                });
            }
            else
            {
                menuDialogParameter.AddCallback = OnClickMenuAdd;
                menuDialogParameter.QuitCallback = OnClickMenuQuit;
                DialogManager.Instance.OpenMenu(menuDialogParameter);
            }
        }

        [Button]
        private void CloseMenuDialog()
        {
            DialogManager.Instance.CloseMenu();
        }

        [Title("Alert Dialog")]
        [SerializeField]
        private bool useAlertDefault = true;
        [SerializeField, HideIf("useAlertDefault")]
        private AlertDialogParameter alertDialogParameter;

        [Title("Alert Dialog")]
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

        [Button]
        private void CloseAlertDialog()
        {
            if (alertDialogIds.TryPop(out var id))
            {
                DialogManager.Instance.CloseAlert(id);
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

        private void OnClickMenuAdd()
        {
            Debug.Log("DialogManagerTest.OnClickMenuAdd");
        }

        private void OnClickMenuQuit()
        {
            Debug.Log("DialogManagerTest.OnClickMenuQuit");
        }
    }
}