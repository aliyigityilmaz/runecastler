using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public enum Element
    {
        Fire,
        Water,
        Wind,
        Earth
    }

    public Element elementType;
    public int level; // 1, 2, 3 seviyeleri

    public int health;
    public int attackPower;
    public float attackRange;
    public float attackCooldown;
    public float movementSpeed;

    protected float attackTimer;
    protected NavMeshAgent agent;
    protected Animator animator;

    // Düþman saldýracaðý binayý bulur
    protected GameObject targetBuilding;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = movementSpeed;

        FindNearestBuilding(); // En yakýndaki binayý bulur.
    }

    private void Update()
    {
        if (targetBuilding != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetBuilding.transform.position);

            // Eðer hedefe ulaþtýysak saldýr
            if (distanceToTarget <= attackRange)
            {
                agent.isStopped = true;
                AttackBuilding();
            }
            else
            {
                // Hareket et
                agent.isStopped = false;
                agent.SetDestination(targetBuilding.transform.position);
            }
        }
    }

    void AttackBuilding()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    protected abstract void PerformAttack(); // Her elementin kendine özgü saldýrýsý olacak

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Ölüm animasyonu tetikle
        animator.SetTrigger("Die");

        // Yok etme mantýðý (animasyon sonrasý)
        Destroy(gameObject, 2f); // 2 saniye sonra objeyi yok et
    }

    void FindNearestBuilding()
    {
        // Binalarý bulma (Basit versiyonu: etraftaki binalarýn listesinden en yakýnýný bulma)
        Building[] allBuildings = FindObjectsOfType<Building>();
        float minDistance = Mathf.Infinity;

        foreach (Building building in allBuildings)
        {
            if (building.buildingName != "Cleanser") // Temizleyici hariç
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetBuilding = building.gameObject;
                }
            }
        }
    }
}