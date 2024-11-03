using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RedHood : Monster
{
    public float moveTowardPlayerRatio = 1f;

    public float nextActionDelay = 1f;

    public Animator ar;

    public Transform throwKnifeFirePos;

    public Transform[] trackingArrowFirePos;

    public ParticleSystem[] particles;

    Transform Player
    {
        get { return PlayerController.instance.transform; }
    }

    Dir dir // �÷��̾��� ��ġ���� ������ �־ ������ �ٶ� ���� Left, �������� �Ĵٺ��� Right
    {
        get
        {
            return transform.position.x > Player.position.x ?
                Dir.Left : Dir.Right;
        }
    }

    int phase = 0; // ���� ������ �ܰ� Phase
    int pattern = 0; // �������� ������ ����
    int current = 0; // ���� ���� �׼� Index

    Vector3 beforePos = new Vector3(0, 3.255901f, 0); // ���� ��ġ ����
    Vector3 currentPosition = Vector3.zero; // ���� ��ġ

    Action[][][] patternActions; // ���Ϻ� �ൿ�� ���������� ���� Action ���� �迭

    private void Awake()
    {
        #region patternActions ������, ���� ũ�⿡ �°� �ʱ�ȭ

        patternActions = new Action[3][][];

        patternActions[0] = new Action[3][];
        patternActions[1] = new Action[3][];
        patternActions[2] = new Action[3][];

        patternActions[0][0] = new Action[5];
        patternActions[0][1] = new Action[5];
        patternActions[0][2] = new Action[5];

        patternActions[1][0] = new Action[4];
        patternActions[1][1] = new Action[4];
        patternActions[1][2] = new Action[4];

        patternActions[2][0] = new Action[5];
        patternActions[2][1] = new Action[5];
        patternActions[2][2] = new Action[5];

        #endregion

        #region �������� ���� ����

        #region 1 Phase Pattern Set
        patternActions[0][0][0] = FastKnifeAttack;
        patternActions[0][0][1] = ThrowPoisonApple;
        patternActions[0][0][2] = KnifeAttack;
        patternActions[0][0][3] = ThrowPoisonApple;
        patternActions[0][0][4] = StealthKnifeAttack;

        patternActions[0][1][0] = FastKnifeAttack;
        patternActions[0][1][1] = ThrowPoisonApple;
        patternActions[0][1][2] = KnifeAttack;
        patternActions[0][1][3] = ThrowPoisonApple;
        patternActions[0][1][4] = StealthKnifeAttack;

        patternActions[0][2][0] = FastKnifeAttack;
        patternActions[0][2][1] = ThrowPoisonApple;
        patternActions[0][2][2] = KnifeAttack;
        patternActions[0][2][3] = ThrowPoisonApple;
        patternActions[0][2][4] = StealthKnifeAttack;
        #endregion

        #region 2 Phase Pattern Set
        patternActions[1][0][0] = AxeAttack;
        patternActions[1][0][1] = StealthKnifeAttack;
        patternActions[1][0][2] = FastKnifeAttack;
        patternActions[1][0][3] = StealthBackMoveThrowKnife;

        patternActions[1][1][0] = FastKnifeAttack;
        patternActions[1][1][1] = AxeAttack;
        patternActions[1][1][2] = StealthKnifeAttack;
        patternActions[1][1][3] = StealthBackMoveThrowKnife;

        patternActions[1][2][0] = StealthKnifeAttack;
        patternActions[1][2][1] = AxeAttack;
        patternActions[1][2][2] = FastKnifeAttack;
        patternActions[1][2][3] = StealthBackMoveThrowKnife;
        #endregion

        #region 3 Phase Pattern Set
        patternActions[2][0][0] = AxeAttack;
        patternActions[2][0][1] = ChargeAxeAttack;
        patternActions[2][0][2] = ChargeArrow;
        patternActions[2][0][3] = ChargeAxeAttack;
        patternActions[2][0][4] = ChargeArrow;

        patternActions[2][1][0] = TripleTrackingArrow;
        patternActions[2][1][1] = ChargeAxeAttack;
        patternActions[2][1][2] = ChargeAxeAttack;
        patternActions[2][1][3] = ChargeArrow;
        patternActions[2][1][4] = ChargeAxeAttack;

        patternActions[2][2][0] = AxeAttack;
        patternActions[2][2][1] = ChargeAxeAttack;
        patternActions[2][2][2] = ChargeAxeAttack;
        patternActions[2][2][3] = ChargeArrow;
        patternActions[2][2][4] = TripleTrackingArrow;
        #endregion

        #endregion

        currentPosition = transform.position;

        Setting();
    }
    public override void StartMonster()
    {
        base.StartMonster();

        currentPosition = transform.position;

        patternActions[phase][pattern][current].Invoke();
    }
    #region ���� ���� ȭ��                                [ TripleTrackingArrow ]

    int trackingArrowFirePosIndex = 0; // trackingArrowFirePos[]�� �ּҷ� ����ϴ� Index ��
    void TripleTrackingArrow()
    {
        Debug.Log("���� ���� ȭ�� ����");

        ar.SetTrigger("TripleArrowShoot");

        trackingArrowFirePosIndex = 0;
    }
    void TrackingArrowShoot()
    {
        Debug.Log("���� ���� ȭ�� �߻�");

        TrackingArrow arrow = PoolingManager.Instance.GetObject<TrackingArrow>("TrackingArrow");

        arrow.StartMove(trackingArrowFirePos[trackingArrowFirePosIndex++].position);
    }
    #endregion
    #region Ȱ�� ������ �ܴ��� �̵� �� ���� ���⿡ ���   [ ChargeArrow ]
    void ChargeArrow()
    {
        Debug.Log("���� ����");
        ar.SetTrigger("ChargeArrow");
    }
    void ChargeArrowEffect()
    {
        Debug.Log("���� ����Ʈ");
        particles[3].Play(); // Ȱ�� �����ϴ� ����Ʈ ��
    }
    void CheckDash() // �̵��ؾ��� �ʿ䰡 ���� �� �̵��� �� ȭ���� ���
    {
        Debug.Log("�̵�Ȯ��");
        if (Mathf.Abs(Player.position.x - currentPosition.x) < 3)
        {
            particles[0].Play(); // ���� ���

            while (Mathf.Abs(Player.position.x - currentPosition.x) < 5f || currentPosition.x > 10f || currentPosition.x < -11f)
            {
                currentPosition.x = UnityEngine.Random.Range(-11f, 10f);
            }
            transform.position = currentPosition;

            particles[0].Play(); // ���� ���
        }
        LookAt(); // �÷��̾� �Ĵٺ���

        ChargeArrowShoot(); // ȭ�� �߻�
    }
    void ChargeArrowShoot()
    {
        Debug.Log("ȭ�� �߻�");
        Vector3 front = (dir == Dir.Left ? Vector3.left : Vector3.right);
        Vector3 top = Vector3.up;

        for (int t = 0; t < 6; t++)
        {
            RedHoodArrow arrow = PoolingManager.Instance.GetObject<RedHoodArrow>("RedHoodArrow");

            arrow.transform.position = throwKnifeFirePos.position;

            arrow.StartMove(12f, Vector3.Lerp(front, top, t != 0 ? t / 5f : 0));
        }
        ar.SetTrigger("ShootArrow");
    }
    #endregion
    #region ������ ���Ŀ� ����������� ������ ũ�� �ֵθ� [ ChargeAxeAttack ]
    void ChargeAxeAttack()
    {
        Debug.Log("���� ���� ����");
        ar.SetTrigger("AxeGreatAttackWaiting"); // �غ���

        particles[2].Play(); // ��õ ����Ʈ? ǥ��

        LookAt();
    }
    void ChargeAxe()
    {
        LookAt();

        Debug.Log("�ֵθ��� �� X:" + currentPosition.x.ToString());

        currentPosition.x += (dir == Dir.Left ? -8.74f : 8.74f);

        Debug.Log("�ֵθ��� �� X:" + currentPosition.x.ToString());

        transform.position = currentPosition;

        Debug.Log("��ġ Ȯ�� X:" + transform.position.x.ToString());

        ar.SetTrigger("AxeGreatAttack");
    }
    void DelayNextActionSetting(float delay)
    {
        StartCoroutine(DelayNextActionSettingCoroutine(delay));
    }
    IEnumerator DelayNextActionSettingCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        NextActionSetting(-1);
    }
    #endregion
    #region ���� �� �̵� �� �÷��̾�� �ܰ��� ����      [ StealthBackMoveThrowKnife ]
    void StealthBackMoveThrowKnife()
    {
        StartCoroutine(StealthBackMoveThrowKnifeCoroutine());
    }
    IEnumerator StealthBackMoveThrowKnifeCoroutine()
    {
        particles[0].Play(); // ���� ����Ʈ ���

        sr.enabled = false; // ���� ���� �����

        cd.enabled = false; // �浹 ��Ȱ��ȭ

        hpBarParent.SetActive(false); // hp�� �����

        currentPosition = currentPosition + (Vector3.right * (currentPosition.x > 0 ? -10 : 10));

        while (Mathf.Abs(currentPosition.x - Player.position.x) < 8f)
        {
            currentPosition.x = UnityEngine.Random.Range(-10f, 10f);
        }

        float t = 0;

        Vector3 start = transform.position;
        Vector3 move = start;

        while (t < 1f)
        {
            t += Time.deltaTime;

            move.x = Mathf.Lerp(start.x, currentPosition.x, t);

            transform.position = move;

            yield return null;
        }
        transform.position = currentPosition;

        sr.enabled = true; // ���� ���� ǥ��

        particles[0].Play(); // ���� ����Ʈ ���

        cd.enabled = true; // �浹 Ȱ��ȭ

        hpBarParent.SetActive(true); // hp�� ǥ��

        LookAt(); // �÷��̾� �Ĵٺ��� �ϱ�

        ar.SetTrigger("ThrowKnife"); // ���� ���
    }
    void ThrowKnife()
    {
        // �ܰ� ����
        RedHoodKnife knife = PoolingManager.Instance.GetObject<RedHoodKnife>("RedHoodKnife");

        // �ܰ� ��ġ ����
        knife.transform.position = throwKnifeFirePos.position;

        // �ܰ� �ӵ�, ���� ����
        knife.StartMove(12f, ((Player.position + Vector3.up * 0.5f) - knife.transform.position).normalized);
    }
    #endregion
    #region �÷��̾� �Ĵٺ� �� �ش� �������� �̵�         [ MoveTowardPlayer ]
    void MoveTowardPlayer()
    {
        LookAt(); // �÷��̾� �Ĵٺ���

        // ���̰� �� ���������� ��
        if (Mathf.Abs(currentPosition.x - Player.position.x) > 2f)
            particles[1].Play(); // �뽬 ����Ʈ ���

        ForwardMove(Mathf.Abs(transform.position.x - Player.position.x) + (dir == Dir.Left ? -1f : 1f)); // �÷��̾���� �Ÿ� ���� �����ͼ� �̵���Ŵ
    }
    #endregion
    #region �̵� �� ���� ����                             [ AxeAttack ]
    void AxeAttack()
    {
        Debug.Log("���� ����");
        ar.SetTrigger("AxeAttack");
    }
    #endregion
    #region �̵� �� ����                                  [ KnifeAttack ]
    void KnifeAttack()
    {
        Debug.Log("��� �̵� ����");
        // �÷��̾�� ����� �̵��Ͽ� �ܰ� 3Ÿ ����
        StartCoroutine(KnifeAttackCoroutine());
    }
    IEnumerator KnifeAttackCoroutine()
    {
        Debug.Log("KnifeAttackCoroutine");

        // �÷��̾���� ���� �Ÿ� Ȯ���� float
        float distance = Mathf.Abs(transform.position.x - Player.position.x);

        LookAt(); // �÷��̾� �Ĵٺ���

        // �ٴ� �ִϸ��̼����� ��ȯ
        if (distance > 1.2f) ar.SetTrigger("Run");

        while (distance > 1.2f) // ����� �������� ����
        {
            // �÷��̾��� ��ġ�� ���� x�� �̵��� �Ѵ�
            currentPosition.x += (dir == Dir.Right ? 1 : -1) * 5f * Time.deltaTime;

            // ��ġ�� �����Ѵ�
            transform.position = currentPosition;

            // ���� ���� ���Ѵ�
            distance = Mathf.Abs(transform.position.x - Player.position.x);

            yield return null;
        }

        // ���� �ִϸ��̼� ����
        ar.SetTrigger("KnifeAttack");
    }
    #endregion
    #region ���� �̵� �� ����                             [ FastKnifeAttack ]
    void FastKnifeAttack()
    {
        Debug.Log("���� �̵� ����");
        // �÷��̾�� ���� �ӵ��� �̵��Ͽ� �ܰ� 3Ÿ ����
        StartCoroutine(FastKnifeAttackCoroutine());
    }
    IEnumerator FastKnifeAttackCoroutine()
    {
        Debug.Log("FastKnifeAttackCoroutine");

        LookAt(); // �÷��̾� �Ĵٺ���

        // ���� �غ� �ڼ� ���ϱ�
        ar.SetTrigger("FastKnifeAttackWaiting");

        float t = 1;
        Color color = Color.white;
        while (t > 0.2f)
        {
            yield return null;

            t -= Time.deltaTime * 2f;

            color.a = t;

            sr.color = color;
        }

        beforePos = currentPosition;

        bool isLeft = UnityEngine.Random.value > 0.5f;

        currentPosition.x = Player.position.x + (isLeft ? 1.2f : -1.2f);
        currentPosition.y = Player.position.y;

        transform.position = currentPosition;

        LookAt();

        t = 0.5f;

        color.a = 0.5f;

        sr.color = color;

        while (t < 1f)
        {
            yield return null;

            t += Time.deltaTime * 2f;

            color.a = t;

            sr.color = color;
        }

        // ���� �ִϸ��̼� ����
        ar.SetTrigger("FastKnifeAttack");
    }
    void LastFastKnifeAttack()
    {
        StartCoroutine(LastFastKnifeAttackCoroutine());
    }
    IEnumerator LastFastKnifeAttackCoroutine()
    {
        Color color = Color.white;

        float t = 1f;

        while (t > 0.2f)
        {
            yield return null;

            t -= Time.deltaTime * 4f;

            color.a = t;

            sr.color = color;
        }

        currentPosition = beforePos;

        transform.position = currentPosition;

        t = 0.2f;

        LookAt();

        while (t < 1f)
        {
            yield return null;

            t += Time.deltaTime * 4f;

            color.a = t;

            sr.color = color;
        }

        NextActionSetting();
    }
    #endregion
    #region ���� �� ġ��Ÿ ���ϱ�                         [ StealthKnifeAttack ]
    void StealthKnifeAttack()
    {
        Debug.Log("���� ġ��Ÿ");
        StartCoroutine(StealthKnifeAttackCoroutine());
    }
    IEnumerator StealthKnifeAttackCoroutine()
    {
        // ���� �غ� �ڼ� ���ϱ�
        ar.SetTrigger("StealthAttackWaiting");

        yield return new WaitForSeconds(0.5f); // �ڼ� ���� �� 0.5�� ������

        particles[0].Play(); // ���� ����Ʈ ���

        sr.enabled = false; // ���� ���� �����

        cd.enabled = false; // �浹 ��Ȱ��ȭ

        hpBarParent.SetActive(false); // hp�� �����

        yield return new WaitForSeconds(1f); // ���� ���, ǥ�� ������ 1�� ��

        float t = 0;

        bool isLeft = UnityEngine.Random.value > 0.5f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(Player.position.x + (isLeft ? -1.2f : 1.2f), Player.position.y, 1);
        Vector3 middlePos = new Vector3(startPos.x + targetPos.x / 2f, isLeft ? 3f : -3f, 1);

        while (t < 1f)
        {
            yield return null;
            t += Time.deltaTime;

            middlePos.Set(startPos.x + targetPos.x / 2f, isLeft ? 3f : -3f, 1);
            targetPos.Set(Player.position.x + (isLeft ? -1.2f : 1.2f), Player.position.y, 1);
            currentPosition = Vector3.Lerp(Vector3.Lerp(startPos, middlePos, t), Vector3.Lerp(middlePos, targetPos, t), t);
            transform.position = currentPosition;
        }

        sr.enabled = true; // ���� ���� ǥ��

        particles[0].Play(); // ���� ����Ʈ ���

        cd.enabled = true; // �浹 Ȱ��ȭ

        hpBarParent.SetActive(true); // hp�� ǥ��

        LookAt(); // �÷��̾� �Ĵٺ��� �ϱ�

        // ���� �ִϸ��̼� ����
        ar.SetTrigger("StealthAttack");
    }
    #endregion
    #region ����� �����鼭 �̵�                          [ ThrowPoisonApple ]
    void ThrowPoisonApple()
    {
        Debug.Log("��� ����");
        StartCoroutine(ThrowPoisonAppleCoroutine());
    }
    IEnumerator ThrowPoisonAppleCoroutine()
    {
        Debug.Log("ThrowPoisonAppleCoroutine");

        yield return null;

        ar.SetTrigger("Run"); // �޸��� �ִϸ��̼�

        float goal = (10f * (transform.position.x <= 0 ? 1 : -1));

        float baseDelay = 0.3f;
        float delay = 0.5f;

        float current = 0;

        bool isLeft = goal < 0;
        bool dirCheck = !isLeft;

        LookAt(0);

        while (true)
        {
            currentPosition.x += Time.deltaTime * 5f * (isLeft ? -1 : 1);

            transform.position = currentPosition;

            goal += (isLeft ? 1 : -1) * Time.deltaTime * 5f;

            isLeft = goal < 0;

            current += Time.deltaTime;

            if (current > delay)
            {
                delay += baseDelay;

                GameObject poisonApple = PoolingManager.Instance.GetObject("PoisonApple");

                poisonApple.transform.position = transform.position + Vector3.up * 0.5f;
            }

            if (dirCheck == isLeft) break;

            yield return null;
        }

        NextActionSetting();
    }
    #endregion
    #region �������� ��������ŭ �̵�                      [ ForwardMove ]
    void ForwardMove(float moveValue)
    {
        Debug.Log("�̵��� �߻�");
        StartCoroutine(ForwardMoveCoroutine(moveValue));
    }
    IEnumerator ForwardMoveCoroutine(float moveValue)
    {
        float st = currentPosition.x;

        float ed = currentPosition.x + moveValue * (transform.localScale.x > 0 ? -1 : 1);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 5f;

            currentPosition.x = Mathf.Lerp(st, ed, t);

            transform.position = currentPosition;

            yield return null;
        }
    }
    #endregion
    #region �÷��̾� Ȥ�� ���𰡸� �ٶ󺸱�               [ LookAt ]
    void LookAt(float pos = float.MaxValue)
    {
        if (pos != float.MaxValue)
            transform.localScale = new Vector3(transform.position.x > pos ? 1 : -1, 1, 1);
        else
            transform.localScale = new Vector3(dir == Dir.Left ? 1 : -1, 1, 1);
    }
    #endregion
    #region ���� �ൿ ����                                [ NextActionSetting ]
    void NextActionSetting(float delay = 0)
    {
        ar.SetTrigger("Idle");

        current++; // ���� ���Ͽ��� ���� �׼����� �ѱ��

        // ���� ���� ������ ������ ���
        if (current >= patternActions[phase][pattern].Length)
        {
            // �������� ���� �ϳ��� ���� �Ѵ�
            pattern = UnityEngine.Random.Range(0, patternActions[phase].Length);

            // ù �κ� �׼����� �ʱ�ȭ �Ѵ�
            current = 0;
        }

        if (currentPosition.y > -3.255901f)
        {
            StartCoroutine(FallCoroutine());
        }
        else
        {
            StartCoroutine(NextActionSettingCoroutine(delay));
        }
    }
    IEnumerator FallCoroutine()
    {
        ar.SetTrigger("Fall");

        while (currentPosition.y > -3.255901f)
        {
            currentPosition.y -= Time.deltaTime * 4f;

            transform.position = currentPosition;

            yield return null;
        }
        currentPosition.y = -3.255901f;

        transform.position = currentPosition;

        ar.SetTrigger("Land");

        DelayNextActionSetting(1f);
    }
    IEnumerator NextActionSettingCoroutine(float delay)
    {
        if (!IsAlive) // ü���� 0 ���ϰ� ���� ���
        {
            // Phase�� 2�� ��쿡�� ������ �������̹Ƿ� Die 
            if (phase == 2) DieSetting();

            // Phase�� 0,1�� ��쿡�� ���� ����� �����Ƿ� NextPhase
            else NextPhaseSetting();
        }
        else
        {
            // delay�� 0���� ũ�ٸ� �⺻ �����̸� �����ϰ� delay ���� ��ٸ���
            if (delay > 0) yield return new WaitForSeconds(delay);

            // �⺻���� 0�̶�� �⺻ ActionDelay�� �����Ѵ�
            else if (delay == 0) yield return new WaitForSeconds(nextActionDelay);

            else yield return null; // ���� ������ ���� 1������ �ѱ��

            // ���� delay�� �������� �޾ƿԴٸ� ��� ����ȴ�
            patternActions[phase][pattern][current].Invoke();
        }
    }
    #endregion
    public override void Hit(float damage, bool isMainAttack = true)
    {
        base.Hit(damage, isMainAttack);
    }
    void NextPhaseSetting()
    {
        phase++; // ����� �Ѵܰ� ���δ�

        cd.enabled = false; // �浹 ��Ȱ��ȭ

        Debug.Log("���� Phase : " + phase.ToString());

        // �������� ���� �ϳ��� ���� �Ѵ�
        pattern = UnityEngine.Random.Range(0, patternActions[phase].Length);

        current = 0; // ������ ������ 0���� �ʱ�ȭ

        if (phase == 1) // �߰� �������� ���
        {
            ar.SetTrigger("Phase_1");

            particles[4].Play(); // Phase_1 Effect ���
        }
        else // ������ �������� ���
        {
            ar.SetTrigger("Phase_2");

            particles[5].Play(); // Phase_2 Effect ���
            particles[6].Play(); 
            particles[7].Play(); 
        }

        StartCoroutine(FillHpCoroutine());
    }
    IEnumerator FillHpCoroutine()
    {
        maxHp = phase == 1 ? 50000 : 70000;
        maxHp *= Inventory.CurrentData.monsterHp;

        float t = 0;

        while(t < 1f)
        {
            t += Time.deltaTime * 0.5f;

            Hp = Mathf.Lerp(0, maxHp, t);

            yield return null;
        }
        Hp = maxHp;

        cd.enabled = true; // �浹 Ȱ��ȭ
    }
    void NextPhaseActionSelect()
    {
        // Phase 1�� ���� ȯ�� �λ�� ���� ������ �����Ѵ�
        if (phase == 1) AxeAttack();

        // Phase 2�� ���� ȯ�� �λ�� ���� �������� �����Ѵ�
        else ChargeAxeAttack();
    }
    void DieSetting()
    {
        ar.SetTrigger("Die");

        particles[8].Play();
        particles[9].Play();
    }
    void ClearGame()
    {
        GameManager.Instance.ClearGame();
    }
}