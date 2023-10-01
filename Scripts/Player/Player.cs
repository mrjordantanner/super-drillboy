using Assets.Scripts;
using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    public GameObject PlayerGraphics;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public SpriteFlicker spriteFlicker;
    [HideInInspector]
    public SpriteTrails trails;

    [HideInInspector]
    public HitpointsUI hitpointsUI;
    [HideInInspector]
    public ProgressBar StaminaBar;
    [HideInInspector]
    public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<Controller2D>();
        spriteFlicker = GetComponentInChildren<SpriteFlicker>();
        trails = GetComponentInChildren<SpriteTrails>();

        PlayerManager.Instance.UpdatePlayerRef(this);
        PlayerManager.Instance.SetProperties();

        CreateHealthUI();
        //CreateStaminaBar();
    }


    void Update()
    {
        if (hitpointsUI != null)
        {
            MoveHealthUI();
        }

        if (StaminaBar != null)
        {
            MoveStaminaBar();
        }

        HandleAnimation();
    }

    void HandleAnimation()
    {
        anim.SetBool("isDashing", PlayerManager.Instance.isDashing);
        anim.SetBool("isBraking", PlayerManager.Instance.isBraking);
        anim.SetBool("isIdle", controller.grounded);
        anim.SetFloat("VelocityY", PlayerManager.Instance.velocityY);
    }

    void MoveHealthUI()
    {
        hitpointsUI.gameObject.transform.position = transform.position + (Vector3)DamageUI.Instance.healthBarOffset;
    }

    void MoveStaminaBar()
    {
        StaminaBar.gameObject.transform.position = transform.position + (Vector3)DamageUI.Instance.healthBarOffset - new Vector3(0, 0.5f, 0);
    }

    void CreateHealthUI()
    {
        var HealthBarObj = Instantiate(
            DamageUI.Instance.HealthBarPrefab,
            transform.position + (Vector3)DamageUI.Instance.healthBarOffset,
            Quaternion.identity,
            DamageUI.Instance.worldSpaceCanvas.transform);

        HealthBarObj.name = $"Healthbar_{Utils.ShortName(name)}";
        hitpointsUI = HealthBarObj.GetComponent<HitpointsUI>();

        hitpointsUI.hitpoints = PlayerManager.Instance.health;
        hitpointsUI.maxHitpoints = PlayerManager.Instance.maxHealth;
        hitpointsUI.RefreshHitpoints(PlayerManager.Instance.health);
    }

    //void CreateStaminaBar()
    //{
    //    var StaminaBarObj = Instantiate(
    //        DamageUI.Instance.StaminaBarPrefab,
    //        transform.position + (Vector3)DamageUI.Instance.healthBarOffset - new Vector3(0, 0.75f, 0),
    //        Quaternion.identity,
    //        DamageUI.Instance.worldSpaceCanvas.transform);

    //    StaminaBarObj.name = $"StaminaBar_{Utils.ShortName(name)}";
    //    StaminaBar = StaminaBarObj.GetComponent<ProgressBar>();
    //    SetStaminaBar(PlayerManager.Instance.stamina);

    //}

    //public void SetStaminaBar(float stamina)
    //{
    //    StaminaBar.cooldown = stamina;
    //    StaminaBar.slider.maxValue = 1f;
    //    StaminaBar.slider.value = 1f;
    //}

    public void PlayerDestroyObject(Collider2D collision)
    {
        if (PlayerManager.Instance.isSuperDashing)
        {
            CameraShaker.Instance.Shake(CameraShaker.ShakeStyle.Large);
        }
        else
        {
            CameraShaker.Instance.Shake(CameraShaker.ShakeStyle.Medium);
        }

        var block = collision.gameObject.GetComponent<Block>();
        block.DestroyBlock();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            PlayerManager.Instance.CollectCoin();
            Destroy(collision.gameObject);
        }
    }

    public IEnumerator HitFlash()
    {
        StopCoroutine(HitFlash());

        var interval = 0.1f;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(interval);
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }
}
