using UnityEngine;

namespace _Project.Scripts.Weapon
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Transform[] muzzles;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [field: SerializeField] public bool IsLeadWeapon { get; set; } = true;
        [field: SerializeField] public WeaponView PairedWeapon { get; set; }

        private RangedWeapon weapon;
        private bool isPreview;

        public Transform[] Muzzles => muzzles;
        
        private void Update()
        {
            if (isPreview || weapon == null)
                return;

            Vector2 targetPos = weapon.TargetPosition;

            if (IsLeadWeapon)
            {
                RotateTowardsTarget(targetPos);
                if (PairedWeapon != null)
                {
                    PairedWeapon.CopyRotationFrom(this);
                }
            }
        }
        
        private void RotateTowardsTarget(Vector2 targetPos)
        {
            Vector2 direction = targetPos - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = Mathf.Abs(angle) > 90f;
            }
        }
        
        private void CopyRotationFrom(WeaponView lead)
        {
            transform.rotation = lead.transform.rotation;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = lead.spriteRenderer.flipY;
            }
        }

        public void Bind(RangedWeapon weapon)
        {
            if (isPreview)
                return;

            this.weapon = weapon;
            weapon.OnShot += HandleShot;
        }

        private void OnDestroy()
        {
            if (weapon != null)
                weapon.OnShot -= HandleShot;
        }

        private void HandleShot()
        {
            if (isPreview)
                return;

            FireProjectiles();
        }
        
        private void FireProjectiles()
        {
            Vector2 target = weapon.TargetPosition;
            foreach (var muzzle in muzzles)
            {
                Vector2 baseDir = (target - (Vector2)muzzle.position).normalized;

                GameObject proj = Instantiate(
                    weapon.definition.projectileDefinition.prefab,
                    muzzle.position,
                    Quaternion.identity
                );
        
                float angle = UnityEngine.Random.Range(
                    -weapon.definition.spreadAngle,
                    weapon.definition.spreadAngle
                );

                Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

                proj.GetComponent<Projectile.Projectile>()
                    .Init(dir, weapon.definition.projectileDefinition, weapon.definition.damage, weapon.Owner);
            }
        }
        
        public void SetAsPreview()
        {
            isPreview = true;

            if (weapon != null)
            {
                weapon.OnShot -= HandleShot;
                weapon = null;
            }

            if (muzzles != null)
            {
                foreach (var muzzle in muzzles)
                {
                    if (muzzle != null)
                        muzzle.gameObject.SetActive(false);
                }
            }
            transform.localRotation = Quaternion.identity;
        }
        
        public void SetDisabled(bool disabled)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.color = disabled
                ? new Color(1f, 1f, 1f, 0.4f)
                : Color.white;
        }
    }

}