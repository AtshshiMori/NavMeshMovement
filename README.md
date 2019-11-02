# NavMeshMovement

NavMeshAgentは簡単にオブジェクトを自動で移動させることができる便利な機能ですが、使っていてある問題が発生したので、その解決策を載せます。

## 問題
以下のようにスピードがある程度遅ければ良いが、早くすると急に曲がるときに通り過ぎてしまう。

Speed=3のときは問題なく曲がれる。
![speed3.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/260272/4aa22787-d174-071f-c835-a8bc864be0b8.gif)
  
Speed=5 のときは行き過ぎてしまい上手く曲がれない。
![speed5.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/260272/42448425-616e-bff4-38c3-4540917fdea3.gif)
  
## ソースコード
サンプルシーンのソースコードです。
Unityちゃんに以下のスクリプトを追加します。
targetにはインスペクターからUnityちゃんが向かう先であるカプセル型のオブジェクトを指定しておきます。

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject target; // ゴールとなるターゲットオブジェクト
    UnityEngine.AI.NavMeshAgent agent;
    Animator animator;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        agent.destination = target.transform.position; // ターゲットの設定
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude); // Unityちゃんをアニメーションさせるためのパラメータ
    }
}
```


## 解決策1　等速で動かす
とりあえず等速で動くようにすることで解決ができます。
Update関数内に以下を追加します。

```
agent.velocity = (agent.steeringTarget - transform.position).normalized * agent.speed;
transform.forward = agent.steeringTarget - transform.position;
```
agent.velocityでNavMeshAgentのスピードを指定できます。

NavMeshAgentは常にゴールまでのパスを保持しており、
agent.steeringTargetはパスの中継地点のうち、現在向かっている地点となります。

<img width="591" alt="steeringTarget.png" src="https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/260272/f14a8700-5126-9c7f-fbc9-2445a36c9018.png">


したがって、
`(agent.steeringTarget - transform.position)`
により方向を取得し、agent.speedをかけることで常にインスペクターでNavMeshAgentに設定したスピードで動くことになります。

  
  
結果がこちら  
![fixedSpeed.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/260272/475f0f18-013a-0373-208b-f00702b76146.gif)

上手く曲がれるようになりましたが、動きに滑らかさはなくなってしまいます。表現したいものによっては使えると思います。

## 解決策2　曲がり角でスピードを落とす
曲がり角が近くなったらスピードを落とすようにします。
以下のようにスクリプトに追加します。

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject target; // ゴールとなるターゲットオブジェクト
    
    UnityEngine.AI.NavMeshAgent agent;
    Animator animator;
    
    private float speed = 0.0f; // 追加:NavMeshAgentのspeedを保持するための変数
    
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        speed = agent.speed;            // 追加:初期値の保持
    }

    void Update()
    {
        agent.destination = target.transform.position; // ターゲットの設定
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude); // Unityちゃんをアニメーションさせるためのパラメータ

        // 以下を追加
        if (Vector3.Distance(agent.steeringTarget, transform.position) < 1.0f)
        {
            agent.speed = 1.0f;
        }
        else
        {
            agent.speed = speed;
        }
    }
}
```

先ほどの steeringTarget で曲がり角の位置を取得し、距離が近くなったときに speed を 1.0f に変更しています。


その結果がこちら  
![slowSpeed.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/260272/0ac1fc8c-8cdb-ca08-1f34-7e6a9dd900fa.gif)
  
スムーズに曲がれるようになりました。

