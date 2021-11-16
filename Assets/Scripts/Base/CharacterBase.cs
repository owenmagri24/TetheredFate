using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField]
    protected LayerMask m_PlatformsLayerMask;
    protected Rigidbody2D m_Rigidbody2D;

    protected BoxCollider2D m_BoxCollider2D;

    [SerializeField]
    protected float m_JumpForce = 500f;
    
    [SerializeField]
    protected float m_Speed = 5f;

    protected Vector2 direction = Vector2.zero;

    [SerializeField]
    protected float m_InteractDistance = 1f;

    [SerializeField]
    protected LayerMask m_InteractObjectMask;

    protected RaycastHit2D m_hit;

    protected GameObject m_ObjectHit;

    [SerializeField]
    protected Transform m_BoxHolder;
    protected bool m_HoldingObject = false;

    protected virtual void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update(){
        transform.Translate(direction * m_Speed * Time.deltaTime);
        m_hit = Physics2D.Raycast(transform.position, Vector2.right*transform.localScale.x, m_InteractDistance, m_InteractObjectMask);
    }

    public void OnMovement(InputValue value)
    {
        direction = value.Get<Vector2>();
        //character flipping
        Vector3 characterScale = transform.localScale;
        if(direction.x < 0){
            characterScale.x = -1;
        }
        if(direction.x > 0){
            characterScale.x = 1;
        }
        transform.localScale = characterScale;
    }

    public void OnJump(){
        if(IsGrounded()){
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    protected bool IsGrounded(){
        //BoxCast will only check for objects with Platforms Layer
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(m_BoxCollider2D.bounds.center, m_BoxCollider2D.bounds.size, 0f, Vector2.down, .1f, m_PlatformsLayerMask);
        return raycastHit2D.collider != null;
    }

    protected void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * transform.localScale.x * m_InteractDistance);
    }

    public void OnInteract(){
        /*
        if(m_hit.collider != null){
            m_ObjectHit = m_hit.collider.gameObject;

            if(m_ObjectHit.tag == "PushableObject")
            {
                Debug.Log(m_ObjectHit);

                m_ObjectHit.GetComponent<FixedJoint2D>().enabled = true;
                m_ObjectHit.GetComponent<FixedJoint2D>().connectedBody = m_Rigidbody2D;
            }
        }
        */
        if(m_hit.collider != null){
            m_ObjectHit = m_hit.collider.gameObject;

            if(m_ObjectHit.tag == "PushableObject" && m_HoldingObject == false)
            {
                m_HoldingObject = true;
                m_ObjectHit.transform.parent = m_BoxHolder;
                m_ObjectHit.transform.position = m_BoxHolder.position;
                m_ObjectHit.GetComponent<Rigidbody2D>().isKinematic = true;
            }
            else if(m_HoldingObject == true)
            {
                m_HoldingObject = false;
                m_ObjectHit.transform.parent = null;
                m_ObjectHit.GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }
    }
}