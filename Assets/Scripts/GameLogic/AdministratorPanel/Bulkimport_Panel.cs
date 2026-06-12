using SFB;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;

public class Bulkimport_Panel : BaseWindowPanel
{
    private Button downloadButton, selectButton, cancelButton, okButton, againSelectButton;
    private GameObject unselectedStateObj, selectedStateObj;
    private TMP_Text desText;
    private string dataPath;
    private void Awake()
    {
        unselectedStateObj = transform.Find("bg/selecttable/bg/UnselectedState").gameObject;
        selectedStateObj = transform.Find("bg/selecttable/bg/SelectedState").gameObject;
        downloadButton = transform.Find("bg/download/downloadButton").GetComponent<Button>();
        selectButton = unselectedStateObj.transform.Find("selectButton").GetComponent<Button>();
        againSelectButton = selectedStateObj.transform.Find("againSelectButton").GetComponent<Button>();
        cancelButton = transform.Find("bg/btns/cancelButton").GetComponent<Button>();
        okButton = transform.Find("bg/btns/okButton").GetComponent<Button>();
        desText = selectedStateObj.transform.Find("des").GetComponent<TMP_Text>();
    }
    private void Start()
    {
        downloadButton.onClick.AddListener(() =>
        {
            string downloadUrl = "";
            string filename = "";
            switch (AppController.Instance.loginUser.Usertype)
            {
                case (int)UserType.管理员:
                    PanelUIType panelUIType = AdministratorPanel.currentPanel.GetUIType();
                    switch (panelUIType)
                    {
                        case PanelUIType.AdminTeacherManagement_Panel:                      
                            downloadUrl = Application.streamingAssetsPath+ "/UserTemplate/Teacher/教师信息表.xlsx";
                            filename = "教师信息表.xlsx";
                            break;
                        case PanelUIType.AdminStudentManagement_Panel:                        
                            downloadUrl = Application.streamingAssetsPath + "/UserTemplate/Student/学生信息表.xlsx";
                            filename = "学生信息表.xlsx";
                            break;
                    }
                    break;
                case (int)UserType.教师:
                    downloadUrl = Application.streamingAssetsPath + "/UserTemplate/Student/学生信息表.xlsx";
                    filename = "学生信息表.xlsx";
                    break;
            }
            //测试       
            var paths = StandaloneFileBrowser.OpenFolderPanel("选择文件夹", "", true);
            string saveDrivePath = WriteResult(paths);
            if (string.IsNullOrEmpty(saveDrivePath)) return;
            LoadHelper.Instance.DownLoadFile(downloadUrl, saveDrivePath, filename, null,true);

        });
        selectButton.onClick.AddListener(() =>
        {
            OpenFile();
            if (string.IsNullOrEmpty(dataPath)) return;
            unselectedStateObj.SetActive(false);
            selectedStateObj.SetActive(true);
            desText.text = GetFileInfo(dataPath);
        });
        againSelectButton.onClick.AddListener(() =>
        {
            OpenFile();
            if (string.IsNullOrEmpty(dataPath)) return;
            desText.text = GetFileInfo(dataPath);
        });

        cancelButton.onClick.AddListener(() =>
        {
            unselectedStateObj.SetActive(true);
            selectedStateObj.SetActive(false);
            OnHide();
        });
        okButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(dataPath))
            {
                UIManager.Instance.OpenMessageBoxUI("错误提示", "请选择文件！", 0, EnumMessageBoxType.OK);
                return;
            }
            DataSet dataSet = OpExcel.GetDataSet(dataPath);
            switch (AppController.Instance.loginUser.Usertype)
            {
                case (int)UserType.管理员:
                    PanelUIType panelUIType = AdministratorPanel.currentPanel.GetUIType();
                    switch (panelUIType)
                    {
                        case PanelUIType.AdminTeacherManagement_Panel:
                            if (dataSet.Tables[0].TableName.Trim() != "教师信息")
                            {
                                UIManager.Instance.OpenMessageBoxUI("错误提示", "选择文件错误！", 0, EnumMessageBoxType.OK);
                            }
                            else
                            {
                                AdminTeacherManagement_Panel panel = AdministratorPanel.GetPanel((int)PanelUIType.AdminTeacherManagement_Panel) as AdminTeacherManagement_Panel;
                                panel.AddUsers(dataSet);
                            }
                            break;
                        case PanelUIType.AdminStudentManagement_Panel:
                            if (dataSet.Tables[0].TableName.Trim() != "学生信息")
                            {
                                UIManager.Instance.OpenMessageBoxUI("错误提示", "选择文件错误！", 0, EnumMessageBoxType.OK);
                            }
                            else
                            {
                                AdminStudentManagement_Panel panel = AdministratorPanel.GetPanel((int)PanelUIType.AdminStudentManagement_Panel) as AdminStudentManagement_Panel;
                                panel.AddUsers(dataSet);
                            }
                            break;
                    }
                    break;
                case (int)UserType.教师:
                    //if (dataSet.Tables[0].TableName.Trim() != "学生信息")
                    //{
                    //    UIManager.Instance.OpenMessageBoxUI("错误提示", "选择文件错误！", 0, EnumMessageBoxType.OK);
                    //}
                    //else
                    //{
                    //    StudentManagement_Panel panel = TeacherPanel.GetPanel((int)PanelUIType.StudentManagement_Panel) as StudentManagement_Panel;
                    //    panel.AddUsers(dataSet);
                    //}
                    break;
            }
        });
    }
    void OpenFile()
    {
        var extensions = new[] {
            new ExtensionFilter("Files", "xlsx", "xls")
            };
        dataPath = WriteResult(StandaloneFileBrowser.OpenFilePanel("打开文件", "", extensions, true));
    }
}
