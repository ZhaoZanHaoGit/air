using UnityEngine;

public class CameraTargets : MonoBehaviour
{
    public GameObject[] targets;
    public GameObject GetTarget(int index) {
        return targets[index].gameObject;
    }
}
