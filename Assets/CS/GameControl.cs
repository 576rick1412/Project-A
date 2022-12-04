using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameControl : MonoBehaviour
{
    [SerializeField] int Player_HP;
    bool OnPanel = true; // �ǳ� ON / OFF ���� /  �� : Ȱ��ȭ / ���� : ��Ȱ��ȭ

    [HideInInspector] public Vector3[] Card_Pos; // ī�� ��ġ��
    [HideInInspector] public GameObject[] Card = new GameObject[5]; // ī�� ������Ʈ
    GameObject TempCard;        // ��ȯ�� �ӽ� ī��

    bool NextRound = false;     // ���� ����� �̵� / �� : ���� , ���� : �ߴ�
    bool Combineing = true;     // ī�� ���� ��

    float RotationSpeed;        // ī�� ȸ�� �ӵ�
    int Combine_num = 0;        // ī�� ���� Ƚ��

    [Header("UI")]
    GameObject Panel;                                   // ȭ�� ������
    [SerializeField] TextMeshProUGUI Round_Text;        // ���� ���� ǥ��
    [SerializeField] TextMeshProUGUI Combine_Times;     // ���� ���� Ƚ�� ǥ��
    [HideInInspector] public GameObject[] Life_Star = new GameObject[3];

    void Start()
    {
        {
            // ���Ӹ޴��� CS
            GameManager.GM.Now_Level = 1;
            GameManager.GM.RotationSpeed = GameManager.GM.Data.Set_RotationSpeed;
            GameManager.GM.Combine_Speed = GameManager.GM.Data.Set_Combine_Speed;
            GameManager.GM.Combine_times = GameManager.GM.Data.Set_Combine_times;

            // ���� ��Ʈ�� CS
            Player_HP = GameManager.GM.Data.Set_HP;
            RotationSpeed = GameManager.GM.RotationSpeed;

            Panel = GameObject.Find("All_Border");

            {
                Life_Star[0] = GameObject.Find("Heart_0");
                Life_Star[1] = GameObject.Find("Heart_1");
                Life_Star[2] = GameObject.Find("Heart_2");
            } // HP ������Ʈ �ʱ�ȭ
            {
                Card[0] = GameObject.Find("Card_0");
                Card[1] = GameObject.Find("Card_1");
                Card[2] = GameObject.Find("Card_2");
                Card[3] = GameObject.Find("Card_3");
                Card[4] = GameObject.Find("Card_4");
            } // ī�� ������Ʈ �ʱ�ȭ

        } // �ΰ��� ��ġ �ʱ�ȭ

        Init_Struct(true); // ī�� �ʱ�ȭ
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
        // Init �� ���� �� �迭 ũ�� �þ
        if (Init) System.Array.Resize(ref Card_Pos, Card_Pos.Length + Card.Length);
        for (int i = 0; i < Card.Length; i++) Card_Pos[i] = Card[i].transform.position;
    } // ī�� �ʱ�ȭ / ����ü �ʱ�ȭ ( ī�尡 ���� �� ������ �ϱ� ���� )


    void GameLoop()
    {
        Panel.SetActive(OnPanel); // ȭ�� �ǳ�

        if (!NextRound) return; // ���� ���尡 ������ �� ����

        int Combine_times = GameManager.GM.Combine_times;
        if (Combine_num < Combine_times && Combineing && NextRound)
        {
            int a = Random.Range(0, Card.Length);
            int b = Random.Range(0, Card.Length);
            while (a == b) b = Random.Range(0, Card.Length); // �ߺ� �˻�

            StartCoroutine(Combine_Card(a, b));
        }

        // ī�� ���� ������ ���� �ߴ�
        if (Combine_num >= Combine_times && Combineing) { NextRound = false; OnPanel = false; } // ȭ�� �ǳ� ��Ȱ��ȭ
        
    }   // ī�� ���� ����
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


                    for (int i = 0; i < Card.Length; i++)
                    {
                        if (ONJ == Card[i]) 
                        { 
                            Debug.Log("ī���ȣ : " + i);
                            StartCoroutine(Card_Button(i));
                        }
                    }
                }
            }

        }
    }       // ����ĳ��Ʈ
    void Update_UI()
    {
        Round_Text.text = "Round : " + GameManager.GM.Now_Level;                        // ���� �ؽ�Ʈ ������Ʈ
        Combine_Times.text = "" + Combine_num + " / " + GameManager.GM.Combine_times;   // ���� Ƚ�� ǥ��
        for (int i = 2; i > Player_HP - 1; i--) Life_Star[i].SetActive(false);          // HP ǥ��
    }  // UI ������Ʈ

    
    IEnumerator Rotation_Card(bool Side)
    {
        yield return new WaitForSeconds(1f); // ���� �� ��� ���

        // Side�� ������ �� �ո� -> �޸� / Side�� ���� �� �޸� -> �ո�
        Vector3 Vec = Side == false ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);

        for (int i = 0; i < Card.Length;i++)
        {
            Card[i].transform.DORotate(Vec, RotationSpeed).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.2f);
        }

        if (!Side) NextRound = true; // ���� ����� ���ư����� ������ ���� 
        yield return new WaitForSeconds(0.2f);
    } // ī�� ȸ��
    IEnumerator Combine_Card(int Card_1,int Card_2)
    {
        Combineing = false; // if ���� ���ư��� �ʵ��� �������� ����
        // ī�� �̵�
        float Combine_Speed = GameManager.GM.Combine_Speed;
        Card[Card_1].transform.DOMove(Card_Pos[Card_2], Combine_Speed);
        Card[Card_2].transform.DOMove(Card_Pos[Card_1], Combine_Speed);
        yield return new WaitForSeconds(Combine_Speed * 2);

        // ī�� ��ġ ��ȯ
        TempCard     = Card[Card_1];
        Card[Card_1] = Card[Card_2];
        Card[Card_2] = TempCard;

        Init_Struct(false);

        Combineing = true; // if ���� ���ư����� ������ ����
        Combine_num++; // ���� Ƚ�� 1 �߰�
        
        yield return null;
    } // ī�� ���� �ڷ�ƾ
    IEnumerator Card_Button(int Card_Num)
    {
        // Side�� ������ �� �ո� -> �޸� / Side�� ���� �� �޸� -> �ո�
        Vector3 Vec_front = new Vector3(0f, 180f, 0f);
        Vector3 Vec_back = new Vector3(0f, 0f, 0f);
        Card[Card_Num].transform.DORotate(Vec_front, RotationSpeed * 2).SetEase(Ease.InOutQuad);

        Uptate_Data(); // ������ ������Ʈ �Լ�
        yield return new WaitForSeconds(0.5f);
        Card[Card_Num].transform.DORotate(Vec_back, RotationSpeed).SetEase(Ease.InOutQuad);

        StartCoroutine(Rotation_Card(true));
        yield return new WaitForSeconds(1f);

        Item_Check(Card_Num); // ������ ���� ���� , ���� �Ѿ ����
        yield return new WaitForSeconds(0.5f);

        GameManager.GM.SavaData();
        yield return null;
    }

    void Item_Check(int Card_Num)
    {
        bool Round;
        int Card_Type = GameManager.GM.Card_Type_Num[Card_Num];


        switch (Card_Type)
        {
            case 0:                         // ��ź
                break;
            case 1:                         // ���
                Player_HP = 0;
                break;


            case 2:                         // ���� Ƚ�� ����
                break;
            case 3:                         // ���� Ƚ�� ����
                break;


            case 4:                         // ���� �ӵ� ����
                break;
            case 5:                         // ���� �ӵ� ���� 
                break;


            case 6:                         // HP 1 ȸ��
                ; break;
            case 7:                         // Ư�� ������ , ī�� 1ȸ ������
                break;
        }

        // ���� ��� ����
        if (Card_Type != 0 || Card_Type != 1) { Round = true; Debug.Log("����!!!!"); }
        else { Round = false; Debug.Log("ī�� Ʋ��"); }


        // ī�带 Ʋ������ HP�� �������� ��, HP�� �����ϰ� ���� ����� �Ѿ / �ƴϸ� ���� ����
        if (!Round && Player_HP > 1) { Player_HP--; Round = true; }
        else if (!Round && Player_HP <= 1) Debug.Log("���� ����");

        // ���� ����� �Ѿ�ٸ�, �ٽ� ��� ī�� ������
        if (Round) StartCoroutine(Rotation_Card(false));

    }

    void Uptate_Data()
    {
        // ��ġ �ʱ�ȭ �� ���� ���� �غ�
        GameManager.GM.Now_Level++; // ���� Ƚ�� �߰�

        // �ְ��� �޼� �� �ְ��� ��� ������Ʈ
        if (GameManager.GM.Now_Level > GameManager.GM.Data.Max_Score) 
            GameManager.GM.Data.Max_Score = GameManager.GM.Now_Level;

        // ���� �ӵ��� 0.08�ʸ� �Ѿ��, ī�� ���� Ƚ���� ������Ű�� �ɷ� ��ü / �ʹ� ���� ���°� �� ����
        if (GameManager.GM.Combine_Speed >= 0.12f) GameManager.GM.Combine_Speed *= 0.85f; // ���� �ӵ� ���� ����
        else { GameManager.GM.Combine_Speed = 0.1f; GameManager.GM.Combine_times += 2; }  // ���� Ƚ�� ���� ����

        Combine_num = 0; // �ٽ� ���� �� �ְ� 0���� �ʱ�ȭ
        OnPanel = true; // ȭ�� �ǳ� Ȱ��ȭ
    }
}
