using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SuperScrollView
{
    public class SceneNameInfo
    {
        public string mName;
        public string mSceneName;
        public SceneNameInfo(string name, string sceneName)
        {
            mName = name;
            mSceneName = sceneName;
        }
    }
    class MenuSceneScript : MonoBehaviour
    {
        public Transform mButtonPanelTf;
        SceneNameInfo[] mSceneNameArray = new SceneNameInfo[]
        {
            new SceneNameInfo("Immediate Expand","TreeViewDemo_Immediate"),
            new SceneNameInfo("Clip Expand","TreeViewDemo_Clip"),
            new SceneNameInfo("Scale Expand","TreeViewDemo_Scale"),
            new SceneNameInfo("Immediate Expand\n(With Line)","TreeViewWithLineDemo_Immediate"),
            new SceneNameInfo("Clip Expand\n(With Line)","TreeViewWithLineDemo_Clip"),
            new SceneNameInfo("Scale Expand\n(With Line)","TreeViewWithLineDemo_Scale"),

        };
        void Start()
        {
            CreateFpsDisplyObj();
            int count = mButtonPanelTf.childCount;
            for (int i = 0; i < count; ++i)
            {
                SceneNameInfo info = mSceneNameArray[i];
                Button button = mButtonPanelTf.GetChild(i).GetComponent<Button>();
                button.onClick.AddListener(delegate ()
                {
                    SceneManager.LoadScene(info.mSceneName);
                });
                Text text = button.transform.Find("Text").GetComponent<Text>();
                text.text = info.mName;
            }

        }

        void CreateFpsDisplyObj()
        {
            FPSDisplay fpsObj = FindObjectOfType<FPSDisplay>();
            if (fpsObj != null)
            {
                return;
            }
            GameObject go = new GameObject();
            go.name = "FPSDisplay";
            go.AddComponent<FPSDisplay>();
            DontDestroyOnLoad(go);
        }

    }
}
