using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Idle : Istate_Player
{

    FSM_Player FSM_Player;
    Player_Params player_Params;//从FSM中调用的Player_Params
    public Player_Idle(FSM_Player FSM_Player)
    {
        this.FSM_Player = FSM_Player;
        player_Params = FSM_Player.player_Params;
    }

    public void OnEnter()
    {
        FSM_Player.PlayPlayerAnimate(PlayerAnimateName.Idle);
    }
    public void OnUpdate()
    {
        //注意使用的是GetKey而不是GetKeyDown,防止A和D键一起按
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))&&!(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)))
        {
            FSM_Player.PlayerAnimateTransform(PlayerAnimationEnum.Run);
        }


        if (Input.GetKeyDown(KeyCode.J))
        {
            FSM_Player.RequestAttack();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FSM_Player.PlayerAnimateTransform(PlayerAnimationEnum.Jump);
        }

    }
    public void OnExit()
    {

    }
}


public class Player_Run : Istate_Player
{

    FSM_Player FSM_Player;
    Player_Params player_Params;//从FSM_Player中调用的Player_Params
    private bool _isRun;

    public Player_Run(FSM_Player FSM_Player)
    {
        this.FSM_Player = FSM_Player;
        player_Params = FSM_Player.player_Params;//两个脚本之间传值，不能因为public就直接用还要写明那个类中的player_Params
    }
    public void OnEnter()
    {
        FSM_Player.PlayPlayerAnimate(PlayerAnimateName.Run);
    }
    public void OnUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal != 0)
        {
            //player_Params.Player_Rigidbody2d.drag = 50;
            player_Params.Player_Rigidbody2d.velocity = new Vector2(player_Params.Speed * horizontal, player_Params.Player_Rigidbody2d.velocity.y);
            if (horizontal > 0)
            {
                player_Params.Player_Transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                player_Params.Player_Transform.localScale = new Vector3(1, 1, 1);
            }
        }
        if (horizontal == 0)
        {
            //player_Params.Player_Animator.Play("Idle");

            FSM_Player.PlayerAnimateTransform(PlayerAnimationEnum.Idle);
        }

        ///
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FSM_Player.PlayerAnimateTransform(PlayerAnimationEnum.Jump);
        }

    }
    public void OnExit()
    {

    }
}


public class Player_Jump : Istate_Player
{

