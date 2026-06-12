using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteLine : MonoBehaviour
{
  public  GameObject lineObject;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button=GetComponent<Button>();
        button.onClick.AddListener(() => {
            if (lineObject != null)
            { Destroy(lineObject); }
            gameObject.SetActive(false);
            lineObject=null;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
