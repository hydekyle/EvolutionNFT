using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Firebase.Database;
using Enums;

public class Cofres_System : MonoBehaviour {

    public static Cofres_System Instance;
    public Sprite s_chest1, s_chest2, s_chestVIP1, s_chestVIP2, s_chestVIP3;
    public Sprite s_chest1_opened, s_chest2_opened, s_chestVIP1_opened, s_chestVIP2_opened, s_chestVIP3_opened;
    public Transform chest, buy_chest_VIP, buy_chest, resplandor;
    public Text text_chest, text_chest_VIP;
    Button chest_button;
    public Transform chest_slots;
    public Chest selected_chest_type;

    void Awake()
    {
        Instance = this;
        chest_button = chest.Find("Chest").GetComponent<Button>();
    }

    void OnEnable()
    {
        OnBack();
    }

    public void OnBack()
    {
        selected_chest_type = Chest.None;
        CanvasBase.Instance.UpdateGoldView();
        GetComponent<Canvas>().targetDisplay = 0;
        MostrarIconoCofres();
        if (GameManager.Instance.userdb.cofres_VIP.Count > 0) PutChest(GameManager.Instance.userdb.cofres_VIP[0]); //Si tienes cofres VIP...
        else if (GameManager.Instance.userdb.cofres.Count > 0) PutChest(GameManager.Instance.userdb.cofres[0]); //Si SOLO tienes cofres NORMALES...
        else chest.gameObject.SetActive(false);


    }

    private void PutChest(int n)
    {
        switch (n)
        {
            case 1: selected_chest_type = Chest.Cutre1; break;
            case 2: selected_chest_type = Chest.Cutre2; break;
            case 3: selected_chest_type = Chest.VIP1; break;
            case 4: selected_chest_type = Chest.VIP2; break;
            case 5: selected_chest_type = Chest.VIP3; break;
            default: selected_chest_type = Chest.None; break;
        }
        PutChestSpriteByType(selected_chest_type);
        chest_button.interactable = true;
    }

    private void PutChestSpriteByType(Chest chest_type)
    {
        switch (chest_type)
        {
            case Chest.Cutre1: PutChestSprites(s_chest1); break;
            case Chest.Cutre2: PutChestSprites(s_chest2); break;
            case Chest.VIP1: PutChestSprites(s_chestVIP1); break;
            case Chest.VIP2: PutChestSprites(s_chestVIP2); break;
            case Chest.VIP3: PutChestSprites(s_chestVIP3); break;
        }
    }

    //private void SetChestType(bool VIP)
    //{
    //    chest.gameObject.SetActive(false);

    //    if (VIP) PutChestSprites(s_chestVIP, s_shadow_chestVIP);
    //    else PutChestSprites(s_chest, s_shadow_chest);

    //    buy_chest.gameObject.SetActive(!VIP);
    //    buy_chest_VIP.gameObject.SetActive(VIP);

    //    chest.gameObject.SetActive(true);
    //    chest.GetComponent<Animator>().Play("Chest_Caer");
    //}

    public void MostrarIconoCofres()
    {
        int n = 0;
        int h = 4 - GameManager.Instance.userdb.cofres.Count;
        foreach (int i in GameManager.Instance.userdb.cofres)
        {
            Image image = chest_slots.transform.GetChild(n).Find("Image").GetComponent<Image>();
            image.sprite = i == 1 ? s_chest1 : s_chest2;
            image.color = Color.white;
            n++;
        }
        for (var x = 4; x > n; x--)
        {
            chest_slots.transform.GetChild(n).Find("Image").GetComponent<Image>().color = Color.clear;
        }
    }

    void OnDisable()
    {
        chest.gameObject.SetActive(false);
    }

    private void DefaultSetup()
    {
        buy_chest_VIP.gameObject.SetActive(false);
        buy_chest_VIP.gameObject.SetActive(true);
        //SetChestType(false);
    }

