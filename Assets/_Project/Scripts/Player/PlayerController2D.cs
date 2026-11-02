using System.Collections;
using UnityEngine;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Input;
using _Project.Scripts.Core.Item;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Stats = _Project.Scripts.Core.Actor.Stats;

namespace _Project.Scripts.Player
{
    public sealed class PlayerController2D : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GroundCheck groundCheck;
        [SerializeField] private SpriteRenderer sprite;

        [Header("Move")]
        [SerializeField] private float moveSpeed = 6f;
        
        [Header("Jump")]
        [SerializeField] private float jumpImpulse = 8f;

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 12f;
        [SerializeField] private float dashDuration = 0.12f;
        [SerializeField] private float dashCooldown = 0.6f;

        private ICoreInputReader input;
        private Stats stats;
        private Item currentItem;
        private PlayerCombat playerCombat; 
        
        private bool isDashing;
        private float nextDashTime;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>(true);
            if (groundCheck == null) groundCheck = GetComponentInChildren<GroundCheck>(true);

            if (rb != null) rb.freezeRotation = true;
        }

        private void Start()
        {
            if (App.Instance == null || App.Instance.Services == null)
            {
                Debug.LogError("[PlayerController2D] App/Services not ready.", this);
                enabled = false;
                return;
            }
            
            var player = GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("[PlayerController2D] Player component not found.", this);
                enabled = false;
                return;
            }

            stats = player.Stats;
            playerCombat = player.Combat;

            input = App.Instance.Services.Get<ICoreInputReader>();
            if (input == null)
            {
                Debug.LogError("[PlayerController2D] ICoreInputReader not found in Services. Check App.Compose() registration.", this);
                enabled = false;
                return;
            }

            input.JumpPressed += OnJumpPressed;
            input.DashPressed += OnDashPressed;
            input.ReloadPressed += OnReloadPressed;
            input.SwitchWeaponPressed += OnSwitchWeaponPressed;
            input.InteractPressed += OnInteractPressed;
        }

        private void OnDestroy()
        {
            if (input == null) return;
            input.JumpPressed -= OnJumpPressed;
            input.DashPressed -= OnDashPressed;
            input.ReloadPressed -= OnReloadPressed;
            input.SwitchWeaponPressed -= OnSwitchWeaponPressed;
            input.InteractPressed -= OnInteractPressed;
        }

        private void FixedUpdate()
        {
            if (rb == null || input == null) return;
            if (isDashing) return;

            // По SRS: управление только на земле
            /*
            if (groundCheck != null && !groundCheck.IsGrounded)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }
            */
            float speedMultiplier = stats != null ? stats.MoveSpeedMultiplier : 1f;
            float vx = input.MoveX * moveSpeed * speedMultiplier;
            rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
            
            if (input.AttackHeld)
                playerCombat?.Attack();

            if (sprite != null)
            {
                if (vx > 0.01f) sprite.flipX = false;
                else if (vx < -0.01f) sprite.flipX = true;
            }
        }

        private void OnJumpPressed()
        {
            if (rb == null) return;
            if (groundCheck != null && !groundCheck.IsGrounded) return;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            
            playerCombat.CancelReloadIfAny();
        }

        private void OnDashPressed()
        {
            if (rb == null) return;
            if (Time.time < nextDashTime) return;
            if (groundCheck == null) return;

            StartCoroutine(DashRoutine());
            playerCombat.CancelReloadIfAny();
        }
        private void OnReloadPressed() => playerCombat?.ReloadCurrentWeapon();
        private void OnSwitchWeaponPressed() => playerCombat?.SwitchSlot();
        
        private void OnInteractPressed()
        {
            Debug.Log("Interact");

            if (currentItem == null)
            {
                Debug.Log("No item nearby");
                return;
            }

            // Доп. защита: предмет мог уже уничтожиться/отключиться
            if (!currentItem.gameObject.activeInHierarchy)
            {
                currentItem = null;
                return;
            }

            var player = GetComponent<Player>();
            currentItem.Interact(player);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Важно: Item может быть на родителе, а collider на child
            var item = other.GetComponentInParent<Item>();
            if (item == null)
                return;

            Debug.Log($"Item detected: {item.name}");

            // Если уже стоим в другом предмете — выберем ближайший (MVP логика)
            if (currentItem == null)
            {
                currentItem = item;
                return;
            }

            float dNew = (item.transform.position - transform.position).sqrMagnitude;
            float dCur = (currentItem.transform.position - transform.position).sqrMagnitude;

            if (dNew < dCur)
                currentItem = item;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var item = other.GetComponentInParent<Item>();
            if (item == null)
                return;

            // Если у предмета несколько коллайдеров — выход одного не должен сбрасывать, если мы всё ещё внутри другого.
            // MVP-упрощение: сбрасываем только если именно этот item был текущим.
            if (currentItem == item)
                currentItem = null;
        }


        private IEnumerator DashRoutine()
        {
            isDashing = true;
            nextDashTime = Time.time + dashCooldown;

            float dir = (sprite != null && sprite.flipX) ? -1f : 1f;

            float end = Time.time + dashDuration;
            while (Time.time < end)
            {
                rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
                yield return new WaitForFixedUpdate();
            }

            isDashing = false;
        }
    }
}
