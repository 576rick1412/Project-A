using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Proto : MonoBehaviour
{
    Animator anim;
    bool Card;  // ÂüÀÌ¸é ¾Õ¸é, °ÅÁþÀÌ¸é µÞ¸é
    int CardIntegerValue;
    void Start()
    {
        anim = GetComponent<Animator>();
        CardIntegerValue = 0;
    }

    void Update()
    {

    }

    IEnumerator CCC()
    {
        for (; ; )
        {
            CardIntegerValue = Card == true ? 2 : 1; Card = !Card;
            anim.SetInteger("OnCard", CardIntegerValue);
            yield return new WaitForSeconds(1f);
        }
    }
}
