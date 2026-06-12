using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class showusermessage : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI name, department, id, phonenumber;


    // Update is called once per frame
    void Update()
    {
        id.text = LoginManager.user.id;
        name.text = LoginManager.user.name;
        department.text = LoginManager.user.department;
        phonenumber.text = LoginManager.user.phonenumber;
    }
}
