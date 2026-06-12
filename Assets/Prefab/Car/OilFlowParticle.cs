using UnityEngine;

public class OilFlowParticle : MonoBehaviour
{
    private ParticleSystem ps;
    private float timer = 0f;
    private bool isColumnar = true;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 5f; // 粒子存活时间
        main.startSpeed = 2f; // 初始速度向下流动
        main.startSize = 0.1f; // 初始大小
        main.gravityModifier = 1f; // 受重力影响

        var emission = ps.emission;
        emission.rateOverTime = 100f; // 高发射率，柱状流

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone; // 初始柱状（锥形发射）
        shape.angle = 5f; // 窄角度，模拟柱状
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 10f && isColumnar)
        {
            isColumnar = false;
            var emission = ps.emission;
            emission.rateOverTime = 5f; // 降低速率，变成点滴

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere; // 转为球形，随机滴落
            shape.radius = 0.05f;

            var main = ps.main;
            main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f); // 随机大小，模拟点滴
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1f); // 慢速滴落
        }
    }
}