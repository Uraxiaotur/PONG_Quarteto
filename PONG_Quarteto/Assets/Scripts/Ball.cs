using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float speed;

    private float ogSpeed;
    private bool inMovement;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        ogSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            inMovement = true;
        }
        
        if (!inMovement && Input.GetKeyDown(KeyCode.Space))
        {
            ThrowBall();
        }
    }

    void OnEnable()
    {
        GameOM.OnGameStart += ThrowBall;
    }

    void OnDisable()
    {
        GameOM.OnGameStart -= ThrowBall;
    }
    
    private void ThrowBall()
    {
        SetDirection();
        direction = direction.normalized;
        rb.linearVelocity = direction * speed;
    }

    private void SetDirection()
    {
        if (GameManager.Instance.GetScoredPlayer() == player.None)
        {
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f));   
        }
        else if (GameManager.Instance.GetScoredPlayer() == player.P1)
        {
            direction = new Vector2(1f, Random.Range(-0.3f, 0.3f));   
        }
        else if (GameManager.Instance.GetScoredPlayer() == player.P2)
        {
            direction = new Vector2(-1f, Random.Range(-0.3f, 0.3f));   
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocityY = rb.linearVelocityY + (transform.position.y - collision.transform.position.y);
            direction.x *= -1f;
            speed += 0.5f;
        }

        if (collision.gameObject.CompareTag("Score1"))
        {
            if (GameManager.Instance.P2Points > 10)
            {
                Debug.Log("P2 Venceu");
                rb.linearVelocity = Vector2.zero;
                GameOM.GameOver(0);
                transform.position = new Vector2(0f, 0f);
                inMovement = false;
                speed = ogSpeed;
            }
            else if (GameManager.Instance.P2Points < 11)
            {
                Debug.Log("P2 Pontuou");
                rb.linearVelocity = Vector2.zero;
                GameOM.PlayerScored(player.P1);
                transform.position = new Vector2(0f, 0f);
                inMovement = false;
                speed = ogSpeed;
            }
        }
        else if (collision.gameObject.CompareTag("Score2"))
        {
            if (GameManager.Instance.P1Points > 10)
            {
                Debug.Log("P1 Venceu");
                rb.linearVelocity = Vector2.zero;
                GameOM.GameOver(0);
                transform.position = new Vector2(0f, 0f);
                inMovement = false;
                speed = ogSpeed;
            }
            else if (GameManager.Instance.P1Points < 11)
            {
                Debug.Log("P1 Pontuou");
                rb.linearVelocity = Vector2.zero;
                GameOM.PlayerScored(player.P2);
                transform.position = new Vector2(0f, 0f);
                inMovement = false;
                speed = ogSpeed;
            }
           
        }
    }
    
}
