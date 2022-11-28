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
        public Vector3 Card_Pos;      // ī�� ��ġ��
        public Animator Card_anim;    // ī�� �ִϸ��̼�
        public bool Card_Check;       // ���̸� �ո�, �����̸� �޸�
        public int Card_Value;        // ī�� �ִϸ��̼� �Ķ����
    }

    [SerializeField] Card_Info[] card_Info;

    bool Combineing = true;  // ī�� ���� ��
    int  Combine_num = 0; // ī�� ���� Ƚ��

    void Start()
    {
        // ī�� �ʱ�ȭ
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
        // ī�� ���� ����
        int Combine_times = GameManager.GM.Data.Combine_times;

        if(Combine_num < Combine_times && Combineing)
        {
            int a = Random.Range(0, Card.Length);
            int b = Random.Range(0, Card.Length);
            while (a == b) b = Random.Range(0, Card.Length); // �ߺ� �˻�

            StartCoroutine(Combine_Card(a, b));
        }
    }
    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        Combineing = false; // if ���� ���ư��� �ʵ��� �������� ����

        GameObject Temp;

        // ī�� �̵�
        float Combine_Speed = GameManager.GM.Data.Combine_Speed;
        Card[Card_1].transform.DOMove(card_Info[Card_2].Card_Pos, Combine_Speed);
        Card[Card_2].transform.DOMove(card_Info[Card_1].Card_Pos, Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // ī�� ��ġ ��ȯ
        Temp = Card[Card_1];
        Card[Card_1] = Card[Card_2];
        Card[Card_2] = Temp;

        Struct_Reset();

        Combineing = true; // if ���� ���ư����� ������ ����
        Combine_num++; // ���� Ƚ�� 1 �߰�
        yield return null;
    } // ī�� ���� �ڷ�ƾ
    void Struct_Reset()
    {
        for (int i = 0; i < Card.Length; i++)
        {
            card_Info[i].Card_Pos = Card[i].transform.position;
            card_Info[i].Card_anim = Card[i].GetComponent<Animator>();
        }
    } // ����ü �ʱ�ȭ / ī�尡 ���� �� ������ �ϱ� ����

    public void Rotate_Card(int Value)
    {
        bool Card_Check = card_Info[Value].Card_Check;

        card_Info[Value].Card_Value = Card_Check == true ? 2 : 1;
        card_Info[Value].Card_Check = !Card_Check;

        card_Info[Value].Card_anim.SetInteger("OnCard", card_Info[Value].Card_Value);
    } // ī�� ������
}
