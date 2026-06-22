using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELECTest : MonoBehaviour
{
    [Header("电磁线圈双接线柱配置")]
    public DynamicNode electricityPortA; // 接线柱 A (例如接24V)
    public DynamicNode electricityPortB; // 接线柱 B (例如接0V)
    public GameObject @object;

    void Update()
    {
        if (electricityPortA == null || electricityPortB == null) return;

        // 获取 A、B 两个端子当前在全网并查集中的通电极性
        NodeType statusA = electricityPortA.currentPowerStatus;
        NodeType statusB = electricityPortB.currentPowerStatus;

        // 核心物理判定：只有当两端子【同时通电】，且【一端是正极、一端是负极】时，才视为有电流通过线圈
        bool hasPower = (statusA == NodeType.Positive && statusB == NodeType.Negative) ||
                        (statusA == NodeType.Negative && statusB == NodeType.Positive);

        @object.SetActive(hasPower);
    }

    }
