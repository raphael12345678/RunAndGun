using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 4f;

    // NOUVEAU : Permet de savoir à qui appartient la balle
    public bool isEnemyBullet = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // SÉCURITÉ : Si c'est une balle ennemie et qu'elle touche un autre ennemi, on ignore totalement
        if (isEnemyBullet && collision.CompareTag("Enemy"))
        {
            return; // La balle continue sa route et traverse l'allié
        }

        // 1. Si la balle APPARTIENT AU JOUEUR et touche un ENNEMI
        if (!isEnemyBullet && collision.CompareTag("Enemy"))
        {
            if (collision.isTrigger) return;

            // Cherche si c'est un ennemi à distance
            EnemyOneBehavior enemyRanged = collision.GetComponent<EnemyOneBehavior>();
            if (enemyRanged != null)
            {
                enemyRanged.TakeDamage(damage);
            }

            // Cherche si c'est un ennemi de mêlée
            EnemyMeleeBehavior enemyMelee = collision.GetComponent<EnemyMeleeBehavior>();
            if (enemyMelee != null)
            {
                enemyMelee.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // 2. Si la balle APPARTIENT À L'ENNEMI et touche le JOUEUR
        if (isEnemyBullet && collision.CompareTag("Player"))
        {
            if (PlayerController.instant != null)
            {
                PlayerController.instant.TakeDamage(damage);
            }
            Destroy(gameObject);
            return;
        }

    }
}