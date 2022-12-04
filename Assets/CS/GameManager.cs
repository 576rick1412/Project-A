using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;
using AESWithJava.Con;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public MainDB Data;
    string FilePath;

    public int Interval_AutoSave;   // �ڵ� ���� ����
    public int Now_Level;           // ���� ����


    public float RotationSpeed;     // ī�� ȸ�� �ӵ�
    public float Combine_Speed;     // ī�� ���� �ӵ�
    public int Combine_times;       // īƮ ���� Ƚ��

    public int[] Card_Type_Num = new int[5];    // ī�� ������ ��ȣ

    void Awake() 
    { 
        GM = this;
        Application.targetFrameRate = 60;
        FilePath = Application.persistentDataPath + "/MainDB.txt"; Debug.Log(FilePath);

        LoadData();
        {
            RotationSpeed = Data.Set_RotationSpeed;
            Combine_Speed = Data.Set_Combine_Speed;
            Combine_times = Data.Set_Combine_times;
        } // �ΰ��� ��ġ �ʱ�ȭ
        SavaData();
        // ������ �ҷ��� �� �ʱ�ȭ�ϰ� �ٽ� ����
    }
    void Start()
    {
        StartCoroutine(AutoSave(Interval_AutoSave)); // �ڵ�����
    }

    void Update()
    {

    }

    IEnumerator AutoSave(int Wait_Time) { for (; ; ) { SavaData(); yield return new WaitForSeconds(Wait_Time); } }

    public void SavaData()
    {
        string key = Data.key;
        var save = JsonUtility.ToJson(Data);

        save = Program.Encrypt(save, key);
        File.WriteAllText(FilePath, save);
    }   // Json ����
    public void LoadData()
    {
        if (!File.Exists(FilePath)) { ResetMainDB(); return; }

        string key = Data.key;
        var load = File.ReadAllText(FilePath);

        load = Program.Decrypt(load, key);
        Data = JsonUtility.FromJson<MainDB>(load);
    }   // Json �ε�
    public void ResetMainDB()
    {
        Data = new MainDB
        {
            Max_Score =         0,
            Set_HP =            3,
            Set_RotationSpeed = 0.2f,
            Set_Combine_Speed = 0.5f,
            Set_Combine_times = 5
        };

        SavaData();
        LoadData();
    } // ���� ������ ���� �� ���� ���� �� �ʱ�ȭ

    [Serializable]
    public class MainDB
    {
        // AES ��ȣȭ Ű
        [HideInInspector] public string key = "6dgs8h4123fgdhdfg86763af8sg321fhd";

        public int Max_Score;           // �ִ� ����

        public int      Set_HP;             // �÷��̾� ü��
        public float    Set_RotationSpeed;  // ī�� ȸ�� �ӵ� ����
        public float    Set_Combine_Speed;  // ī�� ���� �ӵ� ����
        public int      Set_Combine_times;  // īƮ ���� Ƚ�� ����
    } // ���� ������
}

namespace AESWithJava.Con
{
    public class Program
    {
        public static string Decrypt(string textToDecrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 256;
            rijndaelCipher.BlockSize = 256;
            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[32];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) { len = keyBytes.Length; }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        public static string Encrypt(string textToEncrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 256;
            rijndaelCipher.BlockSize = 256;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[32];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) { len = keyBytes.Length; }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }
    }
} // AES ��ȣȭ