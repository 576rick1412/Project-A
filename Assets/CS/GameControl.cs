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

    [SerializeField] float RotationSpeed;   // ī�� ȸ�� �ӵ�
    bool  NextRound = true;     // ���� ����� �̵� / �� : ���� , ���� : �ߴ�
    bool Combineing = true;     // ī�� ���� ��
    int  Combine_num = 0;       // ī�� ���� Ƚ��

    void Start()
    {
        Init_Struct(); // ī�� �ʱ�ȭ
        StartCoroutine(Rotation_Card(true));
    }

    void Update()
    {
        Combine();
    }

    void Init_Struct()
    {
        System.Array.Resize(ref card_Info, card_Info.Length + Card.Length);
        for (int i = 0; i < Card.Length; i++)
        {
            card_Info[i].Card_Pos = Card[i].transform.position;
            card_Info[i].Card_anim = Card[i].GetComponent<Animator>();
            card_Info[i].Card_Check = true;
            card_Info[i].Card_Value = 0;
        }
    } // ī�� �ʱ�ȭ

    void Combine()
    {
        if (!NextRound) return; // ���� ���尡 ������ �� ����

        int Combine_times = GameManager.GM.Data.Combine_times;
        if (Combine_num < Combine_times && Combineing)
        {
            int a = Random.Range(0, Card.Length);
            int b = Random.Range(0, Card.Length);
            while (a == b) b = Random.Range(0, Card.Length); // �ߺ� �˻�

            StartCoroutine(Combine_Card(a, b));
        }
    } // ī�� ���� ����

    IEnumerator Rotation_Card(bool Side)
    {
        // Side�� ���� �� �ո� -> �޸� / Side�� ������ �� �޸� -> �ո�
        Vector3 Vec = Side == true ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);

        for (int i = 0; i < Card.Length;i++)
        {
            Card[i].transform.DORotate(Vec, RotationSpeed).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.2f);
        }
        NextRound = true; // ���� ����� ���ư����� ������ ����
        yield return null;
    } // ī�� ȸ��

    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        Combineing = false; // if ���� ���ư��� �ʵ��� �������� ����

        // ī�� �̵�
        float Combine_Speed = GameManager.GM.Data.Combine_Speed;
        Card[Card_1].transform.DOMove(card_Info[Card_2].Card_Pos, Combine_Speed);
        Card[Card_2].transform.DOMove(card_Info[Card_1].Card_Pos, Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // ī�� ��ġ ��ȯ
        GameObject Temp = Card[Card_1];
        Card[Card_1]    = Card[Card_2];
        Card[Card_2]    = Temp;

        Reset_Struct();

        Combineing = true; // if ���� ���ư����� ������ ����
        NextRound = false; // ī�� ���� ������ ���� �ߴ�
        Combine_num++; // ���� Ƚ�� 1 �߰�
        yield return null;
    } // ī�� ���� �ڷ�ƾ

    void Reset_Struct()
    {
        for (int i = 0; i < Card.Length; i++)
        {
            card_Info[i].Card_Pos = Card[i].transform.position;
            card_Info[i].Card_anim = Card[i].GetComponent<Animator>();
        }
    } // ����ü �ʱ�ȭ / ī�尡 ���� �� ������ �ϱ� ����
}
