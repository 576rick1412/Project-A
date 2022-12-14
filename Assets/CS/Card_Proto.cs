using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Proto : MonoBehaviour
{
    [Header("번호")]
    [SerializeField] int Type_Num; // 카드 번호

    [SerializeField] SpriteRenderer Icon_Object; // 아이콘이 들어갈 게임오브젝트
    [SerializeField] Sprite[] Icon; // 아이콘

    // 게임오브젝트로 생성되서 UI 텍스트로 하기 힘들어졌기 때문에 스프라이트로 대체
    [SerializeField] SpriteRenderer Text_Object; // 텍스트가 들어갈 게임오브젝트
    [SerializeField] Sprite[] Text; // 텍스트
    private void Awake()
    {

    }
    void Update()
    {
        Icon_Object.sprite = Icon[GameManager.GM.Card_Type_Num[Type_Num]];
        Text_Object.sprite = Text[GameManager.GM.Card_Type_Num[Type_Num]];

        /*
        switch (GameManager.GM.Card_Type_Num[Type_Num])
        {
            case 0 : break; // 폭탄
            case 1 : break; // 즉사
            case 2 : break; // 섞는 횟수 증가
            case 3 : break; // 섞는 횟수 감소
            case 4 : break; // 섞는 속도 증가
            case 5 : break; // 섞는 속도 감소 
            case 6 : break; // HP 1 회복
            case 7 : break; // 특수 아이템 , 카드 1회 돌려보기
        }
        */
    }
}
