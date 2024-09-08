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

    // D��man sald�raca�� binay� bulur
    protected GameObject targetBuilding;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = movementSpeed;

        FindNearestBuilding(); // En yak�ndaki binay� bulur.
    }

    private void Update()
    {
        if (targetBuilding != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetBuilding.transform.position);

            // E�er hedefe ula�t�ysak sald�r
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

    protected abstract void PerformAttack(); // Her elementin kendine �zg� sald�r�s� olacak

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
        // �l�m animasyonu tetikle
        animator.SetTrigger("Die");

        // Yok etme mant��� (animasyon sonras�)
        Destroy(gameObject, 2f); // 2 saniye sonra objeyi yok et
    }

    void FindNearestBuilding()
    {
        // Binalar� bulma (Basit versiyonu: etraftaki binalar�n listesinden en yak�n�n� bulma)
        Building[] allBuildings = FindObjectsOfType<Building>();
        float minDistance = Mathf.Infinity;

        foreach (Building building in allBuildings)
        {
            if (building.buildingName != "Cleanser") // Temizleyici hari�
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