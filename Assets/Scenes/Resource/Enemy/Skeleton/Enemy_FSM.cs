using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Enemy_Params{
        public int Damage;
        public int HP;
        public int Speed;
        public int Patrol_Speed;
        public int Chase_Speed;

        public Animator Enemy_Animator;
        public Rigidbody2D Enemy_Rigidbody2D; 

        public GameObject Enemy_patrol_Prefab;//预制体

        public Transform Patrol_left;
        public Transform Patrol_right;
         Transform Chase_left;
         Transform Chase_right;

        // 空闲和巡逻相关参数
        public float idleDuration = 2f; // 空闲2秒后开始巡逻
        public bool isPatrolWaiting = false; // 是否是巡逻等待状态
        public float patrolWaitTimer = 0f; // 巡逻等待计时器
        public float patrolWaitDuration = 2f; // 巡逻等待时间
        public float patrolSpeed = 0f; // 巡逻速度

    }
public class Enemy_FSM : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public Enemy_Params Enemy_Params;
    public Istate_Enemy Istate_Enemy;
    public Dictionary<Enemy_enum,Istate_Enemy> Enemy_dic = new Dictionary<Enemy_enum, Istate_Enemy>();
    
    // 巡逻方向控制
    public bool movingToRight = true;

    private void Awake() {
         //
        Enemy_Params.Patrol_left = Instantiate(Enemy_Params.Enemy_patrol_Prefab).transform;
        Enemy_Params.Patrol_right = Instantiate(Enemy_Params.Enemy_patrol_Prefab).transform;
        Enemy_Params.Patrol_left.position = new Vector3(transform.position.x-3,transform.position.y,transform.position.z);
        Enemy_Params.Patrol_right.position = new Vector3(transform.position.x+3,transform.position.y,transform.position.z);
        
        }
    
    void Start()
    {
        Enemy_Params.Enemy_Animator = GetComponent<Animator>();
        Enemy_Params.Enemy_Rigidbody2D = GetComponent<Rigidbody2D>();
        Enemy_dic.Add(Enemy_enum.Idle,new Enemy_Idle(this));
        Enemy_dic.Add(Enemy_enum.Walk,new Enemy_Walk(this));
        Enemy_dic.Add(Enemy_enum.Attack,new Enemy_Attack(this));
        Enemy_dic.Add(Enemy_enum.Chase,new Enemy_Chase(this));
        Transition_EnemyAnimate(Enemy_enum.Idle);
    }

    // Update is called once per frame
     void Update()
    {
         Istate_Enemy.OnUpdate();
    }
    public void Transition_EnemyAnimate(Enemy_enum state){
        if(Istate_Enemy!=null){
            Istate_Enemy.OnExit();
        }
        Istate_Enemy = Enemy_dic[state];
        Istate_Enemy.OnEnter();
    }

    public void Play_EnemyAniamte(string name){
        Enemy_Params.Enemy_Animator.Play(name);
        
    }
}


public static class Enemy_AnimationName{
    public const string Idle = "Idle";
    public const string Attack = "Attack";
    public const string Walk = "Walk";
    public const string Chase = "Chase";

} 

public enum Enemy_enum{
    Idle,
    Walk,
    Attack,
    Chase,
    Dead,
}