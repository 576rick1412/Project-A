using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Proto : MonoBehaviour
{
    [Header("��ȣ")]
    [SerializeField] int Type_Num; // ī�� ��ȣ

    [SerializeField] SpriteRenderer Icon_Object; // �������� �� ���ӿ�����Ʈ
    [SerializeField] Sprite[] Icon; // ������

    // ���ӿ�����Ʈ�� �����Ǽ� UI �ؽ�Ʈ�� �ϱ� ��������� ������ ��������Ʈ�� ��ü
    [SerializeField] SpriteRenderer Text_Object; // �ؽ�Ʈ�� �� ���ӿ�����Ʈ
    [SerializeField] Sprite[] Text; // �ؽ�Ʈ
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
            case 0 : break; // ��ź
            case 1 : break; // ���
            case 2 : break; // ���� Ƚ�� ����
            case 3 : break; // ���� Ƚ�� ����
            case 4 : break; // ���� �ӵ� ����
            case 5 : break; // ���� �ӵ� ���� 
            case 6 : break; // HP 1 ȸ��
            case 7 : break; // Ư�� ������ , ī�� 1ȸ ��������
        }
        */
    }
}
