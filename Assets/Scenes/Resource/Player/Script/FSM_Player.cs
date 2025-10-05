using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Player_Params
{
    public float Speed;//角色移动速度   
    public float JumpHeight;//角色跳跃高度
    public Animator Player_Animator;//角色动画播放器
    public Rigidbody2D Player_Rigidbody2d;//角色的刚体控制器
    public Transform Player_Transform;//角色的Transform

}

public class FSM_Player : MonoBehaviour
{
    [SerializeField]
    public Player_Params player_Params;
    public Istate_Player Istate_Player;//玩家动画的基类
    public PlayerAnimationEnum PlayerAnimationEnum;//玩家的动画枚举
    public SpriteRenderer SpriteRenderer;

    public Dictionary<PlayerAnimationEnum, Istate_Player> dic_animate = new Dictionary<PlayerAnimationEnum, Istate_Player>();//创建<枚举，接口>的字典
    
    // 攻击队列系统
    private Queue<PlayerAnimationEnum> attackQueue = new Queue<PlayerAnimationEnum>();
    private bool isAttacking = false;
    private PlayerAnimationEnum currentAttack = PlayerAnimationEnum.Idle;

    

    // Start is called before the first frame update
    void Start()
    {
        player_Params.Player_Animator = transform.Find("Player_Sprite").GetComponent<Animator>();//找到子对象叫Player_Sprite的
        dic_animate.Add(PlayerAnimationEnum.Idle, new Player_Idle(this));
        dic_animate.Add(PlayerAnimationEnum.Run, new Player_Run(this));
        dic_animate.Add(PlayerAnimationEnum.Jump, new Player_Jump(this));
        dic_animate.Add(PlayerAnimationEnum.Fall, new Player_Fall(this));
        
        dic_animate.Add(PlayerAnimationEnum.Attack1, new Player_Attack1(this));
        dic_animate.Add(PlayerAnimationEnum.Attack2, new Player_Attack2(this));
        dic_animate.Add(PlayerAnimationEnum.Attack3, new Player_Attack3(this));
        PlayerAnimateTransform(PlayerAnimationEnum.Idle);
        Application.targetFrameRate = 60;

    }

    // Update is called once per frame
    void FixedUpdate()
    {    
        Istate_Player.OnUpdate();
    }
    public void PlayerAnimateTransform(PlayerAnimationEnum PlayerAnimationEnum)
    {
        if (Istate_Player != null)
        {
            Istate_Player.OnExit();
        }
        Istate_Player = dic_animate[PlayerAnimationEnum];
        Istate_Player.OnEnter();
    }
    public void PlayPlayerAnimate(String name)
    {
        player_Params.Player_Animator.Play(name);
    }

    // 攻击队列管理方法
    public void RequestAttack()
    {
        if (!isAttacking)
        {
            // 如果当前没有攻击，直接开始Attack1
            StartAttackSequence(PlayerAnimationEnum.Attack1);
        }
        else
        {
            // 如果正在攻击，将下一个攻击加入队列
            EnqueueNextAttack();
        }
    }

    private void StartAttackSequence(PlayerAnimationEnum attackType)
    {
        isAttacking = true;
        currentAttack = attackType;
        PlayerAnimateTransform(attackType);
    }

    private void EnqueueNextAttack()
    {
        // 根据当前攻击确定下一个攻击
        PlayerAnimationEnum nextAttack = GetNextAttackInSequence(currentAttack);
        if (nextAttack != PlayerAnimationEnum.Idle)
        {
            attackQueue.Enqueue(nextAttack);
        }
    }

    private PlayerAnimationEnum GetNextAttackInSequence(PlayerAnimationEnum current)
    {
        switch (current)
        {
            case PlayerAnimationEnum.Attack1:
                return PlayerAnimationEnum.Attack2;
            case PlayerAnimationEnum.Attack2:
                return PlayerAnimationEnum.Attack3;
            case PlayerAnimationEnum.Attack3:
                return PlayerAnimationEnum.Attack1; // 循环回到第一段
            default:
                return PlayerAnimationEnum.Idle;
        }
    }

    public void OnAttackAnimationComplete()
    {
        if (attackQueue.Count > 0)
        {
            // 执行队列中的下一个攻击
            PlayerAnimationEnum nextAttack = attackQueue.Dequeue();
            StartAttackSequence(nextAttack);
        }
        else
        {
            // 攻击序列结束，返回Idle
            isAttacking = false;
            currentAttack = PlayerAnimationEnum.Idle;
            PlayerAnimateTransform(PlayerAnimationEnum.Idle);
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public PlayerAnimationEnum GetCurrentAttack()
    {
        return currentAttack;
    }

}


public static class PlayerAnimateName
{//先尝试不加前缀static,const来试一下2025/6/22
    public const String Idle = "Idle";
    public const String Run = "Run";
    public const String Jump = "Jump";
    public const String Fall= "Fall";

    public const String Attack1 = "Attack1";
    public const String Attack2 = "Attack2";
    public const String Attack3 = "Attack3";
}

public enum PlayerAnimationEnum
{
    Idle,
    Run,
    Jump,
    Fall,
    Attack1,
    Attack2,
    Attack3
}



