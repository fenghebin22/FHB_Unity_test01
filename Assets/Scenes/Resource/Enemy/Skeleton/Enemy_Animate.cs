using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Idle : Istate_Enemy{
    public Enemy_FSM enemy_FSM;
    public Enemy_Params enemy_Params;
    private float idleTimer = 0f;

    public Enemy_Idle(Enemy_FSM fsm){
        enemy_FSM = fsm;
        enemy_Params = fsm.Enemy_Params;
    }
    public void OnEnter(){
        enemy_FSM.Play_EnemyAniamte(Enemy_AnimationName.Idle);
        enemy_Params.Enemy_Rigidbody2D.velocity = new Vector2(0, enemy_Params.Enemy_Rigidbody2D.velocity.y);
        idleTimer = 0f;
        enemy_Params.patrolWaitTimer = 0f;
    }
    public void OnUpdate(){
        idleTimer += Time.deltaTime;
        
        // 如果是巡逻等待状态
        if(enemy_Params.isPatrolWaiting){
            enemy_Params.patrolWaitTimer += Time.deltaTime;
            //Debug.Log($"Patrol waiting: {enemy_Params.patrolWaitTimer}/{enemy_Params.patrolWaitDuration}");
            
            // 巡逻等待时间到，切换回Walk状态继续巡逻
            if(enemy_Params.patrolWaitTimer >= enemy_Params.patrolWaitDuration){
                enemy_Params.isPatrolWaiting = false;
                enemy_Params.patrolWaitTimer = 0f;
                //Debug.Log("Patrol wait finished, switching back to Walk");
                enemy_FSM.Transition_EnemyAnimate(Enemy_enum.Walk);
            }
            return;
        }
        
        // 空闲一段时间后开始巡逻
        if(idleTimer >= enemy_Params.idleDuration){
            //Debug.Log("Idle duration finished, starting patrol");
            enemy_FSM.Transition_EnemyAnimate(Enemy_enum.Walk);
        }
        
        // Logic for idle state, such as checking for player proximity
        // 这里可以添加检测玩家的逻辑，如果检测到玩家可以切换到Chase状态
        
    }
    public void OnExit(){

    }
}

public class Enemy_Attack : Istate_Enemy{
    public Enemy_FSM enemy_FSM;
    public Enemy_Params enemy_Params;

    public Enemy_Attack(Enemy_FSM fsm){
        enemy_FSM = fsm;
        enemy_Params = fsm.Enemy_Params;
    }
    public void OnEnter(){
        enemy_FSM.Play_EnemyAniamte(Enemy_AnimationName.Attack);
        // Here you can add logic to deal damage to the player or trigger any attack effects

    }
    public void OnUpdate(){
        // Logic for attack state, such as checking if the attack animation is done
        if(!enemy_Params.Enemy_Animator.GetCurrentAnimatorStateInfo(0).IsName(Enemy_AnimationName.Attack)){
            enemy_FSM.Transition_EnemyAnimate(Enemy_enum.Idle);
        }               

    }
    public void OnExit(){}
}

public class Enemy_Walk : Istate_Enemy{
    public Enemy_FSM enemy_FSM;
    public Enemy_Params enemy_Params;

    public Enemy_Walk(Enemy_FSM fsm){
        enemy_FSM = fsm;
        enemy_Params = fsm.Enemy_Params;
    }
    public void OnEnter(){
        enemy_FSM.Play_EnemyAniamte(Enemy_AnimationName.Walk); 
        //Debug.Log($"Walk state entered, movingToRight: {enemy_FSM.movingToRight}");
    }
    public void OnUpdate(){
        // 巡逻逻辑
        PatrolBetweenPoints();
    }
    public void OnExit(){}

    private void PatrolBetweenPoints(){
        Vector3 targetPosition;
        
        // 确定目标位置
        if(enemy_FSM.movingToRight){
            targetPosition = enemy_Params.Patrol_right.position;
        } else {
            targetPosition = enemy_Params.Patrol_left.position;
        }

        // 计算移动方向
        Vector3 direction = (targetPosition - enemy_FSM.transform.position).normalized;
        
        // 设置速度
        enemy_Params.Enemy_Rigidbody2D.velocity = new Vector2(direction.x * enemy_Params.patrolSpeed, enemy_Params.Enemy_Rigidbody2D.velocity.y);

        // 设置朝向
        if(direction.x > 0){
            enemy_FSM.transform.localScale = new Vector3(Mathf.Abs(enemy_FSM.transform.localScale.x), enemy_FSM.transform.localScale.y, enemy_FSM.transform.localScale.z);
        } else if(direction.x < 0){
            enemy_FSM.transform.localScale = new Vector3(-Mathf.Abs(enemy_FSM.transform.localScale.x), enemy_FSM.transform.localScale.y, enemy_FSM.transform.localScale.z);
        }

        // 检查是否到达目标点
        float distanceToTarget = Vector3.Distance(enemy_FSM.transform.position, targetPosition);
        
        // 添加调试信息
        //Debug.Log($"Enemy at: {enemy_FSM.transform.position}, Target: {targetPosition}, Distance: {distanceToTarget}, MovingToRight: {enemy_FSM.movingToRight}");
        
        if(distanceToTarget < 0.5f){
            // 到达目标点，切换方向并切换到Idle状态等待
            enemy_FSM.movingToRight = !enemy_FSM.movingToRight;
            //Debug.Log($"Reached patrol point! Switching direction to: {enemy_FSM.movingToRight}");
            // 设置Idle状态为巡逻等待模式
            enemy_Params.isPatrolWaiting = true;
            enemy_FSM.Transition_EnemyAnimate(Enemy_enum.Idle);
        }
    }
}

public class Enemy_Chase : Istate_Enemy{
    public Enemy_FSM enemy_FSM;
    public Enemy_Params enemy_Params;

    public Enemy_Chase(Enemy_FSM fsm){
        enemy_FSM = fsm;
        enemy_Params = fsm.Enemy_Params;
    }
    public void OnEnter(){
        enemy_FSM.Play_EnemyAniamte(Enemy_AnimationName.Chase); 
        enemy_Params.Enemy_Rigidbody2D.velocity = new Vector2(enemy_Params.Speed * Time.deltaTime, enemy_Params.Enemy_Rigidbody2D.velocity.y);

    }
    public void OnUpdate(){
        if(enemy_Params.Enemy_Rigidbody2D.velocity.x == 0){
            enemy_FSM.Transition_EnemyAnimate(Enemy_enum.Idle);
        } 
        // Logic for walk state, such as moving the enemy or checking for player proximity

    }
    public void OnExit(){}
}

public class Enemy_React : Istate_Enemy{
    public Enemy_FSM enemy_FSM;
    public Enemy_Params enemy_Params;

    public Enemy_React(Enemy_FSM fsm){
        enemy_FSM = fsm;
        enemy_Params = fsm.Enemy_Params;
    }
    public void OnEnter(){
        enemy_FSM.Play_EnemyAniamte(Enemy_AnimationName.Attack); 
        // Here you can add logic to deal damage to the player or trigger any attack effects
        
    }
    public void OnUpdate(){
        // Logic for attack state, such as checking if the attack animation is done
        if(!enemy_Params.Enemy_Animator.GetCurrentAnimatorStateInfo(0).IsName(Enemy_AnimationName.Attack)){
            enemy_FSM.Transition_EnemyAnimate(Enemy_enum.Idle);
        }
    }
    public void OnExit(){}
}