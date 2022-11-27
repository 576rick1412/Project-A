using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameControl : MonoBehaviour
{
    [SerializeField] GameObject[] Card;
    [SerializeField] Vector3[] Card_Pos;

    void Start()
    {
        // ī�� ��ġ �ʱ�ȭ
        for (int i = 0; i < Card.Length; i++)
        {
            System.Array.Resize(ref Card_Pos, Card_Pos.Length + 1);
            Card_Pos[i] = Card[i].transform.position;
        }

        Combine_Card(1,2);
    }

    void Combine_Card(int Card_1,int Card_2)
    {
        GameObject Temp;
        
        // ī�� �̵�
        Card[Card_1].transform.DOMove(Card_Pos[Card_2], GameManager.GM.Data.Combine_Speed);
        Card[Card_2].transform.DOMove(Card_Pos[Card_1], GameManager.GM.Data.Combine_Speed);

        // ī�� ��ġ ��ȯ
        Temp = Card[Card_1];
        Card[Card_1] = Card[Card_2];
        Card[Card_2] = Temp;
    }

    void Update()
    {
        
    }
}
