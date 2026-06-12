using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

namespace SuperTreeView
{
    public class ItemScript1 : MonoBehaviour
    {
        public Button mExpandBtn;
        public Image mIcon;
        public Image mSelectImg;
        public Button mClickBtn;
        public Text mLabelText;
        public TextMeshProUGUI mLabelTmp;
        object mData = "";

        public object Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
            }
        }

        void Start()
        {
            mExpandBtn.onClick.AddListener(OnExpandBtnClicked);
            mClickBtn.onClick.AddListener(OnItemClicked);
        }

        public void Init()
        {
            SetExpandBtnVisible(false);
            SetExpandStatus(true);
            IsSelected = false;
        }

        void OnExpandBtnClicked()
        {
            TreeViewItem item = GetComponent<TreeViewItem>();
            item.DoExpandOrCollapse();
        }


        public void SetItemInfo(string iconSpriteName, string labelTxt, object data = null)
        {
            Init();
            mIcon.sprite = ResManager2.Instance.GetSpriteByName(iconSpriteName);
            if (mLabelText != null)
                mLabelText.text = labelTxt;
            if (mLabelTmp != null)
            {
                mLabelTmp.text = labelTxt;
                mLabelTmp.gameObject.SetActive(true);
            }
            RectTransform Rect = mIcon.GetComponent<RectTransform>();
            if (Rect != null)
            {
                // 强行重建当前物体以及所有子物体的布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
            }
            mData = data;

        }

        void OnItemClicked()
        {
            TreeViewItem item = GetComponent<TreeViewItem>();
            item.RaiseCustomEvent(CustomEvent.ItemClicked, null);
            Debug.Log("TreeViewItem Clicked " + Data);

        }

        public void SetExpandBtnVisible(bool visible)
        {
            if (visible)
            {
                mExpandBtn.gameObject.SetActive(true);
            }
            else
            {
                mExpandBtn.gameObject.SetActive(false);
            }
        }

        public bool IsSelected
        {
            get
            {
                return mSelectImg.gameObject.activeSelf;
            }
            set
            {
                mSelectImg.gameObject.SetActive(value);
            }
        }
        public void SetExpandStatus(bool expand)
        {
            if (expand)
            {
                mExpandBtn.transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                mExpandBtn.transform.localEulerAngles = new Vector3(0, 0, 0);

            }
        }


    }

}