using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public Text coinText;
    public Text WinText;
    public int currentCoin = 0;
    public int maxHealth = 3;
    public Text health;

    public Animator animator;
    public Rigidbody2D rb;
    public float jumpHeight = 5f;
    public bool isGround = true;

    private float movement;
    public float moveSpeed = 5f;
    private bool facingRight = true;

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
        if (maxHealth <= 0)
        {
            Die();
        }

        coinText.text = currentCoin.ToString();
        health.text = maxHealth.ToString();
        
        movement = Input.GetAxis("Horizontal");

        if (movement <0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }

        else if (movement > 0f && facingRight == false)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        if(Input.GetKey(KeyCode.Space) && isGround == true)
        {
            Jump();
            //isGround = false;
            animator.SetBool("Jump", true);
        }

        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetFloat("Run", 1f);
        }

        else if(movement<0.1f)
        {
            animator.SetFloat("Run", 0f);
        }
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement,0f,0f) * Time.fixedDeltaTime * moveSpeed; 
    }

    void Jump() {
        rb.AddForce(new Vector2(0f, jumpHeight ), ForceMode2D.Impulse);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if(collision.gameObject.tag == "Ground")
        {
            isGround = true;
            animator.SetBool("Jump", false);
        } 

        if (collision.gameObject.tag == "Echo")
        {
            WinText.gameObject.SetActive(true);
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        
    }

    public void Attack() {
        Collider2D collinfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collinfo)
        {
            if (collinfo.gameObject.GetComponent<PatrolEnemy>() != null)
            {
                collinfo.gameObject.GetComponent<PatrolEnemy>().TakeDamage(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            currentCoin++;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f);
        }

        
    }

    void Die()
    {
        Debug.Log("Player died");
        FindObjectOfType<GameManager>().isGameActive = false;
        Destroy(this.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
