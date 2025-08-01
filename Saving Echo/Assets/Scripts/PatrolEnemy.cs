using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public int maxHealth = 3;
    public bool facingLeft = true;
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public float distance = 1f;
    public LayerMask layerMask;
    public bool inRange = false;
    public Transform player;
    public float attackRange = 10f;
    public float retrieveDistance = 2.5f;
    public float chaseSpeed = 4f;
    public Animator animator;
   
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (FindObjectOfType<GameManager>().isGameActive == false)
        {
            return;
        }
        if (maxHealth <= 0)
        {
            Die();
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        if (inRange){
            Debug.Log("In range of player");
            if (facingLeft && player.position.x > transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingLeft = false;
            }
            else if (!facingLeft && player.position.x < transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = true;
            }
            if (Vector2.Distance(transform.position, player.position) > retrieveDistance)
            {
                animator.SetBool("Attack", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Attack", true);
            }
        }
        else {
            Debug.Log("Not in range of player");
            transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);

            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance);

            if (hit == false && facingLeft == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingLeft = false;
            }
            else if (hit == false && facingLeft == false)
            {
                transform.eulerAngles = new Vector3(0,0,0);
                facingLeft = true;
            }
        }
        
    }

    public void Attack()
    {
        Collider2D collinfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collinfo == true)
        {
            if (collinfo.gameObject.GetComponent<Player>() != null)
            {
                collinfo.gameObject.GetComponent<Player>().TakeDamage(1); 
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
        animator.SetTrigger("Hurt");
    }

    private void OnDrawGizmos()
    {
        if(checkPoint == null){
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if(attackPoint == null){
            return;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}
