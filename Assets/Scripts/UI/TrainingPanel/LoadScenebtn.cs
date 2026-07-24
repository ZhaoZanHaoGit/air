using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    private TraniningPanel traniningPanel;
    public Button loadbtn,deleteBtn;
    public TextMeshProUGUI saveDataText;
    private SaveFileInfo fileInfo;
    // Start is called before the first frame update
    void Start()
    {
        loadbtn.onClick.AddListener(() => { traniningPanel.LoadTraining(fileInfo.fileName); });
        deleteBtn.onClick.AddListener(() => { traniningPanel.DeleteTrainingSave(fileInfo.fileName); });
    }
    public void InitSaveData(TraniningPanel panel,SaveFileInfo saveFileInfo)
    {
        traniningPanel = panel;
        fileInfo = saveFileInfo;
        saveDataText.text = saveFileInfo.saveTime;

    }

}
