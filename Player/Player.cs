using Assets.Scripts;
using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    public GameObject PlayerGraphics;
    [HideInInspector]
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public SpriteFlicker spriteFlicker;
    [HideInInspector]
    public SpriteTrails trails;
    [HideInInspector]
    public TrailRenderer trailRenderer;
    [HideInInspector]
    public PlayerUI playerUI;
    [HideInInspector]
    public ProgressBar StaminaBar;
    [HideInInspector]
    public Rigidbody2D rb;
    public GameObject hitDetector, drill;
    Material defaultMaterial;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<Controller2D>();
        spriteFlicker = GetComponentInChildren<SpriteFlicker>();
        trails = GetComponentInChildren<SpriteTrails>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        PlayerManager.Instance.UpdatePlayerRef(this);
        defaultMaterial = spriteRenderer.material;

        CreatePlayerUI();
    }

    void Update()
    {
        if (playerUI != null)
        {
            MovePlayerUI();
        }

        HandleAnimation();
    }

    void HandleAnimation()
    {
        anim.SetBool("isDashing", PlayerManager.Instance.isDashing);
        anim.SetBool("isDrilling", PlayerManager.Instance.isDrilling);
        anim.SetBool("isKnockedBack", PlayerManager.Instance.isKnockedBack);
        anim.SetBool("isGrounded", PlayerManager.Instance.isGrounded);
        anim.SetBool("isWalking", PlayerManager.Instance.isWalking);
        anim.SetBool("isTeleporting", PlayerManager.Instance.isTeleporting);
        anim.SetFloat("velocityX", PlayerManager.Instance.velocity.x);
        anim.SetFloat("velocityY", PlayerManager.Instance.velocity.y);
    }

    void CreatePlayerUI()
    {
        var PlayerUI = Instantiate(
            DamageUI.Instance.HealthBarPrefab,
            transform.position + (Vector3)DamageUI.Instance.healthBarOffset,
            Quaternion.identity,
            DamageUI.Instance.worldSpaceCanvas.transform);

        PlayerUI.name = "PlayerUI";
        playerUI = PlayerUI.GetComponent<PlayerUI>();
    }

    void MovePlayerUI()
    {
        playerUI.gameObject.transform.position = transform.position + (Vector3)DamageUI.Instance.healthBarOffset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MapSection"))
        {
            PlayerManager.Instance.currentMapSectionOccupied = collision.gameObject;
        }
    }

    public void HitFlash()
    {
        StartCoroutine(SpriteFlash(GameManager.Instance.hitFlashMaterial));
    }

    public void CollectGemFlash()
    {
        StartCoroutine(SpriteFlash(GameManager.Instance.collectGemMaterial));
    }

    public IEnumerator SpriteFlash(Material material)
    {
        spriteRenderer.material = defaultMaterial;

        var interval = 0.15f;

        if (spriteRenderer != null)
        {
            spriteRenderer.material = material;
            yield return new WaitForSeconds(interval);
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

}
