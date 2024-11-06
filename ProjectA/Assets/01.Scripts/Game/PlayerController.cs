using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static bool isLifesteal = false;
    public static bool isMaximizer = false;

    public static PlayerController instance = null;

    public MiragePlayerController mpc = null;

    public FastDownEffect fastDownEffect = null;

    public TMP_Text hpText;
    public TMP_Text spText;

    [SerializeField] ContactFilter2D filter;
    [SerializeField] Image hpFillImage;
    [SerializeField] Image spFillImage;
    [SerializeField] Collider2D bottomCollider;
    [SerializeField] float[] moveSpeed;
    [SerializeField] float[] jumpSpeed;
    [SerializeField] float lastJumpSwing = -15f;
    [SerializeField] float rollRange = 5;
    [SerializeField] float rollSpeed = 2f;
    [SerializeField] float[] playerMaxHpBase;
    [SerializeField] float[] playerMaxSp;
    [SerializeField] float swingUseStamina = 15f;
    [SerializeField] float[] rollUseStamina;
    [SerializeField] float[] jumpUseStamina;
    [SerializeField] float[] staminaRegenSpeed;
    /*
     
    [SerializeField] float moveSpeed = 300f;
    [SerializeField] float jumpSpeed = 13f;
    [SerializeField] float lastJumpSwing = -15f;
    [SerializeField] float rollRange = 5;
    [SerializeField] float rollSpeed = 2f;
    [SerializeField] float playerMaxHp = 100f;
    [SerializeField] float playerMaxHps = 100f;
    [SerializeField] float playerMaxSp = 100f;
    [SerializeField] float swingUseStamina = 15f;
    [SerializeField] float rollUseStamina = 30f;
    [SerializeField] float jumpUseStamina = 0f;
    [SerializeField] float staminaRegenSpeed = 5f;
     
     */
    float playerMaxHp;

    [SerializeField] float fastDownAttackSpeed = -10f;

    Rigidbody2D rb;
    Animator ar;

    [HideInInspector] public Dir currentDir = Dir.Right;
    Dir beforeDir = Dir.Right;
    PlayerState ps = PlayerState.Idle;
    PlayerState pbs = PlayerState.Idle;
    Vector3 scale = Vector3.one;

    float playerHp = 10;
    float playerSp = 10;
    float beforeHpMaxRatio = 1f;

    int swingCombo = 0;
    int jumpSwingCombo = 0;

    bool invincibility = false; // true�� �� ����
    bool swingCheck = false;
    bool isStop = false;
    bool isAttacking = false;
    bool jumpUse = false;
    bool jumping = false;
    bool rolling = false;
    bool dieCheck = false;
    bool fastDownAttackCheck = false;
    bool fastDownAttacking = false;
    bool preventNextAction = false;
    public bool leaf = false; // ���������� ���� ���¹̳� �Ҹ� �ʿ���� ��
    public bool shield = false; // ���������� ���� ������ ���� �� ���� ��
    float PlayerSp {
        get { return playerSp; }
        set {
            playerSp = value;
            spFillImage.fillAmount =
                playerSp < 0 ?
                0 : playerSp / playerMaxSp[(int)Option.difficulty];
            spText.text = playerSp.ToString("F0") + " / " + playerMaxSp[(int)Option.difficulty].ToString();
        }
    }


    private float PlayerHp
    {
        get { return playerHp; }
        set
        {
            playerHp = Mathf.Clamp(value, 0, playerMaxHp);

            hpFillImage.fillAmount =
                playerHp < 0 ?
                0 : playerHp / playerMaxHp;
            hpText.text = playerHp.ToString("F0") + " / " + playerMaxHp.ToString();
        }
    }
    public bool IsAlive { get { return playerHp > 0; } }

    private void Awake()
    {
        instance = this;

        isLifesteal = false;
        isMaximizer = false;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ar = GetComponent<Animator>();

        playerMaxHp = playerMaxHpBase[(int)Option.difficulty];

        PlayerHp = playerMaxHp;

        playerSp = playerMaxSp[(int)Option.difficulty];
    }
    public void Update()
    {
        if (preventNextAction == true) return;

        if (Input.GetButtonDown("Jump"))
        {
            if (preventNextAction) return;
            Jump();
        }
        else if (Input.GetButtonDown("Swing"))
        {
            if (preventNextAction) return;
            if (swingCheck == false) swingCheck = true;
        }
        else if (Input.GetButtonDown("Roll"))
        {
            if (preventNextAction) return;
            Roll();
        }
        else if (Input.GetButtonDown("Down") || Input.GetKeyDown(KeyCode.S))
        {
            FastDownAttack();
        }
    }
    void FastDownAttack()
    {
        if ((ps == PlayerState.Jump || ps == PlayerState.Fall) && !fastDownAttacking)
        {
            if (fastDownAttackCheck == false)
            {
                StartCoroutine(FastDownAttackDelayCoroutine());
            }
            else
            {
                StopCoroutine(FastDownAttackDelayCoroutine());

                fastDownAttacking = true;

                StartCoroutine(FastDownAttackCoroutine());
            }
        }
    }
    IEnumerator FastDownAttackCoroutine()
    {
        // ���� ���� ������ ���� �⺻ ����

        rb.bodyType = RigidbodyType2D.Kinematic; // �߷� ���� X

        rb.linearVelocityY = fastDownAttackSpeed; // �ӵ� ��ȭ

        ps = PlayerState.Fall; // �������� ���·� ��ȯ

        while (transform.position.y > -3.2f) yield return null; // ���� ������ ������ �ݺ�

        SoundManager.Play("PlayerLandAttack", SoundType.Effect);

        fastDownEffect.Enable(transform.position);

        fastDownAttacking = false;

        ps = PlayerState.Idle; // ������ �ִ� ���·� ��ȯ

        rb.linearVelocityY = 0;

        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    IEnumerator FastDownAttackDelayCoroutine()
    {
        fastDownAttackCheck = true;

        yield return new WaitForSeconds(0.2f);

        fastDownAttackCheck = false;
    }
    public void StatusUpdate()
    {
        Debug.Log(beforeHpMaxRatio.ToString() + " / " + Inventory.CurrentData.playerHp);

        // �÷��̾��� �ִ� HP�� �÷��� ��
        if (beforeHpMaxRatio != Inventory.CurrentData.playerHp)
        {
            beforeHpMaxRatio = Inventory.CurrentData.playerHp;

            float beforeHpMax = playerMaxHp;

            playerMaxHp = playerMaxHpBase[(int)Option.difficulty] * Inventory.CurrentData.playerHp;

            float distance = playerMaxHp - beforeHpMax;

            if (distance > 0) PlayerHp += distance;
        }
    }
    private void Jump()
    {
        // ���� ������ ������� �ʾҴٸ�
        if (jumpUse == false)
        {
            if (preventNextAction) return;
            if ((ps == PlayerState.Idle || ps == PlayerState.Run) && UseStamina(jumpUseStamina[(int)Option.difficulty]))
            {
                preventNextAction = true;

                ps = PlayerState.Jump;

                jumpUse = true;
            }
        }
    }
    private void Swing()
    {
        if (fastDownAttacking == true) return;

        if (preventNextAction) return;

        if (ps == PlayerState.Jump || ps == PlayerState.Fall || ps == PlayerState.JumpSwing1 || ps == PlayerState.JumpSwing2 || ps == PlayerState.JumpSwing3)
        {
            if (!rolling)
            {
                if (!isAttacking)
                {
                    if (UseStamina(swingUseStamina))
                    {
                        preventNextAction = true;

                        CancelInvoke();

                        isAttacking = true;

                        isStop = true;

                        if (jumpSwingCombo++ > 0)
                            if (jumpSwingCombo == 4)
                                jumpSwingCombo = 1;

                        ps = (PlayerState)(jumpSwingCombo + 12);

                        rb.linearVelocityY = 0;

                        rb.bodyType = RigidbodyType2D.Kinematic;
                    }
                }
            }
        }
        else if (!rolling)
        {
            if (!isAttacking && UseStamina(swingUseStamina))
            {
                preventNextAction = true;

                CancelInvoke();

                isAttacking = true;

                isStop = true;

                if (swingCombo++ > 0)
                    if (swingCombo == 4)
                        swingCombo = 1;

                ps = (PlayerState)(swingCombo + 9);
            }
        }

    }
    #region �Ҹ� ��� �Լ�
    void FootStapSoundPlay()
    {
        SoundManager.FootStapPlay();
    }
    void JumpSoundPlay()
    {
        SoundManager.Play("PlayerJump", SoundType.Effect);
    }
    void SwingSoundPlay_1()
    {
        SoundManager.Play("PlayerAttack1", SoundType.Effect);
    }
    void SwingSoundPlay_2()
    {
        SoundManager.Play("PlayerAttack2", SoundType.Effect);
    }
    void SwingSoundPlay_3()
    {
        SoundManager.Play("PlayerAttack3", SoundType.Effect);
    }
    #endregion
    private void FixedUpdate()
    {
        if (IsAlive == false)
        {
            if (dieCheck == false)
            {
                dieCheck = true;
                rb.linearVelocityY = 0;
                rb.linearVelocityX = 0;
            }
            return;
        }
        PlayerSp += Time.fixedDeltaTime * staminaRegenSpeed[(int)Option.difficulty] * Inventory.CurrentData.playerStaminaRegen;
        if (PlayerSp > playerMaxSp[(int)Option.difficulty]) PlayerSp = playerMaxSp[(int)Option.difficulty];
        float x = 0;

        if (ps == PlayerState.Fall || ps == PlayerState.Idle || ps == PlayerState.Run)
        {
            if (bottomCollider.IsTouching(filter))
            {
                if (ps == PlayerState.Fall)
                    SoundManager.Play("PlayerLand", SoundType.Effect);

                ps = PlayerState.Idle;
                jumping = false;
                jumpUse = false;
            }
        }


        if (jumpUse == true && jumping == false)
        {
            jumping = true;

            rb.linearVelocityY = 0;
            rb.AddForceY(jumpSpeed[(int)Option.difficulty], ForceMode2D.Impulse);
        }

        // ���� ���� ���°� ������ ��,
        // �÷��̾ �������� ������
        // ���� ���¸� �߶������� �����Ѵ�.
        if (ps == PlayerState.Jump)
            if (rb.linearVelocityY < 0)
                ps = PlayerState.Fall;

        x = Input.GetAxisRaw("Horizontal");

        if (!isStop)
        {
            if (x == 1) currentDir = Dir.Right;

            if (x == -1) currentDir = Dir.Left;

            if (currentDir != beforeDir)
            {
                beforeDir = currentDir;
                scale.x = (int)currentDir;
                transform.localScale = scale;
            }
            if (x != 0)
            {

                if (ps == PlayerState.Idle) ps = PlayerState.Run;

                rb.linearVelocityX = x * moveSpeed[(int)Option.difficulty] * Time.fixedDeltaTime * Inventory.CurrentData.playerMoveSpeed;
            }
            else if (ps == PlayerState.Run) ps = PlayerState.Idle;
        }
        else rb.linearVelocityX = 0;

        if (x == 0) rb.linearVelocityX = 0;

        // ���� �Է� �κ�
        if (swingCheck)
        {
            Swing();
            swingCheck = false;
        }

        // ���� �÷��̾� ���°� ���� �Ǿ��� ��
        if (ps != pbs)
        {
            pbs = ps;

            preventNextAction = false;

            switch (ps)
            {
                case PlayerState.Idle: ar.SetTrigger("Idle"); break;
                case PlayerState.Run: ar.SetTrigger("Run"); break;
                case PlayerState.Roll: ar.SetTrigger("Roll"); break;
                case PlayerState.Fall: ar.SetTrigger("Fall"); break;
                case PlayerState.Jump: ar.SetTrigger("Jump"); break;
                case PlayerState.Swing1: ar.SetTrigger("Swing1"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.Swing2: ar.SetTrigger("Swing2"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.Swing3: ar.SetTrigger("Swing3"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.JumpSwing1: ar.SetTrigger("JumpSwing1"); if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.JumpSwing2: ar.SetTrigger("JumpSwing2");  if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
                case PlayerState.JumpSwing3: ar.SetTrigger("JumpSwing3");  if (mpc != null) { mpc.SetAnimation(ps, transform.position, currentDir); } break;
            }
        }
    }
    void RollEnd()
    {
        isStop = false;
        rolling = false;

        if (rb.linearVelocityY < 0) ps = PlayerState.Fall;

        else {
            ps = PlayerState.Idle;

            jumping = false;
            jumpUse = false;
        }
    }
    private void Roll()
    {
        if (fastDownAttacking == true) return;

        if (!rolling && (leaf || UseStamina(rollUseStamina[(int)Option.difficulty])))
        {

            SoundManager.Play("PlayerRoll", SoundType.Effect);

            leaf = false;
            isStop = true;
            rolling = true;
            isAttacking = false;

            ps = PlayerState.Roll;
            ar.SetTrigger("Roll");

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocityY = 0;

            StartCoroutine(RollCoroutine());
        }
    }
    IEnumerator RollCoroutine()
    {
        double t = 0;

        float startPos = transform.position.x;
        float endPos = transform.position.x + (rollRange * (int)currentDir * Inventory.CurrentData.rollRange);

        while (t < 1f)
        {
            t += Time.deltaTime * rollSpeed;
            transform.position = new Vector3(Mathf.Lerp(startPos, endPos, (float)t), transform.position.y, transform.position.z);
            yield return null;
        }
        transform.position = new Vector3(endPos, transform.position.y, transform.position.z);
    }
    void EnableCombo() // �޺� ���� Ȱ��ȭ ����
    {
        isAttacking = false;

        Invoke("DisableCombo", .7f);
    }
    void EnableJumpCombo() // �޺� ���� Ȱ��ȭ ����
    {
        isAttacking = false;

        Invoke("DisableJumpCombo", .7f);
    }
    void DisableCombo()
    {
        swingCombo = 0;
    }
    void DisableJumpCombo()
    {
        jumpSwingCombo = 0;
    }
    void EnableIdle() // �̵� �� ȸ�� Ȱ��ȭ ����
    {
        ps = PlayerState.Idle;

        isStop = false;

        isAttacking = false;
    }
    void EnableJumpIdle() // ���� ���� �߿� �̵� �� ȸ�� Ȱ��ȭ ����
    {
        ps = PlayerState.Fall;

        isStop = false;

        isAttacking = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    void LastJumpSwing()
    {
        rb.linearVelocityY = lastJumpSwing;
    }
    public void TouchGround()
    {
        ps = PlayerState.Idle;

        isStop = false;

        isAttacking = false;

        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.linearVelocityY = 0;

        jumpUse = false;

        jumping = false;
    }
    public void Hit(float damage)
    {
        if (IsAlive)
        {
            Debug.Log(Inventory.CurrentData.playerAvoidChance.ToString());

            if (damage < 0) // �̰��� ü�� ȸ��
            {
                PlayerHp -= damage;

                DamageTextController.SetDamage(-damage, transform.position + (Vector3.up * 0.5f), 2);
            }
            else if (shield) // ��� ������ ������ ��
            {
                DamageTextController.Shield(transform.position + Vector3.up * 0.5f);
            }
            else if (!invincibility)
            {
                if (Random.Range(0, 101) < Inventory.CurrentData.playerAvoidChance)
                {
                    DamageTextController.Avoid(transform.position + Vector3.up * 0.5f);

                    ar.SetTrigger("Hit");

                    invincibility = true;
                }
                else if (!invincibility && !rolling)
                {
                    invincibility = true;

                    damage *= Inventory.CurrentData.monsterDamage;

                    if (Random.Range(0, 101) < Inventory.CurrentData.monsterCriticalChance)
                        damage *= 2f;

                    PlayerHp -= damage;

                    DamageTextController.SetDamage(damage, transform.position + (Vector3.up * 0.5f), 1);

                    if (PlayerHp <= 0) Die();
                    else ar.SetTrigger("Hit");

                    SoundManager.Play("PlayerHit", SoundType.Effect);
                }
            }
        }

    }
    void Die()
    {
        ar.SetBool("Die", true);
    }
    void OnDeadPanel()
    {
        GameManager.Instance.GameOver();
    }
    void NotInvincibility()
    {
        invincibility = false;
    }
    bool UseStamina(float cost)
    {
        if (PlayerSp > cost)
        {
            PlayerSp -= cost;

            return true;
        }
        return false;
    }
}
public enum Dir
{
    Left = -1,
    Right = 1
}
public enum PlayerState
{
    Idle,
    Run,
    Roll,
    Jump,
    Fall,
    Swing1 = 10,
    Swing2,
    Swing3,
    JumpSwing1,
    JumpSwing2,
    JumpSwing3
}