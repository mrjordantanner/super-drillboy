using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using DG.Tweening.Plugins;
using UnityEngine.InputSystem;
using JetBrains.Annotations;

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

    #region Declarations
    public PlayerInput playerInput;
    public bool followPlayerInSceneView;

    [Header("Options")]
    public bool autoDrill;
    bool autoDrillReleaseTimerEngaged;
    float autoDrillReleaseTimer;
    public float autoDrillReleaseDuration = 0.5f;
    public bool allowHorizontalDrilling;

    [Header("Player")]
    [ReadOnly]
    public GameObject PlayerPrefab;
    public Player player;
    public GameObject currentMapSectionOccupied;
    public Transform playerSpawnPoint;
    [ReadOnly]
    public Checkpoint lastCheckpointReached;
    [ReadOnly]
    public Vector2 lastSpawnPoint = new();

    [Header("Death VFX")]
    public GameObject PlayerDeathParticles;
    public GameObject PlayerDeathProjectile;
    [HideInInspector] public int numberOfProjectiles = 12;
    [HideInInspector] public float projectileSpeed = 12f;
    [HideInInspector] public float spreadAngle = 359f;

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
    public Vector2 directionalInput;
    [ReadOnly]
    public float currentDepth;

    [Header("Collisions")]
    [ReadOnly]
    public float slopeAngle;
    public bool isSliding;
    //public bool slideWasCanceled;
    [ReadOnly]
    public bool collisionsLeft;
    [ReadOnly]
    public bool collisionsRight;
    [ReadOnly]
    public bool collisionsBelow;
    [ReadOnly]
    public Vector2 slideDirection;
    [ReadOnly]
    public float slideDistance;
    public float slopeSlideSpeed = 15f;
    public float maxSlopeAngle = 60f;

    [Header("States")]
    public bool invulnerable;
    [ReadOnly]
    public bool
        canMove = true,
        isGrounded,
        wasGroundedLastFrame,
        isDashing,
        isBoosting,
        isDrilling,
        isKnockedBack,
        isWalking,
        isTeleporting,
        canDrillVert = true,
        canDrillHoriz = true,
        respawning,
        dead,
        facingRight,
        preventBoost,
        masterInvulnerability;

    // TRAILS
    [HideInInspector]
    public float
        defaultSpriteTrailRepeatRate = 0.04f,
        defaultSpriteTrailDuration = 0.15f;


    // PHYSICS
    [HideInInspector] public float 
        gravity = -9,
        fallMultiplier = 2.75f,
        terminalVelocity = -19f,
        dashAcceleration = 900,
        maxDashSpeed = 8,
        knockbackAmount = 3.5f,
        knockbackDuration = 0.2f,
        invulnerabilityDuration = 2f,

        // TODO move to SuperDrill.cs
        boostAcceleration = 1300,
        boostMaxSpeed = 23f;

    [HideInInspector]
    public Ease knockbackEasing = Ease.OutSine;

    // INTS
    //[HideInInspector] 
    public int 
        checkpointsReached,
        checkpointsUntilQuota;

    // MISC
    [HideInInspector] public float
        maxDepthReached;

    // MISC
    [HideInInspector] public float respawnBackgroundScrollSpeed = 0.35f;
    float velocityXSmoothing;
    float velocityYSmoothing = 1f;

    // CAMERA OFFSETS
    [HideInInspector] public float 
        normalCamOffset = 0.27f, 
        dashingCamOffset = 0.29f;

    // STUCK TIMER
    [HideInInspector] public float stuckTimer, stuckTimeBeforeHelpMessage = 7f;
    bool stuckTimerEnabled;
    bool stuckMessageShowing;
    string stuckMessage;

    public float 
        boostTrailRepeatRate = 0.03f,
        boostTrailDuration = 1f,
        boostingCamOffset = 0.38f;

    #endregion

    public void Init()
    {
        //switch (GameManager.Instance.gameMode)
        //{
        //    case GameMode.Adventure:
        //        lives = GameManager.Instance.adventureStartingLives;
        //        break;

        //    case GameMode.Survival:
        //        lives = GameManager.Instance.survivorStartingLives;
        //        break;
        
        //}

        // Add 1 so we don't change width on the starting checkpoint
        checkpointsUntilQuota = MapGenerator.Instance.changeWidthCheckpointQuota + 1;
        stuckMessage = GameManager.Instance.mobileMode ? "TAP TO RETURN TO CHECKPOINT" : "PRESS T TO RETURN TO CHECKPOINT";
        lastSpawnPoint = playerSpawnPoint.position;
    }

    public void UpdatePlayerRef(Player newPlayer)
    {
        player = newPlayer;
    }

    public void SetInitialState()
    {
        Stats.Instance.ResetHealth();
        Stats.Instance.ResetArmor(); 
        SkillController.Instance.ResetResource();
        SkillController.Instance.ResetSkillCharges();
        GemController.Instance.ResetGemDepthMultiplier();

        velocity.y = 0;
        GameManager.Instance.transposer.m_ScreenY = normalCamOffset;
 
        canMove = true;
        canDrillVert = true;
        canDrillHoriz = true;

        facingRight = false;
        invulnerable = false;
        dead = false;
        respawning = false;
        isDashing = false;
        isDrilling = false;
        isBoosting = false;
        isKnockedBack = false;
        isWalking = false;
        isTeleporting = false;
        
        if (player == null)
        {
            Debug.LogError("Tried to initialize Player object's properties but Player was null");
            return;
        }

        player.hitDetector.SetActive(true);
        player.drill.SetActive(true);
        player.spriteRenderer.enabled = true;
        player.trails.enabled = true;
        player.trails.repeatRate = defaultSpriteTrailRepeatRate;
        player.trails.duration = defaultSpriteTrailDuration;
        if (player.playerUI.gameObject != null) player.playerUI.gameObject.SetActive(true);

    }

    public void SpawnPlayer(Vector2 spawnPoint)
    {
        var existingPlayer = FindObjectOfType<Player>();
        if (existingPlayer != null) Destroy(existingPlayer.transform.gameObject);

        var PlayerObject = Instantiate(PlayerPrefab, spawnPoint, Quaternion.identity);
        UpdatePlayerRef(PlayerObject.GetComponent<Player>());
        GameManager.Instance.playerCam.Follow = PlayerObject.transform;
        PlayerObject.name = "Player";

        SetInitialState();
        StartCoroutine(DamageCooldown());

    }

    private void Update()
    {
        if (!player) return;

        MapGenerator.Instance.UpdateScrollingBackground(velocity);

        if (dead) return;

        isGrounded = player.controller.isGrounded;

        if (!isGrounded && wasGroundedLastFrame)
        {
            BecameAirborne();
        }
        else if (isGrounded && !wasGroundedLastFrame) 
        {
            BecameGrounded();
        }

        CalculateVelocityX();
        CalculateVelocityY();

        if (!GameManager.Instance.inputSuspended && !GameManager.Instance.gamePaused && !isKnockedBack)
        {
            HandleInput();
        }

        if (canMove && !GameManager.Instance.inputSuspended && !GameManager.Instance.gamePaused && !isKnockedBack)
        {
            HandleMovement();
        }

        //if (isBoosting)
        //{
        //    HandleBoostTimer();
        //}

        currentDepth = Utils.GetDistance(player.transform.position, playerSpawnPoint.transform.position);
        HUD.Instance.UpdateDepth(currentDepth);
        if (GameManager.Instance.gameMode == GameMode.Survival) GemController.Instance.CalculateGemMultiplier();

        if (stuckTimerEnabled)
        {
            HandleStuckTimer();
        }
    }

    private void LateUpdate()
    {
        wasGroundedLastFrame = isGrounded;
    }

    public void BecameGrounded()
    {
        //if (slideWasCanceled) slideWasCanceled = false;
        stuckTimerEnabled = true;
    }

    public void BecameAirborne()
    {
        //if (slideWasCanceled) slideWasCanceled = false;
        stuckTimerEnabled = false;
        stuckTimer = 0;

        if (stuckMessageShowing)
        {
            stuckMessageShowing = false;
            player.playerUI.ClearMessage();
        }
    }

    void HandleStuckTimer()
    {
        stuckTimer += Time.deltaTime;
        if (stuckTimer >= stuckTimeBeforeHelpMessage && !stuckMessageShowing)
        {
            stuckMessageShowing = true;
            player.playerUI.ShowMessage(stuckMessage, 5f);
        }
    }

    void CalculateVelocityX()
    {
        float targetVelocityX = directionalInput.x * Stats.Instance.MoveSpeed.Value;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, 0);
    }

    void CalculateVelocityY()
    {
        if (isKnockedBack || isTeleporting) return;

        float targetvelocityY = terminalVelocity;

        if (isDashing)
        {
            targetvelocityY -= dashAcceleration * Time.deltaTime;
            if (targetvelocityY < terminalVelocity - maxDashSpeed)
            {
                targetvelocityY = terminalVelocity - maxDashSpeed;
            }

            GameManager.Instance.transposer.m_ScreenY = dashingCamOffset;
        }
        else if (isBoosting)
        {
            targetvelocityY -= boostAcceleration * Time.deltaTime;
            if (targetvelocityY < terminalVelocity - boostMaxSpeed)
            {
                targetvelocityY = terminalVelocity - boostMaxSpeed;
            }

            GameManager.Instance.transposer.m_ScreenY = boostingCamOffset;
        }
        else
        {
            GameManager.Instance.transposer.m_ScreenY = normalCamOffset;
        }

        velocity.y = Mathf.SmoothDamp(velocity.y, targetvelocityY, ref velocityYSmoothing, 0.25f);

        if (isGrounded) velocity.y = 0;
    }

    void HandleMovement()
    {
        if (!GameManager.Instance.inputSuspended && canMove)
        {
            moveVector = velocity;
            player.controller.Move(moveVector * Time.deltaTime, directionalInput);
            isWalking = isGrounded && velocity.x != 0;
        }

        if (canMove && !GameManager.Instance.inputSuspended)
        {
            if (directionalInput.x > 0 && !facingRight)
                TurnAround();
            else if (directionalInput.x < 0 && facingRight)
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
        if (GameManager.Instance.inputSuspended || 
            isKnockedBack || dead || !player ||
            !ControlManager.Instance.joystick.locked) return;

        switch (Config.Instance.UseJoystick.Value)
        {
            // Get on-screen button input
            case 0:

                vert = ControlManager.Instance.drillButton.isButtonPressed ? -1 : 0;

                if (!isSliding)
                {
                    if (ControlManager.Instance.leftButton.isButtonPressed) horiz = -1;
                    else if (ControlManager.Instance.rightButton.isButtonPressed) horiz = 1;
                    else horiz = 0;
                }

                directionalInput = new Vector2(horiz, vert);

                break;

            // Get joystick input
            case 1:

                if (isSliding)
                {
                    directionalInput.y = playerInput.actions["MoveAndDrill"].ReadValue<Vector2>().y;
                }
                else
                {
                    directionalInput = playerInput.actions["MoveAndDrill"].ReadValue<Vector2>();
                }

                // Y axis value is never incremental and is always either -1 or 0
                directionalInput.y = directionalInput.y < 0 ? -1 : 0;

                // Allow incremental input on the X axis or not
                if (!ControlManager.Instance.joystick.isVariableSensitivityOn && !isSliding)
                {
                    if (directionalInput.x > 0) directionalInput.x = 1;
                    else if (directionalInput.x < 0) directionalInput.x = -1;
                }

                break;
        }

        // Get keyboard input
        vert = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? -1 : 0;

        if (!isSliding)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) horiz = -1;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) horiz = 1;
            else horiz = 0;
        }

        // Override joystick input with any keyboard input
        if (vert < 0) directionalInput.y = -1;
        if (horiz != 0 && !isSliding) directionalInput.x = horiz;

        // Drill / Teleport Input
        if (!isBoosting && !isTeleporting)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Teleport();
            }

            if (autoDrill)
            {
                HandleAutoDrill();
            }
            else
            {
                SetDrillState(directionalInput.y < 0);
            }

        }
    }

    void ReleaseAutoDrill()
    {
        autoDrillReleaseTimerEngaged = false;
        autoDrillReleaseTimer = 0;
        SetDrillState(false);
    }

    void HandleAutoDrill()
    {
        if (autoDrillReleaseTimerEngaged)
        {
            autoDrillReleaseTimer += Time.deltaTime;
            if (autoDrillReleaseTimer >= autoDrillReleaseDuration)
            {
                ReleaseAutoDrill();
            }
        }

        bool canAutoDrillVertical = false, canAutoDrillHorizontal = false;
        if (player.controller.verticalRaycastHit)
        {
            canAutoDrillVertical = player.controller.verticalRaycastHit.collider.CompareTag("Destructible") ||
                player.controller.verticalRaycastHit.collider.CompareTag("Nitro");
        }

        if (player.controller.horizontalRaycastHit)
        {
            canAutoDrillHorizontal = player.controller.horizontalRaycastHit.collider.CompareTag("Destructible") ||
                player.controller.horizontalRaycastHit.collider.CompareTag("Nitro");
        }

        if (canAutoDrillVertical || canAutoDrillHorizontal)
        {
            SetDrillState(true);
            autoDrillReleaseTimerEngaged = true;

        }
        else if (!autoDrillReleaseTimerEngaged)
        {
            SetDrillState(false);
        }
    }

    void SetDrillState(bool value)
    {
        player.trails.enabled = value;
        isDashing = value;
        isDrilling = value && isGrounded;
    }

    public void CollectArmor()
    {
        AudioManager.Instance.soundBank.CollectArmor.Play();
        var valueBeforeDamage = Stats.Instance.armor;

        Stats.Instance.armor++;
        if (Stats.Instance.armor > (int)Stats.Instance.MaxArmor.Value)
        {
            Stats.Instance.armor = (int)Stats.Instance.MaxArmor.Value;
        }

        HUD.Instance.hitpointsUI.RefreshWithPop(HUD.Instance.hitpointsUI.armorIcons, valueBeforeDamage);
        player.playerUI.ShowMessage("ARMOR +1");

    }

    //public void GainExtraLife()
    //{
    //    lives++;
    //    AudioManager.Instance.soundBank.CollectExtraLife.Play();
    //    player.playerUI.ShowMessage("LIVES +1");
    //}

    public void TakeDamage()
    {
        if (masterInvulnerability) return;

        var damage = 1;
        
        // Remove Armor
        if (Stats.Instance.armor > 0)
        {
            HUD.Instance.hitpointsUI.RefreshWithPop(HUD.Instance.hitpointsUI.armorIcons, Stats.Instance.armor);
            Stats.Instance.armor -= damage;
        }
        // Remove Health
        else
        {
            HUD.Instance.hitpointsUI.RefreshWithPop(HUD.Instance.hitpointsUI.healthIcons, Stats.Instance.health);
            Stats.Instance.health -= damage;
        }

        player.HitFlash();
        var hitSound = Stats.Instance.armor > 0 ? AudioManager.Instance.soundBank.HitArmor : AudioManager.Instance.soundBank.TakeDamage;
        hitSound.Play();
        GameManager.Instance.VibrateMobileDevice(GameManager.VibrationStyle.Medium);

        StartCoroutine(VFX.Instance.StartDamageEffects());  // TODO make damage effects less severe when armor hit?
        StartCoroutine(Knockback());

        if (Stats.Instance.health <= 0)
        {
            Stats.Instance.health = 0;
            StartCoroutine(PlayerDeath());
            return;
        }

        StartCoroutine(DamageCooldown());
    }

    public IEnumerator Knockback()
    {
        isKnockedBack = true;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.up, knockbackAmount, player.controller.collisionMask);

        Vector3 targetPosition = hit ? hit.point : player.transform.position + new Vector3(0, knockbackAmount, 0);
        float adjustedKnockbackDuration = hit ? knockbackDuration * (hit.distance / knockbackAmount) : knockbackDuration;
        var bgScrollSpeed = hit ? (Utils.GetDistance(hit.point, player.transform.position) * 4.3f) : knockbackAmount * 4.3f;

        player.transform.DOMove(targetPosition, adjustedKnockbackDuration).SetEase(knockbackEasing);
        velocity.y = bgScrollSpeed;
        yield return new WaitForSeconds(adjustedKnockbackDuration);

        isKnockedBack = false;
        velocity.y = 0;
    }

    public IEnumerator PlayerDeath()
    {
        AudioManager.Instance.soundBank.Die.Play();
        GameManager.Instance.VibrateMobileDevice(GameManager.VibrationStyle.Heavy);
        Instantiate(PlayerDeathParticles, player.transform.position, player.transform.rotation);
        SpawnDeathEffect();

        velocity.y = 0;

        player.spriteRenderer.enabled = false;
        player.trails.enabled = false;
        player.hitDetector.SetActive(false);
        player.drill.SetActive(false);

        dead = true;
        canMove = false;
        invulnerable = true;
        isBoosting = false;
        isTeleporting = false;

        player.playerUI.message.text = "";
        player.playerUI.message.alpha = 1;
        player.playerUI.gameObject.SetActive(false);

        HUD.Instance.ShowGemsChangedUIElement();
        SkillController.Instance.ResetResource();

        GemController.Instance.SyncAndResetGemCache();

        //lives--;
        //HUD.Instance.livesLabel.text = lives.ToString();
        //if (lives <= 0)
        //{
            maxDepthReached = Utils.GetDistance(player.transform.position, playerSpawnPoint.position);
            Destroy(player.gameObject);
            StartCoroutine(GameManager.Instance.GameOver());
            yield break;
        //}

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(RespawnPlayer());
    }

    public IEnumerator RespawnPlayer()
    {
        respawning = true;

        MovePlayerToRespawnPoint();
        yield return new WaitForSeconds(1.5f);

        lastSpawnPoint = lastCheckpointReached.transform.position;
        SetInitialState();
        //RespawnReset();
        StartCoroutine(DamageCooldown());
    }

    void MovePlayerToRespawnPoint()
    {
        var respawnPoint = lastCheckpointReached == null ? playerSpawnPoint.transform.position : lastCheckpointReached.transform.position;
        var distanceToRespawnPoint = Utils.GetDistance(player.transform.position, respawnPoint);
        var bgScrollSpeed = distanceToRespawnPoint * respawnBackgroundScrollSpeed;

        player.transform.DOMove(respawnPoint, 1.5f).SetEase(Ease.InOutCubic);
        velocity.y = bgScrollSpeed;
    }

    public IEnumerator DamageCooldown()
    {
        if (invulnerable) yield break;

        invulnerable = true;
        player.trails.enabled = false;
        player.spriteFlicker.On(invulnerabilityDuration);
        yield return new WaitForSeconds(invulnerabilityDuration);
        player.spriteFlicker.Off();
        player.trails.enabled = true;
        invulnerable = false;

        // Take damage again if still standing on damage block
        RaycastHit2D hitLeft = Physics2D.Raycast(player.controller.raycastOrigins.bottomLeft, Vector2.down, 0.1f, player.controller.collisionMask);
        RaycastHit2D hitRight = Physics2D.Raycast(player.controller.raycastOrigins.bottomRight, Vector2.down, 0.1f, player.controller.collisionMask);
        if (hitLeft.collider != null)
        {
            if (hitLeft.collider.CompareTag("Damage"))
            {
                TakeDamage();
            }
        }
        else if (hitRight.collider != null)
        {
            if (hitRight.collider.CompareTag("Damage"))
            {
                TakeDamage();
            }
        }

    }

    public void OnCheckpointReached()
    {
        checkpointsReached++;
        AudioManager.Instance.soundBank.CheckpointReached.Play();
        player.playerUI.ShowMessage("CHECKPOINT");

        GemController.Instance.SyncAndResetGemCache();
        PlayerData.Instance.SaveAllAsync();

        // Chance to change play area width after checkpointQuota number of checkpoints
        checkpointsUntilQuota--;
        if (checkpointsUntilQuota == 0)
        {
            checkpointsUntilQuota = MapGenerator.Instance.changeWidthCheckpointQuota;
            MapGenerator.Instance.AdjustShaftWidth();
        }
    }

    public void OnFinishLineReached()
    {
        //AudioManager.Instance.soundBank.FinishLineReached.Play();   // TODO
        player.playerUI.ShowMessage("FINISH");

        // TODO make some special transitions and/or VFX to ceremonialize this

        StartCoroutine(GameManager.Instance.GameOver());
        GemController.Instance.SyncAndResetGemCache();
        PlayerData.Instance.SaveAllAsync();
    }

    public void SpawnDeathEffect()
    {
        float angleBetweenBullets = spreadAngle / (numberOfProjectiles - 1);
        var initialBulletRotation = Quaternion.AngleAxis(spreadAngle / 2f, Vector3.forward) * transform.rotation * Quaternion.Euler(0f, 0f, 90f);

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            var bulletRotation = initialBulletRotation * Quaternion.AngleAxis(angleBetweenBullets * i, Vector3.forward);
            var ProjectileObject = Instantiate(PlayerDeathProjectile, player.transform.position, bulletRotation);
            ProjectileObject.transform.SetPositionAndRotation(player.transform.position, bulletRotation);
            var rb = ProjectileObject.GetComponent<Rigidbody2D>();
            rb.velocity = rb.transform.right * (projectileSpeed);
        }
    }

    public IEnumerator DrillCooldown(bool wasHorizontalRaycast)
    {
        if (wasHorizontalRaycast)
        {
            canDrillHoriz = false;
            yield return new WaitForSeconds(Stats.Instance.DrillSpeed.Value);
            canDrillHoriz = true;
        }
        else
        {
            canDrillVert = false;
            yield return new WaitForSeconds(Stats.Instance.DrillSpeed.Value);
            canDrillVert = true;
        }
        yield return new WaitForSeconds(0);
    }
   
    public IEnumerator EmergencyTeleport()
    {
        if (stuckMessageShowing)
        {
            stuckMessageShowing = false;
            player.playerUI.ClearMessage();
        }

        player.playerUI.ShowMessage("TELEPORTING TO CHECKPOINT");
        isTeleporting = true;
        stuckTimerEnabled = false;
        stuckTimer = 0;

        velocity.y = 0;

        player.trails.enabled = false;
        player.hitDetector.SetActive(false);
        player.drill.SetActive(false);

        canMove = false;
        invulnerable = true;
        isBoosting = false;
        player.trailRenderer.emitting = false;

        MovePlayerToRespawnPoint();
        yield return new WaitForSeconds(1.5f);

        canMove = true;
        isBoosting = false;
        isTeleporting = false;
        invulnerable = false;
        player.hitDetector.SetActive(true);
        player.drill.SetActive(true);
        player.spriteRenderer.enabled = true;
        player.trails.enabled = true;
        player.trails.repeatRate = defaultSpriteTrailRepeatRate;
        player.trails.duration = defaultSpriteTrailDuration;
        player.trailRenderer.emitting = false;

        velocity.y = 0;
        GameManager.Instance.transposer.m_ScreenY = normalCamOffset;

        StartCoroutine(DamageCooldown());
    }

    public void Teleport()
    {
        if (isBoosting || isTeleporting) return;
        StartCoroutine(EmergencyTeleport());
    }

    public void PlayerDestroyBlock(Collider2D collision)
    {
        var shakeStyle = isBoosting ? CameraShaker.ShakeStyle.Large : CameraShaker.ShakeStyle.Medium;
        CameraShaker.Instance.Shake(shakeStyle);
        GameManager.Instance.VibrateMobileDevice(GameManager.VibrationStyle.Light);

        var block = collision.gameObject.GetComponent<Block>();
        block.DestroyBlock();
        LevelingSystem.Instance.GainXP(1);

        //if (isSliding) CancelSlide();
    }

    //void CancelSlide()
    //{
    //    slideWasCanceled = true;
    //    isSliding = false;
    //    //velocity.x = 0;
    //}



}
