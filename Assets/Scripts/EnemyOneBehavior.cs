using UnityEngine;
using UnityEngine.UI;

public class EnemyOneBehavior : MonoBehaviour
{
    [Header("Statistiques")]
    public int health = 3;
    private int maxHealth;

    [Header("Tir")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletStartZone;
    public float shootRate = 1.5f;
    private float nextShootTime = 0f;

    private Transform targetPlayer;
    private bool isPlayerInRange = false;
    private Vector3 scaleOrigin;
    private Animator anim;
    private bool isDead = false;

    [Header("UI")]
    public Slider healthBar;

    public Image fillImage;
    private void Start()
    {
        anim = GetComponent<Animator>();
        // On mémorise le scale initial (0.2) pour ne pas le casser quand l'ennemi se retourne
        scaleOrigin = transform.localScale; 
        
        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }

        if (fillImage != null)
        {
            fillImage.color = Color.green;
        }

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetPlayer = collision.transform;
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            targetPlayer = null;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && targetPlayer != null)
        {
            // 1. Calculer la direction vers le joueur
            Vector2 directionVersJoueur = targetPlayer.position - bulletStartZone.position;

            // 2. Orienter visuellement l'ennemi vers le joueur (en conservant le scale de 0.2)
            if (directionVersJoueur.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(scaleOrigin.x), scaleOrigin.y, scaleOrigin.z);
            }
            else if (directionVersJoueur.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(scaleOrigin.x), scaleOrigin.y, scaleOrigin.z);
            }

            // 3. Tirer si le délai est écoulé
            if (Time.time >= nextShootTime)
            {
                nextShootTime = Time.time + shootRate;
                Shoot(directionVersJoueur);
            }
        }
    }

    private void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null || bulletStartZone == null) return;

        // Normalise la direction pour que la balle aille à une vitesse constante
        direction.Normalize();

        GameObject bullet = Instantiate(bulletPrefab, bulletStartZone.position, Quaternion.identity);
        bullet.GetComponent<BulletBehavior>().Setup(direction);
    }

    private void Die()
    {
        isDead = true;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        // Lance l'animation de mort
        anim.SetBool("IsDead", true);

        // Désactive TOUS les colliders (le corps + la zone de détection) 
        // pour que le joueur et les balles passent au travers du cadavre
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // Optionnel : Désactive aussi son script pour stopper ses tirs
        this.enabled = false;

        // Détruit l'objet après la fin de l'animation. 
        // Remplace "1f" par la durée exacte de ton animation en secondes (ex: 0.5f)
        Destroy(gameObject, 1f);
    }
    public void TakeDamage(int damageAmount)
    {
        // Si l'ennemi est déjà mort, on ignore les dégâts supplémentaires
        if (isDead) return;

        health -= damageAmount;

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
}