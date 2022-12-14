using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameControl : MonoBehaviour
{
    [SerializeField] int Player_HP;
    bool ZeroHP_UI = true;  // HP가 0일 때 UI를 실행하기 위함
    bool OnPanel = true;    // 판넬 ON / OFF 관리 /  참 : 활성화 / 거짓 : 비활성화

    [HideInInspector] public Vector3[] Card_Pos; // 카드 위치값

    // 카드가 섞으면서 카드 번호도 섞이기 때문에, 원본 카드 저장
    [HideInInspector] public GameObject[] Const_Card = new GameObject[5]; // 카드 원본 오브젝트
    [HideInInspector] public GameObject[] Card = new GameObject[5]; // 카드 오브젝트

    GameObject TempCard;        // 교환용 임시 카드

    bool NextRound = false;     // 다음 라운드로 이동 / 참 : 동작 , 거짓 : 중단
    bool Combineing = true;     // 카드 섞는 중

    float RotationSpeed;        // 카드 회전 속도
    int Combine_num = 0;        // 카드 섞은 횟수

    [Header("UI")]
    GameObject Panel;                                   // 화면 가림판
    [SerializeField] TextMeshProUGUI Round_Text;        // 현재 라운드 표시
    [SerializeField] TextMeshProUGUI Combine_Times;     // 남은 섞는 횟수 표시
    [SerializeField] TextMeshProUGUI Combine_Speed;     // 섞는 속도 표시
    [HideInInspector] public GameObject[] Life_Star = new GameObject[3];

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

            Panel = GameObject.Find("All_Border");
 
            {
                Life_Star[0] = GameObject.Find("Heart_0");
                Life_Star[1] = GameObject.Find("Heart_1");
                Life_Star[2] = GameObject.Find("Heart_2");
            } // HP 오브젝트 초기화
            {
                Const_Card[0] = Card[0] = GameObject.Find("Card_0");
                Const_Card[1] = Card[1] = GameObject.Find("Card_1");
                Const_Card[2] = Card[2] = GameObject.Find("Card_2");
                Const_Card[3] = Card[3] = GameObject.Find("Card_3");
                Const_Card[4] = Card[4] = GameObject.Find("Card_4");
            } // 카드 오브젝트 초기화

        } // 인게임 요소 초기화

        Init_Struct(true);  // 카드 초기화
        Item_Setting();     // 카드 내 아이템 초기화
        StartCoroutine(Rotation_Card(false));
    }

    void Update()
    {
        GameLoop();
        Cast();
        Update_UI();
    }
    void Init_Struct(bool Init)
    {
        // Init 가 참일 떄 배열 크기 늘어남
        if (Init) System.Array.Resize(ref Card_Pos, Card_Pos.Length + Card.Length);
        for (int i = 0; i < Card.Length; i++) Card_Pos[i] = Card[i].transform.position;
    } // 카드 초기화 / 구조체 초기화 ( 카드가 섞일 때 재정렬 하기 위함 )


    void GameLoop()
    {
        Panel.SetActive(OnPanel); // 화면 판넬

        if (!NextRound) return; // 다음 라운드가 거짓일 때 리턴

        int Combine_times = GameManager.GM.Combine_times;
        if (Combine_num < Combine_times && Combineing && NextRound)
        {
            int a = Random.Range(0, Card.Length);
            int b = Random.Range(0, Card.Length);
            while (a == b) b = Random.Range(0, Card.Length); // 중복 검사

            StartCoroutine(Combine_Card(a, b));
        }

        // 카드 선택 때까지 섞기 중단
        if (Combine_num >= Combine_times && Combineing) { NextRound = false; OnPanel = false; } // 화면 판넬 비활성화
        
    }   // 카드 섞기 시작
    void Cast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Ray.origin, Ray.direction * 10, Color.red, 0.2f);
            RaycastHit2D hit = Physics2D.Raycast(Ray.origin, Ray.direction, 1000);

            if (hit)
            {
                if(hit.collider.CompareTag("Card") && !OnPanel)
                {
                    GameObject ONJ = hit.collider.gameObject;
                    for (int i = 0; i < Const_Card.Length; i++)
                    {
                        if (ONJ == Const_Card[i])
                        { 
                            Debug.Log("카드번호 : " + i);
                            StartCoroutine(Card_Button(i));
                        }
                    }
                }
            }

        }
    }       // 레이캐스트
    void Update_UI()
    {
        if (Player_HP <= 0 && ZeroHP_UI) { ZeroHP_UI = false; return; }                 // 체력이 0일 경우, 실행 중단

        Round_Text.text = "Round : " + GameManager.GM.Now_Level;                        // 점수 텍스트 업데이트
        Combine_Times.text = "Times : " + Combine_num + " / " + GameManager.GM.Combine_times;   // 섞은 횟수 표시
        Combine_Speed.text = "Speed : " +  GameManager.GM.Combine_Speed;                        // 섞은 횟수 표시
        for (int i = 2; i > Player_HP - 1; i--) Life_Star[i].SetActive(false);          // HP 표시
    }  // UI 업데이트

    
    IEnumerator Rotation_Card(bool Side)
    {
        yield return new WaitForSeconds(1f); // 시작 시 잠깐 대기

        // Side가 거짓일 때 앞면 -> 뒷면 / Side가 참일 때 뒷면 -> 앞면
        Vector3 Vec = Side == false ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);

        for (int i = 0; i < Card.Length;i++)
        {
            Card[i].transform.DORotate(Vec, RotationSpeed).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.2f);
        }

        if (!Side) NextRound = true; // 다음 라운드로 돌아가도록 참으로 변경
        yield return new WaitForSeconds(0.2f);
    }               // 카드 회전
    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        Combineing = false; // if 문이 돌아가지 않도록 거짓으로 변경
        Combine_num++; // 섞은 횟수 1 추가

        // 카드 이동
        float Combine_Speed = GameManager.GM.Combine_Speed;
        Card[Card_1].transform.DOMove(Card_Pos[Card_2], Combine_Speed);
        Card[Card_2].transform.DOMove(Card_Pos[Card_1], Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // 카드 위치 교환
        TempCard     = Card[Card_1];
        Card[Card_1] = Card[Card_2];
        Card[Card_2] = TempCard;

        Init_Struct(false);

        Combineing = true; // if 문이 돌아가도록 참으로 변경
        yield return null;
    }   // 카드 섞기 코루틴
    IEnumerator Card_Button(int Card_Num)
    {
        // Side가 거짓일 때 앞면 -> 뒷면 / Side가 참일 때 뒷면 -> 앞면
        Vector3 Vec_front = new Vector3(0f, 180f, 0f);
        Vector3 Vec_back = new Vector3(0f, 0f, 0f);
        Const_Card[Card_Num].transform.DORotate(Vec_front, RotationSpeed * 2).SetEase(Ease.InOutQuad);

        Uptate_Data(); // 데이터 업데이트 함수
        yield return new WaitForSeconds(0.5f);
        Const_Card[Card_Num].transform.DORotate(Vec_back, RotationSpeed).SetEase(Ease.InOutQuad);

        bool Round =  Item_Check(Card_Num); // 아이템 판정 이후 , 라운드 넘어감 판정
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Rotation_Card(true));
        yield return new WaitForSeconds(1f);

        // 다음 라운드로 넘어간다면, 다시 모든 카드 뒤집기
        if (Round) StartCoroutine(Rotation_Card(false));

        yield return new WaitForSeconds(2f); // 모든 카드가 뒤집힌 후 카드가 돌아가도록

        GameManager.GM.Card_Type_Num[Card_Num] = Random.Range(0, 8);
        Range_Support(Card_Num); // 아이템 중복 검사

        GameManager.GM.SavaData();
        yield return null;
    }  // 카드 선택

    bool Item_Check(int Card_Num)
    {
        bool Round;
        int Card_Type = GameManager.GM.Card_Type_Num[Card_Num];

        switch (Card_Type)
        {
            case 0:                         // 폭탄
                Debug.Log("폭탄");
                break;
            case 1:                         // 즉사
                Player_HP = 0;
                Debug.Log("즉사");
                break;


            case 2:                         // 섞는 횟수 증가
                // 최대 횟수 20
                if (GameManager.GM.Combine_times < 19) GameManager.GM.Combine_times += 2;
                Debug.Log("섞는 횟수 증가");
                break;
            case 3:                         // 섞는 횟수 감소
                // 최소 횟수 6
                if(GameManager.GM.Combine_times > 6) GameManager.GM.Combine_times -= 1;
                Debug.Log("섞는 횟수 감소");
                break;


            case 4:                         // 섞는 속도 증가
                // 최소 속도 : 0.1  
                if (GameManager.GM.Combine_Speed > 0.1f) GameManager.GM.Combine_Speed -= 0.1f;
                Debug.Log("섞는 속도 증가");
                break;

            case 5:                         // 섞는 속도 감소 
                // 최대 속도: 0.6
                if (GameManager.GM.Combine_Speed < 0.6f) GameManager.GM.Combine_Speed += 0.1f;
                Debug.Log("섞는 속도 감소");
                break;


            case 6:                         // HP 1 회복
                if(Player_HP < 3) Player_HP++;
                for (int i = 0; i < Player_HP; i++) Life_Star[i].SetActive(true); // 따로 활성화하는 기능이 없어서....일단 전부 켜기
                Debug.Log("HP 1 회복");
                break;

            case 7:                         // 특수 아이템 , 카드 1회 돌려보기?
                Debug.Log("특수 카드");
                break;
        }

        // 게임 결과 판정
        if (Card_Type == 0 || Card_Type == 1) { Round = false; goto A; } // 아래 A 로 넘김
        Round = true; // Debug.Log("카드 넘김");

        // ====================================================================
        A: // 게임 결과 판정에서 여기로 이동됨
        // 카드를 틀렸으나 HP가 남아있을 때, HP를 차감하고 다음 라운드로 넘어감 / 아니면 게임 오버
        if (!Round && Player_HP > 1) {  Round = true; Player_HP--; }
        else if (!Round && Player_HP <= 1) { Debug.Log("게임 오버"); if(Player_HP > 0) Player_HP--; }

        return Round;
    }     // 카드 선택 시 아이템 효과 적용
    void Item_Setting()
    {
        for (int i = 0; i < 5; i++) GameManager.GM.Card_Type_Num[i] = Random.Range(0, 8); // 아이템에 난수 넣기
        for (int i = 0; i < 5; i++) Range_Support(i);   // 중복검사

        {
            /*
            // 아이템 세팅 , 폭탄( 0 )은 시작부터, 즉사( 1 )은 15라운드 부터
            GameManager.GM.Card_Type_Num[0] = Random.Range(0, 8);
            GameManager.GM.Card_Type_Num[1] = Random.Range(0, 8);

            while(GameManager.GM.Card_Type_Num[0] == GameManager.GM.Card_Type_Num[1])
                GameManager.GM.Card_Type_Num[1] = Random.Range(0, 8);

            GameManager.GM.Card_Type_Num[2] = Random.Range(0, 8);
            while (GameManager.GM.Card_Type_Num[0] == GameManager.GM.Card_Type_Num[2] 
                || GameManager.GM.Card_Type_Num[1] == GameManager.GM.Card_Type_Num[2])
                GameManager.GM.Card_Type_Num[2] = Random.Range(0, 8);

            GameManager.GM.Card_Type_Num[3] = Random.Range(0, 8);
            while (GameManager.GM.Card_Type_Num[0] == GameManager.GM.Card_Type_Num[3]
        || GameManager.GM.Card_Type_Num[1] == GameManager.GM.Card_Type_Num[3]
        || GameManager.GM.Card_Type_Num[2] == GameManager.GM.Card_Type_Num[3])
                GameManager.GM.Card_Type_Num[3] = Random.Range(0, 8);

            GameManager.GM.Card_Type_Num[4] = Random.Range(0, 8);
            while (GameManager.GM.Card_Type_Num[0] == GameManager.GM.Card_Type_Num[4]
        || GameManager.GM.Card_Type_Num[1] == GameManager.GM.Card_Type_Num[4]
        || GameManager.GM.Card_Type_Num[2] == GameManager.GM.Card_Type_Num[4]
        || GameManager.GM.Card_Type_Num[3] == GameManager.GM.Card_Type_Num[4])
                GameManager.GM.Card_Type_Num[4] = Random.Range(0, 8);
            */
        } // 초기형 노가다 중복검사기
    }               // 아이템 초기화
    void Range_Support(int Num)
    {
        for (int i = 0; i < 5; i++)
        {
            if (Num == i) return; // 무한 방지

            while (GameManager.GM.Card_Type_Num[i] == GameManager.GM.Card_Type_Num[Num])
                GameManager.GM.Card_Type_Num[Num] = Random.Range(0, 8);
        }
    }       // 아이템 중복 검사기


    void Uptate_Data()
    {
        // 수치 초기화 및 다음 라운드 준비
        GameManager.GM.Now_Level++; // 라운드 횟수 추가

        // 최고점 달성 시 최고점 기록 업데이트
        if (GameManager.GM.Now_Level > GameManager.GM.Data.Max_Score) 
            GameManager.GM.Data.Max_Score = GameManager.GM.Now_Level;

        // 섞는 속도가 0.08초를 넘어가면, 카드 섞는 횟수를 증가시키는 걸로 대체 / 너무 빨라서 섞는게 안 보임
        // if (GameManager.GM.Combine_Speed >= 0.12f) GameManager.GM.Combine_Speed *= 0.85f; // 섞는 속도 점점 증가
        //else { GameManager.GM.Combine_Speed = 0.1f; GameManager.GM.Combine_times += 2; }  // 섞는 횟수 점점 증가

        Combine_num = 0; // 다시 섞일 수 있게 0으로 초기화
        OnPanel = true; // 화면 판넬 활성화
    }
}
