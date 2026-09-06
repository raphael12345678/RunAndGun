using UnityEngine;
using UnityEngine.UI; // INDISPENSABLE pour l'UI

public class EnemyMeleeBehavior : MonoBehaviour
{
    [Header("Statistiques")]
    public int health = 3;
    public float speed = 3f;
    private int maxHealth; // Mémorise la vie totale

    [Header("Attaque")]
    public float attackRange = 1.2f;
    public int damage = 1;
    public float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    [Header("UI")]
    public Slider healthBar;
    public Image fillImage;

    private Transform targetPlayer;
    private bool isChasing = false;
    private bool isDead = false;
    private bool isAttackingAction = false;
    private Vector3 scaleOrigin;

    private Animator anim;
    private Rigidbody2D rb;

    private void Start()
    {
        scaleOrigin = transform.localScale;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        maxHealth = health;

        // Initialisation de l'UI
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }

        if (fillImage != null)
        {
            fillImage.color = Color.green;
        }

        // Ignorer les collisions physiques avec le joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D[] enemyColliders = GetComponents<Collider2D>();

            foreach (Collider2D col in enemyColliders)
            {
                if (!col.isTrigger)
                {
                    Physics2D.IgnoreCollision(col, playerCollider, true);
                }
            }
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (isChasing && targetPlayer != null)
        {
            float distance = Vector2.Distance(transform.position, targetPlayer.position);

            if (distance <= attackRange)
            {
                isAttackingAction = true;
                anim.SetBool("IsAttacking", true);
                anim.SetBool("IsWalking", false);

                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + attackCooldown;
                    anim.Play("Attack_enemy_melee", -1, 0f);
                    if (PlayerController.instant != null)
                    {
                        PlayerController.instant.TakeDamage(damage);
                    }
                }
            }
            else
            {
                isAttackingAction = false;
                anim.SetBool("IsAttacking", false);
                anim.SetBool("IsWalking", true);

                // Retournement visuel
                Vector2 direction = targetPlayer.position - transform.position;
                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(scaleOrigin.x), scaleOrigin.y, scaleOrigin.z);
                }
                else if (direction.x < 0)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(scaleOrigin.x), scaleOrigin.y, scaleOrigin.z);
                }

                // Pour éviter que la barre de vie s'affiche à l'envers quand l'ennemi se retourne
                if (healthBar != null)
                {
                    Vector3 canvasScale = healthBar.transform.parent.localScale;
                    canvasScale.x = Mathf.Sign(transform.localScale.x) * Mathf.Abs(canvasScale.x);
                    healthBar.transform.parent.localScale = canvasScale;
                }

                transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, speed * Time.deltaTime);
            }
        }
        else
        {
            isAttackingAction = false;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsAttacking", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;
        if (collision.CompareTag("Player"))
        {
            targetPlayer = collision.transform;
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            targetPlayer = null;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        health -= damageAmount;

        // Mise à jour de l'UI
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        if (fillImage != null)
        {
            float healthPercent = (float)health / maxHealth;
            fillImage.color = Color.Lerp(Color.red, Color.green, healthPercent);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetBool("IsDead", true);

        // Cache la barre de vie
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        Destroy(gameObject, 1f);
    }
}