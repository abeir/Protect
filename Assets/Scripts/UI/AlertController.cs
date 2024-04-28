﻿using System;
using System.Collections.Generic;
using Common.Helper;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{

    /// <summary>
    /// 打开警告对话框的参数
    /// </summary>
    [Serializable]
    public class AlertDialogParameter
    {
        /// <summary>
        /// 对话框飞入的来源方向
        /// </summary>
        public AlertOperator.FlyInDirection FlyInDirection = AlertOperator.FlyInDirection.None;
        /// <summary>
        /// 对话框飞速的时长
        /// </summary>
        public float FlyInDuration;
        /// <summary>
        /// 对话框标题
        /// </summary>
        public string Title;
        /// <summary>
        /// 对话框内容
        /// </summary>
        [TextArea(2, 5)]
        public string Content;
        /// <summary>
        /// 确认按钮的文本
        /// </summary>
        public string OkText;
        /// <summary>
        /// 取消按钮的文本
        /// </summary>
        public string CancelText;
        /// <summary>
        /// 是否允许点击模态框背景时关闭对话框
        /// </summary>
        public bool ClickModalClose;
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

            if (param.ClickModalClose)
            {
                // 设置点击到模态框背景时的回调
                var modalController = info.Modal.GetComponent<ModalController>();
                modalController.UserData = info.ID;
                modalController.OnClick += OnModalClicked;
            }

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

            info.Modal.gameObject.SetActive(true);
            info.Alert.gameObject.SetActive(true);

            alertOperator.Show(
                param.FlyInDirection == AlertOperator.FlyInDirection.None ? defaultFlyInDirection : param.FlyInDirection,
                param.FlyInDuration > 0 ? param.FlyInDuration : defaultFlyInDuration);

            return info.ID;
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        /// <param name="id">对话框ID，创建对话框时会返回ID</param>
        public void Close(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("alert dialog id is empty");
                return;
            }
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
            var id = Strings.Uuid();

            var modal = Instantiate(modalPrefab, transform);
            modal.gameObject.SetActive(false);

            var alert = Instantiate(alertPrefab, transform);
            alert.gameObject.SetActive(false);

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

        // 点击模态框背景时关闭对话框
        private void OnModalClicked(PointerEventData data, object userData)
        {
            if (userData == null)
            {
                Debug.LogWarning("invalid userData, expect a alert dialog id");
                return;
            }
            Close(userData as string);
        }
    }
}