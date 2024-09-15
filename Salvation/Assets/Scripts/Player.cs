using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider hpBarSlider;
    public Slider flyTimeSlider;
    public Animator anim;
    public int jumpPower = 5;

    Rigidbody2D rigid;
    BoxCollider2D col;
    SpriteRenderer spriteRenderer;
    float flyTime = 5f;
    int jumpCnt = 0;
    float curHp;
    float maxHp;

    public static bool haveMemoryPiece;
    public static bool flyOver;
    public static float time;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 체력 설정
        SetHp(100);
        CheckHp();

        // 시간 활성화
        Time.timeScale = 1;

        flyTimeSlider.gameObject.SetActive(false);
        flyOver = false;
        haveMemoryPiece = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            Jump(); // 점프
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SlideBtnDown(); // 슬라이드 눌렀을 때
        }
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            SlideBtnUp(); // 슬라이드 뗐을 때
        }
    }

    // 점프
    public void Jump()
    {
        if (jumpCnt < 2) // 더블 점프까지 가능
        {
            SoundManager.instance.Play(Define.AudioClipType.Jump);

            if (jumpCnt == 0)
                anim.SetInteger("jumpCnt", 1);
            else
                anim.SetInteger("jumpCnt", 2);

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCnt++;
        }
        else
        {
            if (anim.GetBool("isFly") && transform.position.y < 1f)
            {
                SoundManager.instance.Play(Define.AudioClipType.Jump);
                anim.SetInteger("jumpCnt", 2);
                rigid.velocity = Vector2.zero;
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                jumpCnt++;
            }
        }
    }

    // 충돌 관리
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥 밟으면 점프 수 초기화
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("바닥에 닿았습니다.");
            jumpCnt = 0;
            anim.SetInteger("jumpCnt", 0);
        }
    }

    // 트리거 관리
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!anim.GetBool("isFly")) // 나비로 변신하지 않았을 때
        {
            // 장애물에 닿았을 때, 체력 닳기
            if (other.gameObject.CompareTag("Obstacle"))
            {
                Debug.Log(other.gameObject.name + "에 닿음!");
                Damage(30);
            }
        }

        if (other.gameObject.CompareTag("HealItem"))
        {
            ObjectPoolManager.instance.OnReturnToPool(other.gameObject);
            Heal(10);
        }

        if (other.gameObject.CompareTag("Light"))
        {
            ObjectPoolManager.instance.OnReturnToPool(other.gameObject);
            Fly();
        }

        if (other.gameObject.CompareTag("MemoryPiece"))
        {
            SoundManager.instance.Play(Define.AudioClipType.MemoryPiece);
            ObjectPoolManager.instance.OnReturnToPool(other.gameObject);
            haveMemoryPiece = true;
        }

        if (other.gameObject.CompareTag("ClearZone"))
        {
            Debug.Log("게임 클리어!");
            GameManager.instance.StageClear(); // 스테이지 클리어
        }
    }

    // 슬라이드 눌렀을 때
    public void SlideBtnDown()
    {
        SoundManager.instance.Play(Define.AudioClipType.Slide);
        anim.SetBool("isSlide", true);
        col.offset = new Vector2(-0.06f, -0.16f);
        col.size = new Vector2(1.15f, 0.8f);
    }

    // 슬라이드 뗐을 때
    public void SlideBtnUp()
    {
        anim.SetBool("isSlide", false);
        col.offset = new Vector2(-0.06f, 0.06f);
        col.size = new Vector2(0.8f, 1.2f);
    }

    // 나비로 변신해서 날기
    public void Fly()
    {
        SoundManager.instance.Play(Define.AudioClipType.Fly);
        anim.SetBool("isFly", true);
        rigid.gravityScale = 0.5f;
        flyTimeSlider.gameObject.SetActive(true);
        Debug.Log("나비로 변신!");
        StartCoroutine("ButterFlyChange");
    }

    // 나비 변신해서 나는 코루틴
    IEnumerator ButterFlyChange()
    {
        time = 0f;

        while (time < flyTime)
        {
            time += Time.deltaTime;
            flyTimeSlider.value = (flyTime - time) / flyTime;

            yield return null;
        }

        anim.SetBool("isFly", false);
        rigid.gravityScale = 1f;
        flyTimeSlider.gameObject.SetActive(false);
        flyOver = true;
    }

    // 체력 설정
    public void SetHp(float amount)
    {
        maxHp = amount;
        curHp = maxHp;
    }

    // 체력바 갱신
    public void CheckHp()
    {
        hpBarSlider.value = curHp / maxHp;
        // Debug.Log(hpBarSlider.value + " " + curHp / maxHp);
        // StartCoroutine(HpBarSliderUpdate(hpBarSlider.value, curHp / maxHp));
    }

    // 피해
    public void Damage(float damage)
    {
        SoundManager.instance.Play(Define.AudioClipType.Damage);

        if (maxHp == 0 || curHp <= 0)
            return;

        Debug.Log("데미지를 입었습니다.");
        curHp -= damage;
        CheckHp();
        StartCoroutine(DamageColorChange());

        // 현재 체력이 0 아래면 죽음
        if (curHp <= 0)
            Die();
    }

    IEnumerator DamageColorChange()
    {
        spriteRenderer.color = new Color32(252, 180, 255, 255);

        yield return new WaitForSeconds(0.5f);

        spriteRenderer.color = Color.white;
    }

    // 회복
    public void Heal(float heal)
    {
        SoundManager.instance.Play(Define.AudioClipType.Heal);

        if (curHp == maxHp)
            return;

        Debug.Log("체력이 회복되었습니다.");
        curHp += heal;
        if (curHp > maxHp)
            curHp = maxHp;
        CheckHp();
    }

    // 죽음
    public void Die()
    {
        Debug.Log("죽었습니다!");
        Time.timeScale = 0;
        UIManager.instance.LoadDieUI();
    }
}
