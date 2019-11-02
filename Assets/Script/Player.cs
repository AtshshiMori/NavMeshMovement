using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject target; // ゴールとなるターゲットオブジェクト
    UnityEngine.AI.NavMeshAgent agent;
    Animator animator;
    // private float speed = 0.0f; // NavMeshAgentのspeedを保持するための変数
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        // speed = agent.speed;     // 初期値の保持
    }

    void Update()
    {
        agent.destination = target.transform.position; // ターゲットの設定
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude); // Unityちゃんをアニメーションさせるためのパラメータ

        // 解決策1 等速で動かす
        // agent.velocity = (agent.steeringTarget - transform.position).normalized * agent.speed;
        // transform.forward = agent.steeringTarget - transform.position;

        // 解決策2 曲がり角でスピードを落とす
        // if (Vector3.Distance(agent.steeringTarget, transform.position) < 1.0f)
        // {
        //     agent.speed = 1.0f;
        // }
        // else
        // {
        //     agent.speed = speed;
        // }
    }
}
