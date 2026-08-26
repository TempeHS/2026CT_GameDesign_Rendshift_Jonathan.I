using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Base Movement")]
    public float baseSpeed = 8f;
    public float jumpingPower = 16f;

    [Header("Momentum System")]
    public float groundMaxMomentum = 3.21f;
    public float airMaxMomentum = 3.02f;

    [Header("Jumping")]
    public int maxJumps = 2;
    public bool inNoJumpZone = false;

    [Header("Wall Slide / Jump")]
    public Transform wallCheck;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    public float wallJumpDuration = 0.2f;
    public Vector2 wallJumpForce = new Vector2(12f, 16f);

    [Header("Dash")]
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    [Header("References")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public TrailRenderer tr;
    public ParticleSystem jumpParticles;

    [Header("Dash Sprites")]
    public SpriteRenderer playerSprite;
    public Sprite greenSprite;
    public Sprite redSprite;

    [HideInInspector] public bool movementLocked = false;

    // internal state
    float horizontal;
    bool isFacingRight = true;
    float momentum = 1f;
    float holdTime = 0f;
    int lastMoveDir = 0;
    int jumpCount = 0;
    bool isWallSliding = false;
    bool isWallJumping = false;
    bool isDashing = false;
    bool canDash = true;
    Coroutine dashRoutine = null;

    void Reset() => rb = GetComponent<Rigidbody2D>();

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (playerSprite != null && greenSprite != null) playerSprite.sprite = greenSprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UnfreezePlayer();
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
            return;
        }

        if (movementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            if (Input.GetButtonDown("Jump")) Input.ResetInputAxes();
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        HandleMomentum();
        HandleWallSlide();
        HandleWallJump();

        if (!inNoJumpZone &&
            Input.GetButtonDown("Jump") &&
            !isWallSliding &&
            jumpCount < maxJumps)
        {
            momentum *= 0.9f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            if (jumpParticles != null) { jumpParticles.Stop(); jumpParticles.Play(); }
            jumpCount++;
        }

        if (IsGrounded() && rb.linearVelocity.y <= 0.01f) jumpCount = 0;

        if (!inNoJumpZone && Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            dashRoutine = StartCoroutine(Dash());

        Flip();
    }

    void FixedUpdate()
    {
        if (movementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDashing) return;

        if (!isWallJumping)
        {
            float speed = baseSpeed;
            if (!IsGrounded()) speed *= 1.04f;
            rb.linearVelocity = new Vector2(horizontal * speed * momentum, rb.linearVelocity.y);
        }
    }

    void HandleMomentum()
    {
        int moveDir = horizontal > 0 ? 1 : horizontal < 0 ? -1 : 0;
        float maxMomentum = IsGrounded() ? groundMaxMomentum : airMaxMomentum;

        if (lastMoveDir != 0 && moveDir != lastMoveDir)
        {
            momentum *= Mathf.Exp(-10f * Time.deltaTime);
            holdTime = 0f;
            lastMoveDir = moveDir;
            return;
        }

        if (moveDir == 0)
        {
            momentum *= Mathf.Exp(-10f * Time.deltaTime);
            holdTime = 0f;
            lastMoveDir = 0;
            return;
        }

        holdTime += Time.deltaTime;
        float t = holdTime;
        float curve = Mathf.Pow(1f - Mathf.Exp(-1.25f * t), 0.75f);
        float target = 1f + (maxMomentum - 1f) * curve;

        momentum = Mathf.Lerp(momentum, target, 0.5f);
        momentum = Mathf.Clamp(momentum, 1f, maxMomentum);

        lastMoveDir = moveDir;
    }

    void HandleWallSlide()
    {
        if (IsWalled() && !IsGrounded() && Mathf.Abs(horizontal) > 0.1f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else isWallSliding = false;
    }

    void HandleWallJump()
    {
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;
            momentum *= 0.9f;
            float direction = isFacingRight ? -1f : 1f;
            rb.linearVelocity = new Vector2(direction * wallJumpForce.x, wallJumpForce.y);
            if (jumpParticles != null) { jumpParticles.Stop(); jumpParticles.Play(); }
            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    void StopWallJump() => isWallJumping = false;

    IEnumerator Dash()
    {
        if (inNoJumpZone) yield break;

        canDash = false;
        isDashing = true;

        if (playerSprite != null && redSprite != null) playerSprite.sprite = redSprite;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 dashDir = new Vector2(x, y);
        if (dashDir == Vector2.zero) dashDir = new Vector2(isFacingRight ? 1f : -1f, 0f);
        dashDir.Normalize();

        float dashStrength = dashPower;
        if (dashDir.x != 0 && dashDir.y != 0) dashStrength *= 0.75f;
        else if (dashDir.y > 0) dashStrength *= 0.60f;

        if (tr != null) tr.emitting = true;

        float t = 0f;
        while (t < dashTime)
        {
            t += Time.deltaTime;
            float ease = Mathf.Lerp(1.35f, 0.65f, t / dashTime);
            rb.linearVelocity = dashDir * dashStrength * ease;

            if (Input.GetButtonDown("Jump"))
            {
                isDashing = false;
                rb.gravityScale = originalGravity;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                if (jumpParticles != null) { jumpParticles.Stop(); jumpParticles.Play(); }
                break;
            }

            yield return null;
        }

        if (tr != null) tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        ApplyDashMomentum(dashDir);

        yield return new WaitForSeconds(dashCooldown);

        if (playerSprite != null && greenSprite != null) playerSprite.sprite = greenSprite;

        canDash = true;
    }

    void ApplyDashMomentum(Vector2 dashDir)
    {
        bool sameDir = (isFacingRight && dashDir.x > 0) || (!isFacingRight && dashDir.x < 0);
        bool slightAngle = sameDir && Mathf.Abs(dashDir.y) > 0;

        if (sameDir)
        {
            if (slightAngle) momentum *= 0.9f;
            else momentum *= 1.1f;
        }
        else momentum *= Mathf.Exp(-12f * Time.deltaTime);

        float maxMomentum = IsGrounded() ? groundMaxMomentum : airMaxMomentum;
        momentum = Mathf.Clamp(momentum, 1f, maxMomentum);
    }

    public bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    bool IsWalled()
    {
        if (wallCheck == null) return false;
        return Physics2D.OverlapCircle(wallCheck.position, 0.3f, wallLayer);
    }

    void Flip()
    {
        if (isWallJumping) return;
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    public void FreezePlayer()
    {
        Input.ResetInputAxes();
        movementLocked = true;

        if (dashRoutine != null) { StopCoroutine(dashRoutine); dashRoutine = null; }

        isDashing = false;
        isWallJumping = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (tr != null) tr.emitting = false;
        if (playerSprite != null && greenSprite != null) playerSprite.sprite = greenSprite;
    }

    public void UnfreezePlayer()
    {
        Input.ResetInputAxes();
        movementLocked = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 3f;
    }
}
