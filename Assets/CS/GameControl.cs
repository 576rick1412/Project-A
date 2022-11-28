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
        public Animator Card_anim;    // 카드 애니메이션
        public bool Card_Check;       // 참이면 앞면, 거짓이면 뒷면
        public int Card_Value;        // 카드 애니메이션 파라미터
    }

    [SerializeField] Card_Info[] card_Info;

    bool Combineing = true;  // 카드 섞는 중
    int  Combine_num = 0; // 카드 섞은 횟수

    void Start()
    {
        // 카드 초기화
        System.Array.Resize(ref card_Info, card_Info.Length + Card.Length);
        for (int i = 0; i < Card.Length; i++)
        {
            card_Info[i].Card_Pos = Card[i].transform.position;
            card_Info[i].Card_anim = Card[i].GetComponent<Animator>();
            card_Info[i].Card_Check = true;
            card_Info[i].Card_Value = 0;
        }
    }

    void Update()
    {
        // 카드 섞기 시작
        int Combine_times = GameManager.GM.Data.Combine_times;

        if(Combine_num < Combine_times && Combineing)
        {
            int a = Random.Range(0, Card.Length);
            int b = Random.Range(0, Card.Length);
            while (a == b) b = Random.Range(0, Card.Length); // 중복 검사

            StartCoroutine(Combine_Card(a, b));
        }
    }
    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        Combineing = false; // if 문이 돌아가지 않도록 거짓으로 변경

        GameObject Temp;

        // 카드 이동
        float Combine_Speed = GameManager.GM.Data.Combine_Speed;
        Card[Card_1].transform.DOMove(card_Info[Card_2].Card_Pos, Combine_Speed);
        Card[Card_2].transform.DOMove(card_Info[Card_1].Card_Pos, Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // 카드 위치 교환
        Temp = Card[Card_1];
        Card[Card_1] = Card[Card_2];
        Card[Card_2] = Temp;

        Struct_Reset();

        Combineing = true; // if 문이 돌아가도록 참으로 변경
        Combine_num++; // 섞은 횟수 1 추가
        yield return null;
    } // 카드 섞기 코루틴
    void Struct_Reset()
    {
        for (int i = 0; i < Card.Length; i++)
        {
            card_Info[i].Card_Pos = Card[i].transform.position;
            card_Info[i].Card_anim = Card[i].GetComponent<Animator>();
        }
    } // 구조체 초기화 / 카드가 섞일 때 재정렬 하기 위함

    public void Rotate_Card(int Value)
    {
        bool Card_Check = card_Info[Value].Card_Check;

        card_Info[Value].Card_Value = Card_Check == true ? 2 : 1;
        card_Info[Value].Card_Check = !Card_Check;

        card_Info[Value].Card_anim.SetInteger("OnCard", card_Info[Value].Card_Value);
    } // 카드 돌리기
}