    FSM_Player FSM_Player;
    Player_Params player_Params;//从FSM中调用的Player_Params
    public Player_Jump(FSM_Player FSM_Player)
    {
        this.FSM_Player = FSM_Player;
        player_Params = FSM_Player.player_Params;
    }
    public void OnEnter()
    {
        player_Params.Player_Rigidbody2d.velocity = new Vector2(player_Params.Player_Rigidbody2d.velocity.x,
        player_Params.JumpHeight);
        FSM_Player.PlayPlayerAnimate(PlayerAnimateName.Jump);

    }
    public void OnUpdate()
    {
        //Debug.Log("JUMP:" + player_Params.Player_Rigidbody2d.velocity.y);
        if (player_Params.Player_Rigidbody2d.velocity.y < 0.1f)
        {
            FSM_Player.PlayerAnimateTransform(PlayerAnimationEnum.Fall);

        }

        //能够在跳跃的时候改变方向
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            //player_Params.Player_Rigidbody2d.drag = 50;
            player_Params.Player_Rigidbody2d.velocity = new Vector2(player_Params.Speed * horizontal, player_Params.Player_Rigidbody2d.velocity.y);
            if (horizontal > 0)
            {
                player_Params.Player_Transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                player_Params.Player_Transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    public void OnExit()
    {

    }
}

public class Player_Fall : Istate_Player
{

    FSM_Player FSM_Player;
    Player_Params player_Params;//从FSM中调用的Player_Params
    CheckGround checkGround;
    public Player_Fall(FSM_Player FSM_Player)
    {
        this.FSM_Player = FSM_Player;
        player_Params = FSM_Player.player_Params;
    }
    public void OnEnter()
    {

        FSM_Player.PlayPlayerAnimate(PlayerAnimateName.Fall);
        checkGround = GameObject.Find("CheckGround").GetComponent<CheckGround>();
    }
    public void OnUpdate()
    {
        if (checkGround.IsGroundedCollider())
        {
            FSM_Player.PlayerAnimateTransform(PlayerAnimationEnum.Idle);
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            //player_Params.Player_Rigidbody2d.drag = 50;
            player_Params.Player_Rigidbody2d.velocity = new Vector2(player_Params.Speed * horizontal, player_Params.Player_Rigidbody2d.velocity.y);
            if (horizontal > 0)
            {
                player_Params.Player_Transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                player_Params.Player_Transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    public void OnExit()
    {

    }
}


public class Player_Attack1 : Istate_Player//一段普工
{
    FSM_Player FSM_Player;
    Player_Params player_Params;//从FSM中调用的Player_Params
    private bool hasRequestedNextAttack = false; // 防止重复请求

    public Player_Attack1(FSM_Player FSM_Player)
    {
        this.FSM_Player = FSM_Player;
        player_Params = FSM_Player.player_Params;
    }

    public void OnEnter()
    {
        FSM_Player.PlayPlayerAnimate(PlayerAnimateName.Attack1);
        hasRequestedNextAttack = false;
        // Debug.Log("time：" + player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void OnUpdate()
    {
        // 检测攻击键输入，在攻击动画的合适时机允许连击
        if (player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f
          && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
          && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")
          && Input.GetKeyDown(KeyCode.J)
          && !hasRequestedNextAttack)
        {
            FSM_Player.RequestAttack();
            hasRequestedNextAttack = true;
        }
        
        // 动画播放完成时通知FSM
        if (player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
            && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            FSM_Player.OnAttackAnimationComplete();
        }
    }

    public void OnExit()
    {
        hasRequestedNextAttack = false;
    }
}



public class Player_Attack2 : Istate_Player
{
    FSM_Player FSM_Player;
    Player_Params player_Params;//从FSM中调用的Player_Params
    private bool hasRequestedNextAttack = false; // 防止重复请求

    public Player_Attack2(FSM_Player FSM_Player)
    {
        this.FSM_Player = FSM_Player;
        player_Params = FSM_Player.player_Params;
    }

    public void OnEnter()
    {
        FSM_Player.PlayPlayerAnimate(PlayerAnimateName.Attack2);
        hasRequestedNextAttack = false;
    }

    public void OnUpdate()
    {
        // 检测攻击键输入，在攻击动画的合适时机允许连击
        if (player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f
        && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
        && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")
        && Input.GetKeyDown(KeyCode.J)
        && !hasRequestedNextAttack)
        {
            FSM_Player.RequestAttack();
            hasRequestedNextAttack = true;
        }
        
        // 动画播放完成时通知FSM
        if (player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
            && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            FSM_Player.OnAttackAnimationComplete();
        }
    }

    public void OnExit()
    {
        hasRequestedNextAttack = false;
    }
}

public class Player_Attack3 : Istate_Player
{
    FSM_Player FSM_Player;
    Player_Params player_Params;//从FSM中调用的Player_Params
    private bool hasRequestedNextAttack = false; // 防止重复请求

    public Player_Attack3(FSM_Player FSM_Player)
    {
        this.FSM_Player = FSM_Player;
        player_Params = FSM_Player.player_Params;
    }

    public void OnEnter()
    {
        FSM_Player.PlayPlayerAnimate(PlayerAnimateName.Attack3);
        hasRequestedNextAttack = false;
        //Debug.Log("3");
    }

    public void OnUpdate()
    {
        // 检测攻击键输入，在攻击动画的合适时机允许连击（循环回Attack1）
        if (player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f
        && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
        && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3")
        && Input.GetKeyDown(KeyCode.J)
        && !hasRequestedNextAttack)
        {
            FSM_Player.RequestAttack();
            hasRequestedNextAttack = true;
        }
        
        // 动画播放完成时通知FSM
        if (player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
            && player_Params.Player_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            FSM_Player.OnAttackAnimationComplete();
        }
    }

    public void OnExit()
    {
        hasRequestedNextAttack = false;
    }
}
