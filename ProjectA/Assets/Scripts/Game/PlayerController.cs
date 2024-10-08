using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ContactFilter2D filter;

    [SerializeField] private Collider2D bottomCollider;

    [SerializeField] float MoveSpeed = 100f;
    [SerializeField] float JumpSpeed = 20f;

    private Rigidbody2D rb;

    private Animator ar;

    private Dir currentDir = Dir.Right;
    private Dir beforeDir = Dir.Right;

    private PlayerState ps = PlayerState.Idle;
    private PlayerState pbs = PlayerState.Idle;

    private Vector3 scale = Vector3.one;

    private int swingCombo = 0;

    private bool swingCheck = false;
    private bool isStop = false;
    private bool isAttacking = false;
    private bool jumpUse = false;
    private bool jumping = false;
    private bool rolling = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ar = GetComponent<Animator>();
    }
    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        else if (Input.GetButtonDown("Swing"))
        {
            if (swingCheck == false) swingCheck = true;
        }
        else if (Input.GetButtonDown("Change"))
        {
            Change();
        }
        else if (Input.GetButtonDown("Roll"))
        {
            Roll();
        }
    }
    private void Jump()
    {
        // 현재 점프를 사용하지 않았다면
        if (jumpUse == false)
        {
            if (ps == PlayerState.Idle || ps == PlayerState.Run)
            {
                ps = PlayerState.Jump;

                jumpUse = true;
            }
        }
    }
    private void Swing()
    {
        if (!isAttacking)
        {
            if (swingCombo == 0 && ps != PlayerState.Jump && ps != PlayerState.Fall)
            {
                CancelInvoke();

                swingCombo++;

                isAttacking = true;

                isStop = true;

                ps = PlayerState.Swing1;
            }
            else if (swingCombo > 0)
            {
                CancelInvoke();

                swingCombo++;

                if (swingCombo == 4) swingCombo = 1;

                isAttacking = true;

                isStop = true;

                ps = (PlayerState)(swingCombo + 9);
            }
        }
        
    }
    private void Change()
    {

    }
    private void Roll()
    {

    }
    private void FixedUpdate()
    {
        float x = 0;

        if (ps == PlayerState.Fall)
        {
            if (bottomCollider.IsTouching(filter))
            {
                ps = PlayerState.Idle;

                jumping = false;
                jumpUse = false;
            }
        }

        if (jumpUse == true && jumping == false)
        {
            jumping = true;

            rb.linearVelocityY = 0;
            rb.AddForceY(JumpSpeed, ForceMode2D.Impulse);
        }

        // 만약 현재 상태가 점프일 때,
        // 플레이어가 떨어지고 있으면
        // 현재 상태를 추락중으로 변경한다.
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

                rb.linearVelocityX = x * MoveSpeed * Time.fixedDeltaTime;
            }
            else if (ps == PlayerState.Run) ps = PlayerState.Idle;
        }

        if (x == 0) rb.linearVelocityX = 0;

        // 공격 입력 부분
        if (swingCheck)
        {
            Swing();
            swingCheck = false;
        }

        // 현재 플레이어 상태가 갱신 되었을 때
        if (ps != pbs)
        {
            pbs = ps;
            Debug.Log("상태 갱신 : " + ps.ToString());

            switch (ps)
            {
                case PlayerState.Idle: ar.SetTrigger("Idle"); break;
                case PlayerState.Run: ar.SetTrigger("Run"); break;
                case PlayerState.Roll: ar.SetTrigger("Roll"); break;
                case PlayerState.Fall: ar.SetTrigger("Fall"); break;
                case PlayerState.Jump: ar.SetTrigger("Jump"); break;
                case PlayerState.Swing1: ar.SetTrigger("Swing1"); break;
                case PlayerState.Swing2: ar.SetTrigger("Swing2"); break;
                case PlayerState.Swing3: ar.SetTrigger("Swing3"); break;
                case PlayerState.JumpSwing1: ar.SetTrigger("JumpSwing1"); break;
                case PlayerState.JumpSwing2: ar.SetTrigger("JumpSwing2"); break;
                case PlayerState.JumpSwing3: ar.SetTrigger("JumpSwing3"); break;
            }
        }
    }
    void RollEnd()
    {
        ps = PlayerState.Idle;
    }
    void EnableCombo() // 콤보 공격 활성화 구간
    {
        Debug.Log(swingCombo);

        isAttacking = false;
        
        Invoke("DisableCombo", .7f);
    }
    void DisableCombo()
    {
        swingCombo = 0;
    }
    void EnableIdle() // 이동 및 회전 활성화 구간
    {
        ps = PlayerState.Idle;

        isStop = false;

        isAttacking = false;
    }
}
enum Dir
{
    Left = -1,
    Right = 1
}
enum PlayerState
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