    void Update()
    {
        if (gameObject.activeSelf) resplandor.Rotate(Vector3.forward * 80 * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Escape)) VOLVER();
    }

	public void VOLVER()
    {
        AudioSystem.Instance.Sound_ClickEquipItem();
        transform.gameObject.SetActive(false);
        CanvasBase.Instance.BackToMenu();
    }

    public void BTN_ABRIR_COMPRAS()
    {
        transform.parent.Find("Compras").gameObject.SetActive(true);
        transform.gameObject.SetActive(false);
    }

    public void BTN_CERRAR_COMPRAS()
    {
        transform.parent.Find("Compras").gameObject.SetActive(false);
        transform.gameObject.SetActive(true);
    }

    public void BTN_ABRIR_COFRE()
    {
        if (IsVip(selected_chest_type)) AbrirCofre();
        else ChestTimer.Instance.CompareTimes();
    }

    public void UpdateChestsAmount()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("data").Child("chests").GetValueAsync()
            .ContinueWith(task => {
                if (task.IsCompleted)
                {
                    DataSnapshot snap = task.Result;
                    SetChestAmount(snap.Value.ToString());
                }
            });
    }

    public void SetChestAmount(string amount)
    {
        text_chest.text = amount;
        chest_button.interactable = true;
    }

    void SetChestAmount_VIP(string amount)
    {
        text_chest_VIP.text = amount;
    }

    public void COMPRAR_COFRE()
    {
        Button cofre = transform.Find("Buy_Chest").GetComponent<Button>();
        cofre.interactable = false;
        int gold;
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("data").Child("gold").GetValueAsync()
            .ContinueWith(task => {
                if (task.IsCompleted)
                {
                    DataSnapshot snap = task.Result;
                    gold = int.Parse(snap.Value.ToString());
                    if(gold >= 100)
                    {
                        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("data").Child("gold")
                        .SetValueAsync(gold - 100);
                        AñadirCofre(gold - 100);
                    }else
                    {
                        Message.Instance.NewMessage("Necesitas más gemas");
                    }
                    cofre.interactable = true;
                }

            });   
    }

    private void AñadirCofre(int goldNow)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("data").Child("chests").GetValueAsync()
            .ContinueWith(task => {
                if (task.IsCompleted)
                {
                    DataSnapshot snap = task.Result;
                    int nCofres = int.Parse(snap.Value.ToString());

                    FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("data").Child("chests")
                    .SetValueAsync(nCofres + 1).ContinueWith(task2 => {
                        UpdateChestsAmount();
                        CanvasBase.Instance.UpdateGoldViewNoDB(goldNow.ToString());
                        transform.Find("Cofres").Find("Buy_Chest").GetComponent<Button>().interactable = true;
                    });
                }
            });
    }

    public void SetOpenedSprite()
    {
        chest.Find("Chest").GetComponent<Image>().sprite = SpriteChestOpenedByType(selected_chest_type);
    }

    public Sprite SpriteChestOpenedByType(Chest type)
    {
        switch (type)
        {
            case Chest.Cutre1: return s_chest1_opened;
            case Chest.Cutre2: return s_chest2_opened;
            case Chest.VIP1: return s_chestVIP1_opened;
            case Chest.VIP2: return s_chestVIP2_opened;
            case Chest.VIP3: return s_chestVIP3_opened;
            default: return null;
        }
    }

    private void SetNormalChest(int n)
    {
        chest.gameObject.SetActive(false);
    }

    private void PutChestSprites(Sprite chestSprite)
    {
        //Image image = chest.GetComponent<Image>();
        //image.sprite = shadow;
        //image.preserveAspect = true;
        Image retrato = chest.Find("Chest").GetComponent<Image>();
        retrato.sprite = chestSprite;
        retrato.sprite = chestSprite;
        retrato.preserveAspect = true;
        chest.gameObject.SetActive(true);
        chest.GetComponent<Animator>().Play("Chest_Caer");
    }

    private bool IsVip(Chest chest_type)
    {
        switch (chest_type)
        {
            case Chest.Cutre1: return false;
            case Chest.Cutre2: return false;
            case Chest.VIP1: return true;
            case Chest.VIP2: return true;
            case Chest.VIP3: return true;
            default: return false;
        }
    }

    public void AbrirCofre()
    {
        chest_button.interactable = false;
        bool isVIP = IsVip(selected_chest_type);
        int nItems = isVIP ? 4 : 2;    //Cantidad de elementos que salen según el tipo de cofre
        if (selected_chest_type == Chest.Cutre2) nItems++;
        List<string> inventario = Items.Instance.GetYourItems();
        List<string> nuevosItems = new List<string>();

        for (var x = 0; x < nItems; x++)
        {
            nuevosItems.Add(Items.Instance.GetRandomItemID(selected_chest_type));
        }

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID());

        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {

                PlayerDB userData = JsonUtility.FromJson<PlayerDB>(task.Result.GetRawJsonValue());
                GetComponent<Canvas>().targetDisplay = 1;
                chest.GetComponent<Animator>().Play("Chest_Abrir");
                CofreAbierto.Instance.GuardarNuevosItems(nuevosItems);
                EquipDB eDB = userData.equipamiento;
                if (!isVIP) userData.data.cofres.RemoveAt(0);
                else userData.data.cofres_VIP.RemoveAt(0);

                foreach (string s in nuevosItems) inventario.Add(s);

                userData.items = inventario;
                userData.equipamiento = eDB;

                string json = JsonUtility.ToJson(userData);

                reference.SetRawJsonValueAsync(json).ContinueWith((obj2) =>
                {
                    if (obj2.IsCompleted)
                    {
                        CanvasBase.Instance.LoadYourItems();
                        UpdateChestsAmount();
                    }
                });

            }
        });
    }

}
