using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransform : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        AudioSource transformChild = transform.Find("GameObject_1/GameObject_2").GetComponent<AudioSource>();
        Debug.Log(transformChild.clip.name);
        AudioListener.volume = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
