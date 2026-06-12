using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ClickGoNewPos : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform newPosTrans;
    public Button clickBut;
    public Transform cameraTran;
    public CameraMove cameraM;
    void Start()
    {
        clickBut=GetComponent<Button>();
        clickBut.onClick.AddListener(() => {
            GoNewPos();
        });
    }

    
    
    public void GoNewPos() {
        cameraM.isMoving = true;
        cameraTran.DORotateQuaternion(newPosTrans.rotation,2);
        cameraTran.DOMove(newPosTrans.position, 2).OnComplete(() => {
            cameraM.isMoving = false;
        });
        Debug.Log("Be click");
    }
}
