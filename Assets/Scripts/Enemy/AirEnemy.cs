using UnityEngine;

public class AirEnemy : Enemy
{
    [Header("Special Abilities")]
    public GameObject rangedAttackPrefab;  // Ranged attack for level 2 and 3
    public float rangedAttackSpeed = 20f;  // Speed of the ranged attack projectile

    public GameObject specialSkillEffect;  // Special skill effect for level 3
    public float specialSkillCooldown = 10f;
    private float specialSkillTimer;

    private void Start()
    {
        base.Start();  // Enemy base class'taki Start() metodunu �a��r�r

        // Seviye bazl� ba�lang�� ayarlar�
        switch (level)
        {
            case 1:
                movementSpeed = 7f;  // Level 1 daha h�zl�
                attackPower = 15;  // Yak�n sald�r�
                attackRange = 1.5f;
                attackCooldown = 1f;  // H�zl� sald�r� s�resi
                break;
            case 2:
                movementSpeed = 5f;
                attackPower = 10;  // Menzilli sald�r� g�c�
                attackRange = 10f;  // Menzilli sald�r� i�in daha b�y�k menzil
                attackCooldown = 1.5f;
                break;
            case 3:
                movementSpeed = 5f;
                attackPower = 10;
                attackRange = 10f;
                attackCooldown = 1.5f;
                specialSkillTimer = specialSkillCooldown;  // Special skill i�in zamanlay�c�
                break;
        }

        agent.speed = movementSpeed;
    }

    protected override void PerformAttack()
    {
        switch (level)
        {
            case 1:
                MeleeAttack();
                break;
            case 2:
                RangedAttack();
                break;
            case 3:
                RangedAttack();
                SpecialSkill();
                break;
        }
    }

    // Seviye 1 i�in yak�n sald�r�
    void MeleeAttack()
    {
        if (targetBuilding != null)
        {
            targetBuilding.GetComponent<Building>().TakeDamage(attackPower);
            Debug.Log("AirEnemy Level 1 performed melee attack.");
        }
    }

    // Seviye 2 ve 3 i�in menzilli sald�r�
    void RangedAttack()
    {
        if (targetBuilding != null)
        {
            GameObject projectile = Instantiate(rangedAttackPrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().velocity = (targetBuilding.transform.position - transform.position).normalized * rangedAttackSpeed;
            Debug.Log("AirEnemy Level 2/3 performed ranged attack.");
        }
    }

    // Seviye 3 �zel yetenek: Special Skill
    void SpecialSkill()
    {
        specialSkillTimer -= Time.deltaTime;
        if (specialSkillTimer <= 0f)
        {
            // �zel yetenek: AOE alan etkisi gibi d���nelim, �evresindeki d��manlara zarar verir
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 5f);  // 5 birimlik bir alan etki
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.gameObject.CompareTag("Enemy"))
                {
                    enemy.GetComponent<Enemy>().TakeDamage(attackPower * 2);  // �evredeki d��manlara ekstra hasar
                }
            }

            Instantiate(specialSkillEffect, transform.position, Quaternion.identity);  // �zel yetenek g�rsel efekt
            specialSkillTimer = specialSkillCooldown;  // Yeniden kullan�ma kadar bekleme s�resi
            Debug.Log("AirEnemy Level 3 used special skill.");
        }
    }
}