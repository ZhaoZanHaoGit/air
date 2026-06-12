
/******************************************************************************
 * 
 *  Title:				
 *
 *  Version:		
 *
 *  Description:
 *  1.启动脚本
 *
 *  Author:				
 *       
 *  Date:               
 * 
 ******************************************************************************/

using UnityEngine;

public class StartGame : MonoBehaviour
{
    private void Awake()
    {
        if (Defines.IsStart)
        {
            Defines.IsStart = false;
            var prefab = ResManager.Instance.LoadPrefab($"{Defines.UIPREFAB}Canvas");
            var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            CanvasController.Obj = obj;
            //Debug.Log(CanvasController.Obj.name);
            CanvasController.Instance.Init();
        }

        //var prefab = ResManager.Instance.LoadPrefab($"{Defines.UIPREFAB}Canvas");
        //var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        //CanvasController.Obj = obj;
        //Debug.Log(CanvasController.Obj.name);
        //CanvasController.Instance.Init();
        if (PlayerPrefs.HasKey(Defines.FULLSCREEN))
        {
            Screen.fullScreen = PlayerPrefs.GetInt(Defines.FULLSCREEN) == 1 ? true : false;
        }
    }
}
