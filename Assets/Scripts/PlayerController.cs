using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public float speed = 5f;
    public float jumpForce = 5f;

    [Header("Slide Settings")]
    public float slideSpeed = 8f;
    public float slideDuration = 0.4f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletStartZone;
    public float shootRate = 0.2f;
    private float nextShootTime = 0f;

    private bool isGrounded = false;
    private float faceFlip = 1f;
    private Vector3 scaleOrigin = Vector3.one;

    public float horizontal = 0f;
    public float vertical = 0f;
    private bool run = false;
    private bool slide = false;
    private bool isSliding = true; // Empêche de relancer un slide pendant le cooldown
    private bool IsDead = false;
    private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    public static PlayerController instant;

    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;
    public Slider healthBar;
    public Image fillImage;

    private void Awake() {
        instant = this;
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        scaleOrigin = transform.localScale;

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // NOUVEAU : Met la barre en vert au lancement
        if (fillImage != null)
        {
            fillImage.color = Color.green;
        }
    }

    private void Update() {

        if (IsDead) return;

        if (slide) return;

        horizontal = Input.GetAxis("Horizontal");
        rb.linearVelocityX = horizontal * speed;

        if (horizontal == 0f) {
            run = false;
        } else {
            run = true;
            if (horizontal < 0f) faceFlip = -1f;
            else if (horizontal > 0f) faceFlip = 1f;
        }
        
        transform.localScale = new Vector3(scaleOrigin.x * faceFlip, scaleOrigin.y, scaleOrigin.z);

        // Déclenchement du slide uniquement si on court (run), qu'on est au sol et hors cooldown
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && isSliding && run) {
            StartCoroutine(PerformSlide());
        }

        // Tir multidirectionnel
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= nextShootTime) {
            nextShootTime = Time.time + shootRate;
            Shoot();
        }

        animator.SetBool("IsSliding", slide);
        animator.SetBool("IsRunning", run);
        
        // Saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
        }
    }

    private void Shoot() {
        float aimX = Input.GetAxisRaw("Horizontal");
        float aimY = Input.GetAxisRaw("Vertical");
        Vector2 aimDirection = new Vector2(aimX, aimY);

        if (aimDirection == Vector2.zero) {
            aimDirection = new Vector2(faceFlip, 0f);
        }

        aimDirection = aimDirection.normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletStartZone.position, Quaternion.identity);
        bullet.GetComponent<BulletBehavior>().Setup(aimDirection);
    }

    public void TakeDamage(int damageAmount) {
        if (IsDead) return;
        currentHealth -= damageAmount;
        
        if (healthBar != null) {
            healthBar.value = currentHealth;
        }

        // NOUVEAU : Calcul de la couleur selon le % de vie restant
        if (fillImage != null) {
            float healthPercent = (float)currentHealth / maxHealth;
            // Lerp passe du Rouge (0%) au Vert (100%) progressivement
            fillImage.color = Color.Lerp(Color.red, Color.green, healthPercent); 
        }

        Debug.Log("Joueur blessé ! HP restants : " + currentHealth);

        if (currentHealth <= 0) {
            Die();
        }
    }


    private IEnumerator PerformSlide() {
        slide = true;
        isSliding = false;
        run = false;

        animator.SetBool("IsSliding", true);
        animator.SetBool("IsRunning", false);

        float slideDirection = faceFlip;
        float timer = 0f;

        while (timer < slideDuration) {
            rb.linearVelocityX = slideDirection * slideSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        slide = false;
        animator.SetBool("IsSliding", false);

        yield return new WaitForSeconds(0.3f);
        isSliding = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
    }

    private void Die()
    {
        IsDead = true;
        // 1. Déclenche l'animation
        animator.SetBool("IsDead", true);

        // 2. Stoppe physiquement le joueur et le fige totalement (désactive la gravité)
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // 3. Désactive les collisions pour que les ennemis passent au travers
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // 4. Détruit l'objet après 1 seconde (à adapter selon la durée de ton animation)
        Destroy(gameObject, 2f);
    }
}