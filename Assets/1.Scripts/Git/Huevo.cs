using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using System;
using Enums;

public class Huevo : MonoBehaviour {

	public void BTN_HUEVO()
    {
        transform.Find("Huevo").GetComponent<Button>().interactable = false;

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        string rHeadgear = Items.Instance.GetRandomItemID(Equip_Position.Head, 1);
        string rBody     = Items.Instance.GetRandomItemID(Equip_Position.Body, 1);
        string rArms     = Items.Instance.GetRandomItemID(Equip_Position.Arms, 1);
        string rLegs     = Items.Instance.GetRandomItemID(Equip_Position.Legs, 1);

        Equipment equipment = new Equipment()
        {
            head = Items.Instance.ItemByID(rHeadgear),
            body = Items.Instance.ItemByID(rBody),
            arms = Items.Instance.ItemByID(rArms),
            legs = Items.Instance.ItemByID(rLegs)
        };

        List<string> itemList = new List<string>();
        itemList.Add(rHeadgear);
        itemList.Add(rBody);
        itemList.Add(rArms);
        itemList.Add(rLegs);

        UserDB userData = new UserDB()
        {
            //chests = 0,
            //chests_VIP = 0,
            gold = 0,
            gold_VIP = 0,
            victorias = 0,
            derrotas = 0,
            coronas = 0,
            elo = 1000,
            last_time_reward = new TimeSpan(DateTime.UtcNow.Ticks).Ticks.ToString(),
            nivel = "0100",
            cofres = new List<int>() {1, 2},
            cofres_VIP = new List<int>()
        };

        EquipDB equip = new EquipDB()
        {
            head = rHeadgear,
            body = rBody,
            arms = rArms,
            legs = rLegs
        };

        PlayerDB userInfo = new PlayerDB() {
            items = itemList,
            data = userData,
            equipamiento = equip,
            invitation = new List<string>()
            {
                "inv1",
                "inv2"
            },
            dataTurn = ""
        };

        string jsonObjetos = JsonUtility.ToJson(userInfo);
        string jsonEquip = JsonUtility.ToJson(equip);

        if(GameManager.Instance.GetUserID() != null)
        {
            reference.Child("Inventario").Child(GameManager.Instance.GetUserID()).SetRawJsonValueAsync(jsonObjetos).ContinueWith((obj2) =>
            {
                if (obj2.IsCompleted)
                {
                    transform.Find("Huevo").gameObject.SetActive(false);
                    GoMainMenu();
                }

                if (obj2.IsFaulted)
                {
                    transform.Find("Huevo").GetComponent<Button>().interactable = true;
                }
            });
        }else
        {
            //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //.Build();
            //PlayGamesPlatform.InitializeInstance(config);
            //PlayGamesPlatform.Activate();

            Social.Active.Authenticate(Social.localUser, (bool success) => {
                if (success)
                {
                    BTN_HUEVO();
                }
            });
        }

        
    }

    void GoMainMenu()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene(0);
    }
}
