using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
   [Header("检测设置")]
    [SerializeField] private float rayLength = 0.1f;
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private Vector2 offset = new Vector2(0, -0.1f);
    [SerializeField] private LayerMask groundLayer;
    public bool isGround;

    public Collider2D col;

    void Awake() {
        col = GetComponent<Collider2D>();
    }

    private void Update() {
        isGround = IsGroundedBox();
        
        //Debug.Log(IsGroundedCollider());
    }

    // 射线检测版本（精确但可能漏检边缘）
     public bool IsGroundedRay()
    {
        Vector2 origin = (Vector2)transform.position + offset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.red);
        return hit.collider != null;
    }
    

    // 盒型检测版本（适合平台边缘）
    public bool IsGroundedBox()
    {
        Vector2 center = (Vector2)transform.position + offset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0, groundLayer);
        
        return hits.Length > 0;
    }

    // 碰撞体接触检测（实时性最佳）
    public bool IsGroundedCollider()
    {
        return col.IsTouchingLayers(groundLayer);
    }

    // 混合检测（推荐方案）
    public bool IsGrounded()
    {
        return IsGroundedCollider();
    }

    // 调试可视化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}
