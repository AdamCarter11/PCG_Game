using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float variableJumpHeightMultiplier = 0.5f;
    [SerializeField] int maxAirJumps = 1;
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float dashSpeedMultiplier = 2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Tilemap groundTileMap, caveTileMap;
    [SerializeField] TileBase caveTileBase;
    [SerializeField] float mouseDistanceToBreak;

    private Rigidbody2D rb;
    private int airJumpsRemaining;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool canDash = true;
    private bool canMove = true;

    Vector2 startingPos;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        startingPos = transform.position;
    }

    private void Update()
    {
        if(transform.position.y <= 0)
        {
            transform.position = startingPos;
        }
        if (canMove)
        {
            
            float moveInput = Input.GetAxis("Horizontal");
            float moveSpeedMultiplier = Input.GetKeyDown(KeyCode.LeftShift) ? dashSpeedMultiplier : 1f;

            Vector2 movement = new Vector2(moveInput * moveSpeed * moveSpeedMultiplier, rb.velocity.y);
            rb.velocity = movement;
        }

        isGrounded = IsGrounded();
        TileDestruction();

        if (isGrounded)
        {
            airJumpsRemaining = maxAirJumps;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (airJumpsRemaining > 0)
            {
                Jump();
                airJumpsRemaining--;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Dash();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = true;
    }

    private void FixedUpdate()
    {
        if (isJumping)
        {
            if (rb.velocity.y > 0 && !Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (variableJumpHeightMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.velocity.y <= 0)
            {
                isJumping = false;
            }
        }
    }

    private void Dash()
    {
        Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        rb.velocity = dashDirection * dashForce;
        canDash = false;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.01f;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, .5f, Vector2.down, gameObject.GetComponent<Collider2D>().bounds.extents.y + extraHeight, groundLayer);
        return hit.collider != null;
    }
    private void TileDestruction()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Vector3.Distance(mouseWorldPos, transform.position) <= mouseDistanceToBreak)
            {
                Vector3Int cellPos = groundTileMap.WorldToCell(mouseWorldPos);

                if (groundTileMap.HasTile(cellPos))
                {
                    groundTileMap.SetTile(cellPos, null);
                    caveTileMap.SetTile(cellPos, caveTileBase);
                }
            }
            
        }
    }
}
