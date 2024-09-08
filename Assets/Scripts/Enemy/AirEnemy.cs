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
        base.Start();  // Enemy base class'taki Start() metodunu çaðýrýr

        // Seviye bazlý baþlangýç ayarlarý
        switch (level)
        {
            case 1:
                movementSpeed = 7f;  // Level 1 daha hýzlý
                attackPower = 15;  // Yakýn saldýrý
                attackRange = 1.5f;
                attackCooldown = 1f;  // Hýzlý saldýrý süresi
                break;
            case 2:
                movementSpeed = 5f;
                attackPower = 10;  // Menzilli saldýrý gücü
                attackRange = 10f;  // Menzilli saldýrý için daha büyük menzil
                attackCooldown = 1.5f;
                break;
            case 3:
                movementSpeed = 5f;
                attackPower = 10;
                attackRange = 10f;
                attackCooldown = 1.5f;
                specialSkillTimer = specialSkillCooldown;  // Special skill için zamanlayýcý
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

    // Seviye 1 için yakýn saldýrý
    void MeleeAttack()
    {
        if (targetBuilding != null)
        {
            targetBuilding.GetComponent<Building>().TakeDamage(attackPower);
            Debug.Log("AirEnemy Level 1 performed melee attack.");
        }
    }

    // Seviye 2 ve 3 için menzilli saldýrý
    void RangedAttack()
    {
        if (targetBuilding != null)
        {
            GameObject projectile = Instantiate(rangedAttackPrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().velocity = (targetBuilding.transform.position - transform.position).normalized * rangedAttackSpeed;
            Debug.Log("AirEnemy Level 2/3 performed ranged attack.");
        }
    }

    // Seviye 3 özel yetenek: Special Skill
    void SpecialSkill()
    {
        specialSkillTimer -= Time.deltaTime;
        if (specialSkillTimer <= 0f)
        {
            // Özel yetenek: AOE alan etkisi gibi düþünelim, çevresindeki düþmanlara zarar verir
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 5f);  // 5 birimlik bir alan etki
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.gameObject.CompareTag("Enemy"))
                {
                    enemy.GetComponent<Enemy>().TakeDamage(attackPower * 2);  // Çevredeki düþmanlara ekstra hasar
                }
            }

            Instantiate(specialSkillEffect, transform.position, Quaternion.identity);  // Özel yetenek görsel efekt
            specialSkillTimer = specialSkillCooldown;  // Yeniden kullanýma kadar bekleme süresi
            Debug.Log("AirEnemy Level 3 used special skill.");
        }
    }
}