using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameControl : MonoBehaviour
{
    [SerializeField] int Player_HP;

    [SerializeField] GameObject Panel;
    bool OnPanel = true;   // 참 : 활성화 / 거짓 : 비활성화

    [SerializeField] TextMeshProUGUI Round_Text;


    public Vector3[] Card_Pos;
    [System.Serializable]
    struct Card_Info
    {
        public GameObject Card;
        public bool Joker;
    } // 카드 구조체

    [SerializeField] Card_Info[] card_Info;
    [SerializeField] Card_Info Temp_Info;

    [SerializeField] bool  NextRound = false;   // 다음 라운드로 이동 / 참 : 동작 , 거짓 : 중단
    [SerializeField] bool Combineing =  true;   // 카드 섞는 중

    [SerializeField] float RotationSpeed;       // 카드 회전 속도
    [SerializeField] int   Combine_num = 0;     // 카드 섞은 횟수

    void Start()
    {
        {
            // 게임메니저 CS
            GameManager.GM.Now_Level = 1;       
            GameManager.GM.RotationSpeed = GameManager.GM.Data.Set_RotationSpeed;
            GameManager.GM.Combine_Speed = GameManager.GM.Data.Set_Combine_Speed;
            GameManager.GM.Combine_times = GameManager.GM.Data.Set_Combine_times;

            // 게임 컨트롤 CS
            Player_HP = GameManager.GM.Data.Set_HP;
            RotationSpeed = GameManager.GM.RotationSpeed;
        } // 인게임 수치 초기화

        Init_Struct(true); // 카드 초기화
        StartCoroutine(Rotation_Card(false));
    }

    void Update()
    {
        GameLoop();
        Cast();
    }

    void GameLoop()
    {
        Panel.SetActive(OnPanel); // 화면 판넬

        if (!NextRound) return; // 다음 라운드가 거짓일 때 리턴

        int Combine_times = GameManager.GM.Combine_times;
        if (Combine_num < Combine_times && Combineing && NextRound)
        {
            int a = Random.Range(0, card_Info.Length);
            int b = Random.Range(0, card_Info.Length);
            while (a == b) b = Random.Range(0, card_Info.Length); // 중복 검사

            StartCoroutine(Combine_Card(a, b));
        }

        // 카드 선택 때까지 섞기 중단
        if (Combine_num >= Combine_times && Combineing) { NextRound = false; OnPanel = false; } // 화면 판넬 비활성화
        
    } // 카드 섞기 시작

    void Cast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(gameObject.CompareTag("Card"))
                {
                    Debug.Log("힛");
                }
            }
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 0.3f);
        }
    }

    void Init_Struct(bool Init)
    {
        // Init 가 참일 떄 배열 크기 늘어남
        if (Init) System.Array.Resize(ref Card_Pos, Card_Pos.Length + card_Info.Length);
        for (int i = 0; i < card_Info.Length; i++) Card_Pos[i] = card_Info[i].Card.transform.position;
    } // 카드 초기화 / 구조체 초기화 ( 카드가 섞일 때 재정렬 하기 위함 )

    IEnumerator Rotation_Card(bool Side)
    {
        yield return new WaitForSeconds(1f); // 시작 시 잠깐 대기

        // Side가 거짓일 때 앞면 -> 뒷면 / Side가 참일 때 뒷면 -> 앞면
        Vector3 Vec = Side == false ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);

        for (int i = 0; i < card_Info.Length;i++)
        {
            card_Info[i].Card.transform.DORotate(Vec, RotationSpeed).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.2f);
        }

        if (!Side) NextRound = true; // 다음 라운드로 돌아가도록 참으로 변경 
        yield return new WaitForSeconds(0.2f);
    } // 카드 회전
    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        Combineing = false; // if 문이 돌아가지 않도록 거짓으로 변경
        // 카드 이동
        float Combine_Speed = GameManager.GM.Combine_Speed;
        card_Info[Card_1].Card.transform.DOMove(Card_Pos[Card_2], Combine_Speed);
        card_Info[Card_2].Card.transform.DOMove(Card_Pos[Card_1], Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // 카드 위치 교환
        Temp_Info =      card_Info[Card_1];
        card_Info[Card_1] = card_Info[Card_2];
        card_Info[Card_2] = Temp_Info;

        Init_Struct(false);

        Combineing = true; // if 문이 돌아가도록 참으로 변경
        Combine_num++; // 섞은 횟수 1 추가
        
        yield return null;
    } // 카드 섞기 코루틴

    public void Call_Button(int Card_Num) { StartCoroutine(Card_Button(Card_Num)); }
    IEnumerator Card_Button(int Card_Num)
    {
        bool Round;
        // Side가 거짓일 때 앞면 -> 뒷면 / Side가 참일 때 뒷면 -> 앞면
        Vector3 Vec_front = new Vector3(0f, 180f, 0f);
        Vector3 Vec_back = new Vector3(0f, 0f, 0f);
        card_Info[Card_Num].Card.transform.DORotate(Vec_front, RotationSpeed * 2).SetEase(Ease.InOutQuad);

        Uptate_Data(); // 데이터 업데이트 함수
        yield return new WaitForSeconds(0.5f);
        card_Info[Card_Num].Card.transform.DORotate(Vec_back, RotationSpeed).SetEase(Ease.InOutQuad);
        Round_Text.text = "Round : " + GameManager.GM.Now_Level; // 점수 텍스트 업데이트
        // 카드를 맞췄을 때
        if (card_Info[Card_Num].Joker)
        {
            Round = true;
            Debug.Log("정답!!!!");
        }
        else
        {
            Round = false;
            Debug.Log("카드 틀림");
        }

        StartCoroutine(Rotation_Card(true));
        yield return new WaitForSeconds(1f);

        {
            // 카드를 틀렸으나 HP가 남아있을 때, HP를 차감하고 다음 라운드로 넘어감 / 아니면 게임 오버
            if (!Round && Player_HP > 1) { Player_HP--; Round = true; }
            else if(!Round && Player_HP <= 1) Debug.Log("게임 오버");

            // 다음 라운드로 넘어간다면, 다시 모든 카드 뒤집기
            if (Round) StartCoroutine(Rotation_Card(false));
        } // 게임 결과 판정

        yield return new WaitForSeconds(0.5f);


        GameManager.GM.SavaData();
        yield return null;
    }

    void Uptate_Data()
    {
        // 수치 초기화 및 다음 라운드 준비
        GameManager.GM.Now_Level++; // 라운드 횟수 추가

        // 최고점 달성 시 최고점 기록 업데이트
        if (GameManager.GM.Now_Level > GameManager.GM.Data.Max_Score) 
            GameManager.GM.Data.Max_Score = GameManager.GM.Now_Level;

        // 섞는 속도가 0.08초를 넘어가면, 카드 섞는 횟수를 증가시키는 걸로 대체 / 너무 빨라서 섞는게 안 보임
        if (GameManager.GM.Combine_Speed >= 0.12f) GameManager.GM.Combine_Speed *= 0.85f; // 섞는 속도 점점 증가
        else { GameManager.GM.Combine_Speed = 0.1f; GameManager.GM.Combine_times += 2; }  // 섞는 횟수 점점 증가

        Combine_num = 0; // 다시 섞일 수 있게 0으로 초기화
        OnPanel = true; // 화면 판넬 활성화
    }
}
