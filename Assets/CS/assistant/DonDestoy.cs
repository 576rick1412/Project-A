using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonDestoy : MonoBehaviour
{
    void Awake()
    {
        var obj = FindObjectsOfType<DonDestoy>();
        if (obj.Length == 1) DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);
    }
}
