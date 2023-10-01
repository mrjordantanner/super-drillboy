using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Assets.Scripts;
using System.Drawing;


public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        #endregion

        Init();
    }

    [HideInInspector]
    public Player player;

    [Header("Vector Data")]
    [ReadOnly]
    public float horiz;
    [ReadOnly]
    public float vert;
    [ReadOnly]
    public Vector2 moveVector;
    [ReadOnly]
    public Vector2 velocity;
    [ReadOnly]
    public float velocityY, velocityX;
    [ReadOnly]
    public Vector2 directionalInput;

    [Header("Physics")]
    public float gravity = -9;
    public float fallMultiplier = 2.75f;
    public float terminalVelocity = -20f;
    public float moveSpeed = 9;
    public float dashAcceleration = 1000;
    public float maxDashSpeed = 5;
    public float brakeDeceleration = 500;
    public float maxBrakeSpeed = 10;
    public float knockbackAmount = 5f;
    public float knockbackDuration = 0.2f;

    [Header("Stats")]
    public int startingHealth = 3;
    public int health;
    public int maxHealth;
    public int lives, startingLives = 5;
    //public float
    //    stamina = 100,
    //    maxStamina = 100,
    //    staminaRechargeRate = 5,
    //    staminaUseRate = 15;
    public float invulnerabilityDuration = 1f;
    public float superDashDuration = 10f,
        superDashAcceleration = 1500, 
        superDashMaxSpeed = 15f;
    public int checkpointsReached;
    public int checkpointQuota = 5;
    int checkpointsUntilQuota;

    public int coins;
    public int coinQuota = 50;

    [HideInInspector]
    public float maxDepthReached;

    [Header("States")]
    public bool invulnerable;
    [ReadOnly]
    public bool
        canMove = true,
        isDashing,
        isSuperDashing,
        isBraking,
        canBrake = true,
        respawning,
        dead,
        facingRight;

    float velocityXSmoothing;
    float velocityYSmoothing = 1f;
    readonly float accelerationTimeAirborne = 0;
    readonly float airborneAcceleration = 0.25f;

    public void Init()
    {
        SetProperties();
        lives = startingLives;
        checkpointsUntilQuota = checkpointQuota;
    }

    public void UpdatePlayerRef(Player newPlayer)
    {
        player = newPlayer;
    }

    public void SetProperties()
    {
        health = maxHealth = startingHealth;
       // stamina = maxStamina;

        canMove = true;
        invulnerable = false;
        dead = false;
        respawning = false;
        isBraking = false;
        isDashing = false;
        isSuperDashing = false;
        canBrake = true;
    }

    private void Update()
    {
        if (player == null || dead) return;

        CalculateVelocityX();
        CalculateVelocityY();

        if (!GameManager.Instance.inputSuspended && !GameManager.Instance.gamePaused)
        {
            HandleInput();
        }

        if (canMove && !GameManager.Instance.inputSuspended && !GameManager.Instance.gamePaused)
        {
            HandleMovement();
        }

        HUD.Instance.UpdateVelocity(velocityY);
        HUD.Instance.UpdateDepth(Utils.GetDistance(player.transform.position, GameManager.Instance.playerSpawnPoint.transform.position));
    }

    void CalculateVelocityX()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeAirborne);
        velocityX = velocity.x;
    }

    void CalculateVelocityY()
    {
        float targetVelocityY = terminalVelocity;

        if (isBraking)
        {
            targetVelocityY += brakeDeceleration * Time.deltaTime;
            if (targetVelocityY > terminalVelocity + maxBrakeSpeed)
            {
                targetVelocityY = terminalVelocity + maxBrakeSpeed;
            }
        }

        if (isDashing)
        {
            targetVelocityY -= dashAcceleration * Time.deltaTime;
            if (targetVelocityY < terminalVelocity - maxDashSpeed)
            {
                targetVelocityY = terminalVelocity - maxDashSpeed;
            }
        }

        if (isSuperDashing)
        {
            targetVelocityY -= superDashAcceleration * Time.deltaTime;
            if (targetVelocityY < terminalVelocity - superDashMaxSpeed)
            {
                targetVelocityY = terminalVelocity - superDashMaxSpeed;
            }
        }

        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, airborneAcceleration);
        velocityY = velocity.y;

        if (player.controller.grounded)
        {
            velocityY = 0;
            velocity.y = 0;
        }
    }

    void HandleMovement()
    {
        if (!GameManager.Instance.inputSuspended && canMove)
        {
            moveVector = new Vector2(velocityX, velocityY);
            player.controller.Move(moveVector * Time.deltaTime, directionalInput);
        }

        if (canMove && !GameManager.Instance.inputSuspended)
        {
            if (horiz > 0 && !facingRight)
                TurnAround();
            else if (horiz < 0 && facingRight)
                TurnAround();
        }
    }

    void TurnAround()
    {
        facingRight = !facingRight;
        Vector3 scale = player.transform.localScale;
        scale.x *= -1;
        player.transform.localScale = scale;
  
    }

    void HandleInput()
    {
        if (!GameManager.Instance.inputSuspended)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                horiz = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                horiz = 1;
            }
            else
            {
                horiz = 0;
            }

            directionalInput = new(horiz, vert);
        }

        if (!isSuperDashing)
        {
            // Dash downwards
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) && !isBraking)
            {
                isDashing = true;
            }
            else
            {
                isDashing = false;
            }

            // Brake upwards
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) && !isDashing && canBrake && !player.controller.grounded)
            {
                isBraking = true;
            }
            else
            {
                isBraking = false;
            }
        }

        // Release brake
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            canBrake = true;
        }
    }


    //void HandleStamina()
    //{
    //    if (GameManager.Instance.gameRunning && !GameManager.Instance.gamePaused && !isBraking)
    //    {
    //        stamina += staminaRechargeRate * Time.deltaTime;
    //        if (stamina >= maxStamina)
    //        {
    //            stamina = maxStamina;
    //        }

    //    }

    //    if (GameManager.Instance.gameRunning && !GameManager.Instance.gamePaused && isBraking)
    //    {
    //        stamina -= staminaUseRate * Time.deltaTime;
    //        if (stamina <= 0)
    //        {
    //            stamina = 0;
    //            isBraking = false;
    //            canBrake = false;
    //        }
    //    }

    //    player.StaminaBar.slider.value = stamina / maxStamina;
    //}


    public void CollectCoin()
    {
        AudioManager.Instance.Play("Collect-Coin");
        coins++;
        if (coins >= coinQuota)
        {
            coins -= coinQuota;
            StartCoroutine(StartSuperDash());
        }
        HUD.Instance.coinsLabel.text = coins.ToString();
    }

    public void TakeDamage(float velocityY)
    {
        if (GameManager.Instance.masterInvulnerability)
        {
            print("Player immune to damage due to MasterInvulnerability");
            return;
        }
        var damage = 1;

        health -= damage;
        player.hitpointsUI.RefreshHitpoints(health);

        StartCoroutine(player.HitFlash());
        AudioManager.Instance.Play("Player-Hurt");

        HUD.Instance.ScreenFlash();
        Knockback();

        if (health <= 0)
        {
            health = 0;
            StartCoroutine(PlayerDeath());
        }

        StartCoroutine(DamageCooldown());
    }

    void Knockback()
    {
        var value = player.transform.position + new Vector3(0, knockbackAmount, 0);
        player.transform.DOMove(value, knockbackDuration);
    }

    IEnumerator PlayerDeath()
    {
        AudioManager.Instance.Play("Player-Death");
        Instantiate(GameManager.Instance.PlayerDeathVFX, player.transform.position, player.transform.rotation);

        player.spriteRenderer.enabled = false;
        player.trails.enabled = false;
        dead = true;
        canMove = false;
        invulnerable = true;
        Destroy(player.hitpointsUI.gameObject);
        //Destroy(player.StaminaBar.gameObject);

        yield return new WaitForSeconds(2f);

        lives--;
        HUD.Instance.livesRemaining.text = lives.ToString();
        if (lives == 0)
        {
            maxDepthReached = Utils.GetDistance(player.transform.position, GameManager.Instance.playerSpawnPoint.position);
            Destroy(player.gameObject);
            StartCoroutine(GameManager.Instance.GameOver());
            yield break;
        }

        Vector3 respawnPoint;
        if (GameManager.Instance.lastCheckpointReached == null)
        {
            respawnPoint = GameManager.Instance.playerSpawnPoint.transform.position;
        }
        else
        {
            respawnPoint = GameManager.Instance.lastCheckpointReached.transform.position;
        }

        respawning = true;
        GameManager.Instance.SpawnPlayer(respawnPoint);
    }

    public IEnumerator DamageCooldown()
    {
        if (invulnerable) yield break;

        invulnerable = true;
        player.spriteFlicker.On(invulnerabilityDuration);
        yield return new WaitForSeconds(invulnerabilityDuration);
        player.spriteFlicker.Off();
        invulnerable = false;
    }

  

    public void OnCheckpointReached()
    {
        HUD.Instance.ShowMessage("Checkpoint Reached", 1.5f);
        AudioManager.Instance.Play("Checkpoint-Reached");
        checkpointsReached++;
        HUD.Instance.checkpointsReached.text = checkpointsReached.ToString();

        // Reduce play area width after certain number of checkpoints
        checkpointsUntilQuota--;
        if (checkpointsUntilQuota == 0)
        {
            checkpointsUntilQuota = checkpointQuota;
            MapController.Instance.ReduceWallWidth();
        }
    }

    public IEnumerator StartSuperDash()
    {
        HUD.Instance.ShowMessage("Turbo Break!", 2f);
        AudioManager.Instance.Play("Super-Dash");

        var oldRepeatRate = player.trails.repeatRate;

        invulnerable = true;
        isSuperDashing = true;
        player.trails.repeatRate = 0.01f;

        player.anim.SetBool("isSuperDashing", true);
        yield return new WaitForSeconds(superDashDuration);
        isSuperDashing = false;
        invulnerable = false;
        player.trails.repeatRate = oldRepeatRate;
        player.anim.SetBool("isSuperDashing", false);
    }
}

