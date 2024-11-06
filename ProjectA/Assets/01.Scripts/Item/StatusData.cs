using UnityEngine;

[CreateAssetMenu(fileName = "StatusData", menuName = "Scriptable Objects/StatusData")]
public class StatusData : ScriptableObject
{
    public float playerDamage;           // �÷��̾� ������ %
    public float playerHp;              // �÷��̾� ü�� %
    public float playerStaminaRegen;     // �÷��̾� ���¹̳� ȸ�� �ӵ� %
    public float playerMoveSpeed;        // �÷��̾� �̵� �ӵ� %
    public float rollRange;             // �÷��̾� ������ �Ÿ� %

    public float playerCriticalChance;     // �÷��̾� ġ��Ÿ Ȯ�� +
    public float playerAvoidChance;        // �÷��̾� ȸ�� Ȯ�� +

    public float monsterDamage;          // ���� ������ %
    public float monsterHp;              // ���� ü�� %


    public float monsterCriticalChance;    // ���� ġ��Ÿ Ȯ�� +
    public float monsterAvoidChance;       // ���� ȸ�� Ȯ�� +

    public void Init()
    {
        playerDamage = 1.0f;
        playerHp = 1.0f;
        playerStaminaRegen = 1.0f;
        playerMoveSpeed = 1.0f;
        rollRange = 1.0f;

        playerCriticalChance = 0f;
        playerAvoidChance = 0f;

        monsterDamage = 1.0f;
        monsterHp = 1.0f;

        monsterCriticalChance = 0f;
        monsterAvoidChance = 0f;
    }
    public void Apply(StatusData other)
    {
        StatusData baseData = new StatusData();
        baseData.Init();

        playerDamage += other.playerDamage - baseData.playerDamage;
        playerHp += other.playerHp - baseData.playerHp;
        Debug.Log("OtherHP:" + other.playerHp.ToString() + "/BaseHP:" + baseData.playerHp.ToString());
        Debug.Log("PlayerHP:" + playerHp.ToString());
        playerStaminaRegen += other.playerStaminaRegen - baseData.playerStaminaRegen;
        playerMoveSpeed += other.playerMoveSpeed - baseData.playerMoveSpeed;
        playerAvoidChance += other.playerAvoidChance - baseData.playerAvoidChance;
        playerCriticalChance += other.playerCriticalChance - baseData.playerCriticalChance;

        rollRange += other.rollRange - baseData.rollRange;
        monsterAvoidChance += other.monsterAvoidChance - baseData.monsterAvoidChance;
        monsterDamage += other.monsterDamage - baseData.monsterDamage;
        monsterHp += other.monsterHp - baseData.monsterHp;
        monsterCriticalChance += other.monsterCriticalChance - baseData.monsterCriticalChance;
    }
}