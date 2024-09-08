using UnityEngine;

public class Tower : Building
{
    [Header("Tower Stats")]
    public float attackRange = 5f; // Default attack range
    public float attackRate = 1f;  // Default attack speed
    public int attackDamage = 10;  // Default attack damage
    public GameObject arrowPrefab; // Default projectile (arrow)

    private float attackCooldown = 0f;
    private Transform targetEnemy;

    public enum TowerElement
    {
        Normal,
        Fire,
        Water,
        Air,
        Earth
    }

    [Header("Tower Type")]
    public TowerElement towerElement = TowerElement.Normal; // Default tower type

    private void Start()
    {
        // Kule etrafýnda düþmanlarý tespit etmek için bir Collider ekliyoruz.
        SphereCollider rangeCollider = gameObject.AddComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = attackRange;
    }

    private void Update()
    {
        base.Update(); // Call the base building update (for gathering)

        attackCooldown -= Time.deltaTime;
        if (targetEnemy != null && attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = attackRate;
        }
    }

    private void Attack()
    {
        if (targetEnemy == null) return;

        GameObject projectile = Instantiate(GetProjectilePrefab(), transform.position, Quaternion.identity);
        Projectile projScript = projectile.GetComponent<Projectile>();
        projScript.SetTarget(targetEnemy, attackDamage);
    }

    private GameObject GetProjectilePrefab()
    {
        switch (towerElement)
        {
            case TowerElement.Fire:
                return Resources.Load<GameObject>("FireballProjectile");
            case TowerElement.Water:
                return Resources.Load<GameObject>("WaterProjectile");
            case TowerElement.Air:
                return Resources.Load<GameObject>("AirProjectile");
            case TowerElement.Earth:
                return Resources.Load<GameObject>("EarthProjectile");
            default:
                return arrowPrefab; // Default arrow projectile for normal tower
        }
    }

    // Upgrade the tower to a specific element
    public void UpgradeToElement(TowerElement newElement)
    {
        towerElement = newElement;
        switch (newElement)
        {
            case TowerElement.Fire:
                attackDamage = 20;
                attackRange = 7f;
                attackRate = 1.5f;
                Debug.Log("Tower upgraded to Fire!");
                break;
            case TowerElement.Water:
                attackDamage = 15;
                attackRange = 6f;
                attackRate = 1.2f;
                Debug.Log("Tower upgraded to Water!");
                break;
            case TowerElement.Air:
                attackDamage = 10;
                attackRange = 5f;
                attackRate = 0.7f; // Faster attack rate for Air
                Debug.Log("Tower upgraded to Air!");
                break;
            case TowerElement.Earth:
                attackDamage = 25;
                attackRange = 4f;
                attackRate = 1.8f;
                Debug.Log("Tower upgraded to Earth!");
                break;
        }

        // Attack range deðiþtiðinde Collider menzili de güncellenir
        SphereCollider rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.radius = attackRange;
    }

    // Düþman menzile girdiðinde tetiklenir
    private void OnTriggerEnter(Collider other)
    {
        if (targetEnemy == null && other.CompareTag("Enemy"))
        {
            targetEnemy = other.transform;
        }
    }

    // Düþman menzilden çýktýðýnda tetiklenir
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == targetEnemy)
        {
            targetEnemy = null;
        }
    }

    protected override void PerformBuildingAction()
    {
        // No resource gathering for towers, so this can be left empty
    }
}