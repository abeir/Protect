using System;
using System.Collections.Generic;
using Common.Helper;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{

    /// <summary>
    /// 打开警告对话框的参数
    /// </summary>
    [Serializable]
    public class AlertDialogParameter
    {
        public AlertOperator.FlyInDirection FlyInDirection = AlertOperator.FlyInDirection.None;

        public float FlyInDuration;
        /// <summary>
        /// 对话框标题
        /// </summary>
        public string Title;
        /// <summary>
        /// 对话框内容
        /// </summary>
        public string Content;
        public string OkText;
        public string CancelText;
        /// <summary>
        /// 按下确定按钮的回调
        /// </summary>
        public UnityAction OkCallback;
        /// <summary>
        /// 按下取消按钮的回调
        /// </summary>
        public UnityAction CancelCallback;
    }

    public class AlertDialogInfo : IDisposable
    {
        public string ID;
        public RectTransform Modal;
        public RectTransform Alert;
        public UnityAction OkCallback;
        public UnityAction CancelCallback;
        public AlertOperator Operator;

        public void Dispose()
        {
            Modal = null;
            Alert = null;
            OkCallback = null;
            CancelCallback = null;
            Operator = null;
        }
    }


    public class AlertController : MonoBehaviour
    {
        [Title("Prefabs")]
        [SerializeField]
        private RectTransform modalPrefab;
        [SerializeField]
        private RectTransform alertPrefab;

        [Title("Default")]
        [SerializeField]
        private string defaultOkText = "Ok";
        [SerializeField]
        private string defaultCancelText = "Cancel";
        [SerializeField]
        private AlertOperator.FlyInDirection defaultFlyInDirection = AlertOperator.FlyInDirection.FromBottom;
        [SerializeField]
        private float defaultFlyInDuration = AlertOperator.DefaultFlyInDuration;


        private readonly List<AlertDialogInfo> alertDialogs = new();


        /// <summary>
        /// 创建对话框并显示，点击对话框中的按钮会触发回调
        /// </summary>
        /// <returns>返回对话框ID，可用于调用 <see cref="Close"/> 关闭对话框</returns>
        public string Open(AlertDialogParameter param)
        {
            var info = Create();
            info.OkCallback = param.OkCallback;
            info.CancelCallback = param.CancelCallback;

            var alertOperator = info.Alert.GetComponent<AlertOperator>();
            info.Operator = alertOperator;

            alertOperator.SetId(info.ID)
                .SetTitle(param.Title)
                .SetContent(param.Content)
                .SetOkText(string.IsNullOrEmpty(param.OkText) ? defaultOkText : param.OkText)
                .SetCancelText(string.IsNullOrEmpty(param.CancelText) ? defaultCancelText : param.CancelText)
                .SetOkListener(OnClickOk)
                .SetCancelListener(OnClickCancel);

            alertOperator.OnHidden += () =>
            {
                Dispose(info.ID);
            };

            alertOperator.Show(
                param.FlyInDirection == AlertOperator.FlyInDirection.None ? defaultFlyInDirection : param.FlyInDirection,
                param.FlyInDuration > 0 ? param.FlyInDuration : defaultFlyInDuration);

            info.Modal.gameObject.SetActive(true);
            info.Alert.gameObject.SetActive(true);
            return info.ID;
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        /// <param name="id">对话框ID，创建对话框时会返回ID</param>
        public void Close(string id)
        {
            var info = alertDialogs.Find(i => i.ID == id);
            info?.Operator.Hide();
        }


        private void OnClickOk(string id)
        {
            var info = alertDialogs.Find(i => i.ID == id);
            info?.OkCallback?.Invoke();
            info?.Operator.Hide();
        }

        private void OnClickCancel(string id)
        {
            var info = alertDialogs.Find(i => i.ID == id);
            info?.CancelCallback?.Invoke();
            info?.Operator.Hide();
        }

        private AlertDialogInfo Create()
        {
            var modal = Instantiate(modalPrefab, transform);
            modal.gameObject.SetActive(false);

            var alert = Instantiate(alertPrefab, transform);
            alert.gameObject.SetActive(false);

            var id = Strings.Uuid();
            var info = new AlertDialogInfo
            {
                ID = id,
                Modal = modal,
                Alert = alert
            };
            alertDialogs.Add(info);
            return info;
        }


        // 销毁
        private void Dispose(string id)
        {
            var index = alertDialogs.FindIndex(i => i.ID == id);
            if (index < 0)
            {
                return;
            }

            var dialog = alertDialogs[index];
            alertDialogs.RemoveAt(index);

            Destroy(dialog.Modal.gameObject);
            Destroy(dialog.Alert.gameObject);

            dialog.Dispose();
        }
    }
}