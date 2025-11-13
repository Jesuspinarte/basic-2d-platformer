using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    int totalCoins;
    int coinsCollected;
    string curLevel;
    string nextLevel;

    public bool grounded;
    public float speed = 5.0f;
    public float jumpForce = 8.0f;
    public float airControlForce = 10.0f;
    public float airControlMax = 1.5f;
    public AudioSource coinSound;
    public TextMeshProUGUI uiText;

    IEnumerator DoDeath()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(curLevel);
    }

    IEnumerator LoadNextLevel()
    {
        if (nextLevel != "Finished")
        {
            GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(nextLevel);
        }
    }


    void Start()
    {
        coinsCollected = 0;
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        curLevel = SceneManager.GetActiveScene().name;
        if (curLevel == "Level1")
        {
            nextLevel = "Level2";
        }
        else if (curLevel == "Level2")
        {
            nextLevel = "Finished";
        }
    }

    private void Update()
    {
        float xSpeed = Mathf.Abs(rb.linearVelocity.x);
        float ySpeed = Mathf.Abs(rb.linearVelocity.y);
        float blinkVal = Random.Range(0.0f, 800.0f);

        animator.SetFloat("xSpeed", xSpeed);
        animator.SetFloat("ySpeed", ySpeed);

        if (rb.linearVelocity.x*transform.localScale.x < 0.0f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if (blinkVal < 1.0f)
        {
            animator.SetTrigger("blinkTrigger");
        }

        string coinString = "" + coinsCollected.ToString() + " of " + totalCoins.ToString();
        uiText.text = coinString;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");

        if (grounded)
        {
            if (Input.GetAxis("Jump") > 0f)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }
        }

        if (h != 0f)
        {
            rb.linearVelocity = new Vector2(h * speed, rb.linearVelocityY); 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && rb.linearVelocityY == 0)
        {
            grounded = true;
        }

        if (collision.gameObject.tag == "Death")
        {
            StartCoroutine(DoDeath());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && rb.linearVelocityY > 0)
        {
            grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            ++coinsCollected;
            coinSound.Play();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "LevelEnd")
        {
            collision.gameObject.SetActive(false);
            StartCoroutine(LoadNextLevel());
        }

    }
}
