using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameControl : MonoBehaviour
{
    [SerializeField] GameObject Panel;
    bool OnPanel = true;   // �� : Ȱ��ȭ / ���� : ��Ȱ��ȭ

    [SerializeField] TextMeshProUGUI Round_Text;

    public Vector3[] Card_Pos;

    [System.Serializable]
    struct Card_Info
    {
        public GameObject Card;
        public bool Joker;
    } // ī�� ����ü

    [SerializeField] Card_Info[] card_Info;
    [SerializeField] Card_Info Temp_Info;

    [SerializeField] bool  NextRound = false;   // ���� ����� �̵� / �� : ���� , ���� : �ߴ�
    [SerializeField] bool Combineing =  true;   // ī�� ���� ��

    float RotationSpeed;                        // ī�� ȸ�� �ӵ�
    [SerializeField] int   Combine_num = 0;     // ī�� ���� Ƚ��

    void Start()
    {
        // ���� �� ��ġ �ʱ�ȭ
        RotationSpeed = GameManager.GM.Data.Combine_Speed;


        Init_Struct(true); // ī�� �ʱ�ȭ
        StartCoroutine(Rotation_Card(false));
    }

    void Update()
    {
        GameLoop();
    }

    void GameLoop()
    {
        Panel.SetActive(OnPanel); // ȭ�� �ǳ�

        if (!NextRound) return; // ���� ���尡 ������ �� ����

        int Combine_times = GameManager.GM.Data.Combine_times;
        if (Combine_num < Combine_times && Combineing && NextRound)
        {
            int a = Random.Range(0, card_Info.Length);
            int b = Random.Range(0, card_Info.Length);
            while (a == b) b = Random.Range(0, card_Info.Length); // �ߺ� �˻�

            StartCoroutine(Combine_Card(a, b));
        }

        // ī�� ���� ������ ���� �ߴ�
        if (Combine_num >= Combine_times && Combineing) { NextRound = false; OnPanel = false; } // ȭ�� �ǳ� ��Ȱ��ȭ
        
    } // ī�� ���� ����

    IEnumerator Rotation_Card(bool Side)
    {
        yield return new WaitForSeconds(1f); // ���� �� ��� ���

        // Side�� ������ �� �ո� -> �޸� / Side�� ���� �� �޸� -> �ո�
        Vector3 Vec = Side == false ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);

        for (int i = 0; i < card_Info.Length;i++)
        {
            card_Info[i].Card.transform.DORotate(Vec, RotationSpeed).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.2f);
        }

        if (!Side) NextRound = true; // ���� ����� ���ư����� ������ ���� 
        yield return new WaitForSeconds(0.2f);
    } // ī�� ȸ��

    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        Combineing = false; // if ���� ���ư��� �ʵ��� �������� ����
        // ī�� �̵�
        float Combine_Speed = GameManager.GM.Data.Combine_Speed;
        card_Info[Card_1].Card.transform.DOMove(Card_Pos[Card_2], Combine_Speed);
        card_Info[Card_2].Card.transform.DOMove(Card_Pos[Card_1], Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // ī�� ��ġ ��ȯ
        Temp_Info =      card_Info[Card_1];
        card_Info[Card_1] = card_Info[Card_2];
        card_Info[Card_2] = Temp_Info;

        Init_Struct(false);

        Combineing = true; // if ���� ���ư����� ������ ����
        Combine_num++; // ���� Ƚ�� 1 �߰�
        
        yield return null;
    } // ī�� ���� �ڷ�ƾ

    void Init_Struct(bool Init)
    {
        // Init �� ���� �� �迭 ũ�� �þ
        if (Init) System.Array.Resize(ref Card_Pos, Card_Pos.Length + card_Info.Length);
        for (int i = 0; i < card_Info.Length; i++) Card_Pos[i] = card_Info[i].Card.transform.position;
    } // ī�� �ʱ�ȭ / ����ü �ʱ�ȭ ( ī�尡 ���� �� ������ �ϱ� ���� )


    public void Call_Button(int Card_Num) { StartCoroutine(Card_Button(Card_Num)); }
    IEnumerator Card_Button(int Card_Num)
    {
        // Side�� ������ �� �ո� -> �޸� / Side�� ���� �� �޸� -> �ո�
        Vector3 Vec_front = new Vector3(0f, 180f, 0f);
        Vector3 Vec_back  = new Vector3(0f, 0f, 0f);

        card_Info[Card_Num].Card.transform.DORotate(Vec_front, RotationSpeed).SetEase(Ease.InOutQuad);

        // ī�带 ������ ��
        if (card_Info[Card_Num].Joker)
        {
            Uptate_Data(); // ������ ������Ʈ �Լ�
            yield return new WaitForSeconds(0.5f);
            card_Info[Card_Num].Card.transform.DORotate(Vec_back, RotationSpeed).SetEase(Ease.InOutQuad);
            Round_Text.text = "Round : " + GameManager.GM.Data.Now_Level; // ���� �ؽ�Ʈ ������Ʈ

            StartCoroutine(Rotation_Card(true));
            yield return new WaitForSeconds(1f);
            StartCoroutine(Rotation_Card(false));
            yield return new WaitForSeconds(0.5f);

        }

        else
        {
            Debug.Log("���� ����");
        }

        GameManager.GM.SavaData();
        yield return null;
    }

    void Uptate_Data()
    {
        // ��ġ �ʱ�ȭ �� ���� ���� �غ�
        GameManager.GM.Data.Now_Level++; // ���� Ƚ�� �߰�

        // �ְ��� �޼� �� �ְ��� ��� ������Ʈ
        if (GameManager.GM.Data.Now_Level > GameManager.GM.Data.Max_Score) 
            GameManager.GM.Data.Max_Score = GameManager.GM.Data.Now_Level;

        GameManager.GM.Data.Combine_Speed *= 0.9f; // ���� �ӵ� ���� ����
        GameManager.GM.Data.Combine_times += 2;    // ���� Ƚ�� ���� ����
        Combine_num = 0; // �ٽ� ���� �� �ְ� 0���� �ʱ�ȭ
        OnPanel = true; // ȭ�� �ǳ� Ȱ��ȭ
    }
}
