using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Proto : MonoBehaviour
{
    [Header("��ȣ")]
    [SerializeField] int Type_Num; // ī�� ������ ��ȣ

    void Update()
    {
        
        switch(GameManager.GM.Card_Type_Num[Type_Num])
        {
            case 0 : break;
            case 1 : break;
            case 2 : break;
            case 3 : break;
            case 4 : break;
            case 5 : break;
            case 6 : break;
            case 7 : break;
        }
    }
}
