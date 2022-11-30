using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameControl : MonoBehaviour
{
    [SerializeField] GameObject[] Card;

    [System.Serializable]
    struct Card_Info
    {
        public Vector3 Card_Pos;      // 카드 위치값
        public bool Card_Check;       // 참이면 앞면, 거짓이면 뒷면
    } // 카드 구조체

    [SerializeField] Card_Info[] card_Info;

    [SerializeField] float RotationSpeed;   // 카드 회전 속도
    [SerializeField] bool  NextRound = true;     // 다음 라운드로 이동 / 참 : 동작 , 거짓 : 중단
    [SerializeField] bool Combineing = true;     // 카드 섞는 중
    [SerializeField]int  Combine_num = 0;       // 카드 섞은 횟수

    void Start()
    {
        Init_Struct(true); // 카드 초기화
        StartCoroutine(Rotation_Card(true));
    }

    void Update()
    {
        Combine();
    }
    void Combine()
    {
        if (!NextRound) return; // 다음 라운드가 거짓일 때 리턴

        int Combine_times = GameManager.GM.Data.Combine_times;
        if (Combine_num < Combine_times && Combineing)
        {
            int a = Random.Range(0, Card.Length);
            int b = Random.Range(0, Card.Length);
            while (a == b) b = Random.Range(0, Card.Length); // 중복 검사

            StartCoroutine(Combine_Card(a, b));
        }

        // 카드 선택 때까지 섞기 중단
        if (Combine_num >= Combine_times && Combineing) NextRound = false;
        
    } // 카드 섞기 시작

    IEnumerator Rotation_Card(bool Side)
    {
        // Side가 거짓일 때 앞면 -> 뒷면 / Side가 참일 때 뒷면 -> 앞면
        Vector3 Vec = Side == false ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);

        for (int i = 0; i < Card.Length;i++)
        {
            Card[i].transform.DORotate(Vec, RotationSpeed).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.2f);
        }
        NextRound = true; // 다음 라운드로 돌아가도록 참으로 변경
        yield return null;
    } // 카드 회전

    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        //yield return new WaitForSeconds(RotationSpeed * 4);

        Combineing = false; // if 문이 돌아가지 않도록 거짓으로 변경
        // 카드 이동
        float Combine_Speed = GameManager.GM.Data.Combine_Speed;
        Card[Card_1].transform.DOMove(card_Info[Card_2].Card_Pos, Combine_Speed);
        Card[Card_2].transform.DOMove(card_Info[Card_1].Card_Pos, Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // 카드 위치 교환
        GameObject Temp = Card[Card_1];
        Card[Card_1]    = Card[Card_2];
        Card[Card_2]    = Temp;

        Init_Struct(false);

        Combineing = true; // if 문이 돌아가도록 참으로 변경
        Combine_num++; // 섞은 횟수 1 추가
        yield return null;
    } // 카드 섞기 코루틴

    void Init_Struct(bool Init)
    {
        // Init 가 참일 떄 배열 크기 늘어남
        if (Init) System.Array.Resize(ref card_Info, card_Info.Length + Card.Length);
        for (int i = 0; i < Card.Length; i++) card_Info[i].Card_Pos = Card[i].transform.position;
    } // 카드 초기화 / 구조체 초기화 ( 카드가 섞일 때 재정렬 하기 위함 )
}
