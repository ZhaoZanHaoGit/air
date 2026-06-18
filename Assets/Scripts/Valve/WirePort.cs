using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirePort : PortBase
{
    public PortType portType = PortType.Free;
    public BaseValve parentValve;     // 所属阀门
   

    private void Awake()
    {
        base.Awake();
       
    }
   
}
