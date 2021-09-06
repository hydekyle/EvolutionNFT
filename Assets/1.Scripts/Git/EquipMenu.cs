using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Enums;
using System;
using Firebase.Database;

public class EquipMenu : MonoBehaviour
{

    public static EquipMenu Instance;

    int precio = 0;
    Equipable_Item selectedItem = new Equipable_Item();
    public float clampLeft;
    public float distanciaItemsScrollBar = 80f;
    Transform inventarioT, window_mejorar, window_vender;
    Image[] item_image;
    Image[] rarity_image;
    string[] storage_ID;
    Equip_Position currentEquipView = Equip_Position.None;
    List<Equipable_Item> actualList = new List<Equipable_Item>();
    List<int> equip_list_id = new List<int>();
    public GameObject buttonPrefab;
    bool swaping;
    //Item Info Menu:
    Image retrato;
    Text rarity_text, rarity_shadow, skill1_name, skill2_name, skill3_name, value_life, value_attack, value_skill, value_luck;
    Image skill1_orb, skill2_orb, skill3_orb;
    Image skill1_orb2, skill2_orb2, skill3_orb2;


    void Awake()
    {
        Inicialize();
    }

    void Inicialize()
    {
        Instance = this;
        Transform infoT = transform.Find("Info");
        window_mejorar = transform.Find("Window_Mejorar");
        window_vender = transform.Find("Window_Vender");
        retrato = infoT.Find("Retrato").GetComponent<Image>();
        skill1_name = infoT.Find("Skill1").Find("Name").GetComponent<Text>();
        skill2_name = infoT.Find("Skill2").Find("Name").GetComponent<Text>();
        skill3_name = infoT.Find("Skill3").Find("Name").GetComponent<Text>();
        skill1_orb = infoT.Find("Skill1").Find("Orbe").GetComponent<Image>();
        skill2_orb = infoT.Find("Skill2").Find("Orbe").GetComponent<Image>();
        skill3_orb = infoT.Find("Skill3").Find("Orbe").GetComponent<Image>();
        skill1_orb2 = infoT.Find("Skill1").Find("Orbe2").GetComponent<Image>();
        skill2_orb2 = infoT.Find("Skill2").Find("Orbe2").GetComponent<Image>();
        skill3_orb2 = infoT.Find("Skill3").Find("Orbe2").GetComponent<Image>();
        rarity_text = infoT.Find("Rarity_Color").GetComponent<Text>();
        rarity_shadow = infoT.Find("Rarity_Shadow").GetComponent<Text>();
        value_life = infoT.Find("Value_Life").GetComponent<Text>();
        value_attack = infoT.Find("Value_Attack").GetComponent<Text>();
        value_skill = infoT.Find("Value_Skill").GetComponent<Text>();
        value_luck = infoT.Find("Value_Luck").GetComponent<Text>();

        Transform inventario = transform.Find("Panel_Inventario").Find("Inventario");
        inventarioT = inventario.Find("Scroll").Find("Botones_Items");
        //buttonPrefab = inventarioT.Find("Button").gameObject;
        storage_ID = new string[inventarioT.childCount];
        item_image = new Image[inventarioT.childCount];
        rarity_image = new Image[inventarioT.childCount];
        for (var x = 0; x < inventarioT.childCount; x++)
        {
            Transform t = inventarioT.GetChild(x);
            rarity_image[x] = t.GetComponent<Image>();
            item_image[x] = t.Find("Image").GetComponent<Image>();
            t.gameObject.name = x.ToString();
        }
    }

    void OnEnable()
    {
        swaping = false;
        currentEquipView = Equip_Position.None;
        BTN_HEAD();
    }

    public IEnumerator ViewItemInfo()
    {
        Sprite itemSprite = null;
        int itemID = 0;
        switch (currentEquipView)
        {
            case Equip_Position.Head: itemID = GameManager.Instance.player.criatura.equipment.head.ID; break;
            case Equip_Position.Body: itemID = GameManager.Instance.player.criatura.equipment.body.ID; break;
            case Equip_Position.Arms: itemID = GameManager.Instance.player.criatura.equipment.arms.ID; break;
            case Equip_Position.Legs: itemID = GameManager.Instance.player.criatura.equipment.legs.ID; break;
        }
        yield return Items.Instance.ItemSpriteByID(itemID, result => itemSprite = result);
        print(itemID);
        retrato.sprite = itemSprite;
        retrato.preserveAspect = true;
    }

    public IEnumerator ViewItemInfo(string bigID)
    {
        Item item = Items.Instance.GetItem(bigID);

        Sprite itemSprite = null;
        yield return Items.Instance.ItemSpriteByID(item.ID, result => itemSprite = result);
        string textRarity = Lenguaje.Instance.Text_RarityID(item.rarity);
        rarity_text.text = textRarity;
        rarity_shadow.text = textRarity;

        rarity_text.color = ColorByQuality(QualityByRarity(item.rarity));
        retrato.sprite = itemSprite;
        retrato.preserveAspect = true;

        value_attack.text = item.attack.ToString();
        value_life.text = item.health.ToString();
        value_skill.text = item.skill.ToString();
        value_luck.text = item.luck.ToString();

        skill1_orb.sprite = skill1_orb2.sprite = Items.Instance.SphereBySkillID(item.skill_1.ID);
        skill2_orb.sprite = skill2_orb2.sprite = Items.Instance.SphereBySkillID(item.skill_2.ID);
        skill3_orb.sprite = skill3_orb2.sprite = Items.Instance.SphereBySkillID(item.skill_3.ID);

        if (Lenguaje.Instance.spanish_language)
        {
            skill1_name.text = item.skill_1.name_spanish;
            skill2_name.text = item.skill_2.name_spanish;
            skill3_name.text = item.skill_3.name_spanish;
        }
        else
        {
            skill1_name.text = item.skill_1.name_english;
            skill2_name.text = item.skill_2.name_english;
            skill3_name.text = item.skill_3.name_english;
        }

    }

    Quality QualityByRarity(int n)
    {
        switch (n)
        {
            case 1: return Quality.Common;
            case 2: return Quality.Rare;
            case 3: return Quality.Epic;
            case 4: return Quality.Legendary;
            default: return Quality.Common;
        }
    }

    List<Equipable_Item> SortListByQuality(List<Equipable_Item> list)
    {
        List<Equipable_Item> sortedList = new List<Equipable_Item>();
        List<Equipable_Item> legendary = new List<Equipable_Item>();
        List<Equipable_Item> epic = new List<Equipable_Item>();
        List<Equipable_Item> rare = new List<Equipable_Item>();
        List<Equipable_Item> common = new List<Equipable_Item>();
        foreach (Equipable_Item e in list)
        {
            switch (e.quality)
            {
                case Quality.Common: common.Add(e); break;
                case Quality.Rare: rare.Add(e); break;
                case Quality.Epic: epic.Add(e); break;
                case Quality.Legendary: legendary.Add(e); break;
            }
        }
        foreach (Equipable_Item e in legendary) sortedList.Add(e);
        foreach (Equipable_Item e in epic) sortedList.Add(e);
        foreach (Equipable_Item e in rare) sortedList.Add(e);
        foreach (Equipable_Item e in common) sortedList.Add(e);
        return sortedList;
    }

    public Color ColorByQuality(Quality q)
    {
        Color color = new Color();
        switch (q)
        {
            case Quality.Legendary: color = Color.yellow; break;
            case Quality.Epic: color = Color.magenta; break;
            case Quality.Rare: color = Color.cyan; break;
            case Quality.Common: color = Color.white; break;
        }
        return color;
    }

    //Color ColorByQuality(int i)
    //{
    //    Color color = new Color();
    //    switch (i)
    //    {
    //        case 4: color = new Color(255f, 0f, 255f); break;
    //        case 3: color = color = Color.yellow; break;
    //        case 2: color = color = Color.cyan; break;
    //        case 1: color = color = Color.white; break;
    //    }
    //    return color;
    //}

    bool CheckEquipedID(int id)
    {
        int lista = int.Parse(id.ToString().Substring(0, 1));
        Equipment e = GameManager.Instance.player.criatura.equipment;
        return true;
    }

    IEnumerator VisualizarScroll(Equip_Position equipList, Action<bool> ended)
    {
        if (equipList != currentEquipView && GameManager.Instance.player != null)
        {
            currentEquipView = equipList;
            DeselectLastItem();
            string equipedID = "";

            foreach (Transform t in inventarioT) //Limpiar casillas previas
            {
                Destroy(t.gameObject);
            }
            inventarioT.gameObject.SetActive(false);
            inventarioT.localPosition = Vector3.zero;

            switch (equipList)
            {
                case Equip_Position.Head:
                    actualList = SortListByQuality(Items.Instance.inventory_headgear);
                    equipedID = GameManager.Instance.player.criatura.equipment.head.ID_string;
                    CanvasBase.Instance.ShowItemInfo(GameManager.Instance.player.criatura.equipment.head.ID_string);
                    break;
                case Equip_Position.Body:
                    actualList = SortListByQuality(Items.Instance.inventory_bodies);
                    equipedID = GameManager.Instance.player.criatura.equipment.body.ID_string;
                    CanvasBase.Instance.ShowItemInfo(GameManager.Instance.player.criatura.equipment.body.ID_string);
                    break;
                case Equip_Position.Arms:
                    actualList = SortListByQuality(Items.Instance.inventory_arms);
                    equipedID = GameManager.Instance.player.criatura.equipment.arms.ID_string;
                    CanvasBase.Instance.ShowItemInfo(GameManager.Instance.player.criatura.equipment.arms.ID_string);
                    break;
                case Equip_Position.Legs:
                    actualList = SortListByQuality(Items.Instance.inventory_legs);
                    equipedID = GameManager.Instance.player.criatura.equipment.legs.ID_string;
                    CanvasBase.Instance.ShowItemInfo(GameManager.Instance.player.criatura.equipment.legs.ID_string);
                    break;
            }
            storage_ID = new string[actualList.Count];

            int n = 0;
            foreach (Equipable_Item e in actualList)
            {
                yield return Colocar_Pieza_Slider(e.ID_string, n);
                n++;
                //yield return new WaitForEndOfFrame();
            }
            if (n > 1)
            {
                float distance = Vector3.Distance(inventarioT.GetChild(0).localPosition, inventarioT.GetChild(1).localPosition);
                clampLeft = -distance * n + distance;
            }
            else
            {
                clampLeft = 0;
            }
            ended(true);
        }
    }

    void DisableButtonEquiped(Equip_Position position)
    {
        string equipedID = "";
        switch (position)
        {
            case Equip_Position.Head: equipedID = GameManager.Instance.player.criatura.equipment.head.ID_string; break;
            case Equip_Position.Body: equipedID = GameManager.Instance.player.criatura.equipment.body.ID_string; break;
            case Equip_Position.Arms: equipedID = GameManager.Instance.player.criatura.equipment.arms.ID_string; break;
            case Equip_Position.Legs: equipedID = GameManager.Instance.player.criatura.equipment.legs.ID_string; break;
        }

        for (var x = 0; x < storage_ID.Length; x++)
        {
            if (storage_ID[x] == equipedID) inventarioT.GetChild(x).GetComponent<Button>().interactable = false;
            else inventarioT.GetChild(x).GetComponent<Button>().interactable = true;
        }
        inventarioT.gameObject.SetActive(true);
    }

    IEnumerator Colocar_Pieza_Slider(string itemID, int n)
    {
        GameObject go = Instantiate(buttonPrefab, inventarioT);
        Quality itemQuality = Quality.Common;

        switch (int.Parse(itemID.Substring(3, 1)))
        {
            case 1: itemQuality = Quality.Common; break;
            case 2: itemQuality = Quality.Rare; break;
            case 3: itemQuality = Quality.Epic; break;
            case 4: itemQuality = Quality.Legendary; break;
        }

        go.GetComponent<Image>().color = ColorByQuality(itemQuality);
        go.name = n.ToString();
        yield return Items.Instance.ItemSpriteByID(int.Parse(itemID.Substring(0, 3)), result =>
        {
            Image image = go.transform.Find("Image").GetComponent<Image>();
            image.sprite = result;
            image.preserveAspect = true;
        });
        try
        {
            Vector3 pos0 = new Vector3(0, 0, 0);
            Vector3 posPlus = new Vector3(distanciaItemsScrollBar * n, 0, 0);
            go.transform.localPosition = pos0 + posPlus;
            go.GetComponent<Button>().onClick.AddListener(BTN_ITEM);
            go.GetComponent<Button>().onClick.AddListener(AudioSystem.Instance.Sound_ClickEquipItem);
            storage_ID[n] = itemID;
            go.SetActive(true);
        }
        catch { }

    }

    private void UpdateAdn()
    {
        if (window_vender.gameObject.activeSelf) window_vender.Find("Adn").Find("Text").GetComponent<Text>().text = GameManager.Instance.userdb.gold.ToString();
        else window_mejorar.Find("Adn").Find("Text").GetComponent<Text>().text = GameManager.Instance.userdb.gold.ToString();
    }

    private void GuardarInventarioDB()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID());

        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                List<string> inventario = Items.Instance.GetYourItems();
                PlayerDB userData = JsonUtility.FromJson<PlayerDB>(task.Result.GetRawJsonValue());
                userData.data.gold = userData.data.gold + precio;
                EquipDB eDB = new EquipDB()
                {
                    head = GameManager.Instance.player.criatura.equipment.head.ID_string,
                    body = GameManager.Instance.player.criatura.equipment.body.ID_string,
                    arms = GameManager.Instance.player.criatura.equipment.arms.ID_string,
                    legs = GameManager.Instance.player.criatura.equipment.legs.ID_string,
                };
                GameManager.Instance.userdb = userData.data;
                userData.items = inventario;
                userData.equipamiento = eDB;

                string json = JsonUtility.ToJson(userData);

                reference.SetRawJsonValueAsync(json).ContinueWith((obj2) =>
                {
                    if (obj2.IsCompleted)
                    {
                        print("Guardado.");
                        SetActiveBotons(true);
                        UpdateAdn();
                    }
                });
            }
        });
    }

    public void BTN_HEAD()
    {
        try
        {
            StartCoroutine(VisualizarScroll(Equip_Position.Head, ended => { DisableButtonEquiped(Equip_Position.Head); }));
        }
        catch { }
    }

    public void BTN_BODY()
    {
        StartCoroutine(VisualizarScroll(Equip_Position.Body, ended => { DisableButtonEquiped(Equip_Position.Body); }));
    }

    public void BTN_ARMS()
    {
        StartCoroutine(VisualizarScroll(Equip_Position.Arms, ended => { DisableButtonEquiped(Equip_Position.Arms); }));
    }

    public void BTN_LEGS()
    {
        StartCoroutine(VisualizarScroll(Equip_Position.Legs, ended => { DisableButtonEquiped(Equip_Position.Legs); }));
    }

    public void BTN_ITEM()
    {
        if (window_vender.gameObject.activeSelf)
        {
            SelectToSell();
        }
        else if (!swaping)
        {
            swaping = true;
            try
            {
                Equip_Item_ListID(int.Parse(EventSystem.current.currentSelectedGameObject.name));
            }
            catch { print("Oops"); }
        }
    }

    private void SelectToSell()
    {
        selectedItem = Items.Instance.ItemByID(storage_ID[int.Parse(EventSystem.current.currentSelectedGameObject.name)]);
        StartCoroutine(ViewItemInfo(selectedItem.ID_string));
        precio = int.Parse(selectedItem.ID_string.Substring(3, 1)) * 40;
        window_vender.Find("Vender").Find("Adn").Find("Text").GetComponent<Text>().text = "+ " + precio.ToString();
    }

    private void Equip_Item_ListID(int listID)
    {
        if (storage_ID[listID].Length > 0)
        {
            EquiparItem(storage_ID[listID]);
            switch (int.Parse(storage_ID[listID].Substring(0, 1)))
            {
                case 1: StartCoroutine(CanvasBase.Instance.MostrarPieza(Equip_Position.Head, ended => { swaping = false; })); break;
                case 2: StartCoroutine(CanvasBase.Instance.MostrarPieza(Equip_Position.Body, ended => { swaping = false; })); break;
                case 3: StartCoroutine(CanvasBase.Instance.MostrarPieza(Equip_Position.Arms, ended => { swaping = false; })); break;
                case 4: StartCoroutine(CanvasBase.Instance.MostrarPieza(Equip_Position.Legs, ended => { swaping = false; })); break;
            }
            StartCoroutine(Menu.Instance.VisualizarEquipamiento(GameManager.Instance.player.criatura.equipment, 1));
            StartCoroutine(ViewItemInfo());
            CanvasBase.Instance.StatsRefresh();
            DisableButtonEquiped(currentEquipView);
        }
    }

    void EquiparItem(string bigID)
    {
        CanvasBase.Instance.ShowItemInfo(bigID);
        int list = int.Parse(bigID.ToString().Substring(0, 1));
        switch (list)
        {
            case 1: GameManager.Instance.player.criatura.equipment.head = Items.Instance.ItemByID(bigID); break;
            case 2: GameManager.Instance.player.criatura.equipment.body = Items.Instance.ItemByID(bigID); break;
            case 3: GameManager.Instance.player.criatura.equipment.arms = Items.Instance.ItemByID(bigID); break;
            case 4: GameManager.Instance.player.criatura.equipment.legs = Items.Instance.ItemByID(bigID); break;
        }
    }

    public void BTN_EQUIP_INFO()
    {
        GameObject info_window = transform.Find("Info").gameObject;
        info_window.SetActive(info_window.activeSelf ? false : true);
    }

    public void BTN_WINDOW_MEJORAR()
    {
        Open_Window_Mejorar();
    }

    private void SetActiveBotons(bool on)
    {
        if (!on)
        {
            window_mejorar.Find("BTN_Stats").GetComponent<Button>().enabled = false;
            window_mejorar.Find("BTN_Skills").GetComponent<Button>().enabled = false;
        }
        else
        {
            StartCoroutine(Fixer());
        }
    }

    IEnumerator Fixer()
    {
        yield return new WaitForSeconds(1f);
        window_mejorar.Find("BTN_Stats").GetComponent<Button>().enabled = true;
        window_mejorar.Find("BTN_Skills").GetComponent<Button>().enabled = true;
    }

    public void BTN_CAMBIAR_STATS()
    {
        SetActiveBotons(false);
        Database.Instance.ReferenceDataDB().Child("gold").GetValueAsync().ContinueWith(task =>
        {
            int myMoney = int.Parse(task.Result.Value.ToString());
            if (myMoney >= 80)
            {
                Equipable_Item eItem = new Equipable_Item();
                Equipable_Item newItem = new Equipable_Item();
                bool removed = false;
                switch (currentEquipView)
                {
                    case Equip_Position.Head:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.head.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReStats(eItem.ID_string));
                        if (Items.Instance.inventory_headgear.Remove(eItem))
                        {
                            removed = true;
                            Items.Instance.inventory_headgear.Add(newItem);
                            GameManager.Instance.player.criatura.equipment.head = newItem;
                        }
                        break;
                    case Equip_Position.Body:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.body.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReStats(eItem.ID_string));
                        if (Items.Instance.inventory_bodies.Remove(eItem))
                        {
                            removed = true;
                            Items.Instance.inventory_bodies.Add(newItem);
                            GameManager.Instance.player.criatura.equipment.body = newItem;
                        }
                        break;
                    case Equip_Position.Arms:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.arms.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReStats(eItem.ID_string));
                        if (Items.Instance.inventory_arms.Remove(eItem))
                        {
                            removed = true;
                            Items.Instance.inventory_arms.Add(newItem);
                            GameManager.Instance.player.criatura.equipment.arms = newItem;
                        }
                        break;
                    case Equip_Position.Legs:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.legs.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReStats(eItem.ID_string));
                        if (Items.Instance.inventory_legs.Remove(eItem))
                        {
                            removed = true;
                            Items.Instance.inventory_legs.Add(newItem);
                            GameManager.Instance.player.criatura.equipment.legs = newItem;
                        }
                        break;
                }
                if (removed)
                {
                    StartCoroutine(ViewItemInfo(newItem.ID_string));
                    precio = -80;
                    GuardarInventarioDB();
                }
            }
            else
            {
                SetActiveBotons(true);
            }

        });
    }

    public void BTN_CAMBIAR_SKILLS()
    {
        SetActiveBotons(false);
        Database.Instance.ReferenceDataDB().Child("gold").GetValueAsync().ContinueWith(task =>
        {
            int myMoney = int.Parse(task.Result.Value.ToString());
            if (myMoney >= 80)
            {
                Equipable_Item eItem = new Equipable_Item();
                Equipable_Item newItem = new Equipable_Item();
                switch (currentEquipView)
                {
                    case Equip_Position.Head:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.head.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReSkills(eItem.ID_string));
                        Items.Instance.inventory_headgear.Remove(eItem);
                        Items.Instance.inventory_headgear.Add(newItem);
                        GameManager.Instance.player.criatura.equipment.head = newItem;
                        break;
                    case Equip_Position.Body:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.body.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReSkills(eItem.ID_string));
                        Items.Instance.inventory_bodies.Remove(eItem);
                        Items.Instance.inventory_bodies.Add(newItem);
                        GameManager.Instance.player.criatura.equipment.body = newItem;
                        break;
                    case Equip_Position.Arms:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.arms.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReSkills(eItem.ID_string));
                        Items.Instance.inventory_arms.Remove(eItem);
                        Items.Instance.inventory_arms.Add(newItem);
                        GameManager.Instance.player.criatura.equipment.arms = newItem;
                        break;
                    case Equip_Position.Legs:
                        eItem = Items.Instance.ItemByID(GameManager.Instance.player.criatura.equipment.legs.ID_string);
                        newItem = Items.Instance.ItemByID(ItemReSkills(eItem.ID_string));
                        Items.Instance.inventory_legs.Remove(eItem);
                        Items.Instance.inventory_legs.Add(newItem);
                        GameManager.Instance.player.criatura.equipment.legs = newItem;
                        break;
                }
                StartCoroutine(ViewItemInfo(newItem.ID_string));
                precio = -80;
                GuardarInventarioDB();
            }
            SetActiveBotons(true);
        });
    }

    private string ItemReSkills(string itemID)
    {
        string newItem = itemID;
        string newSkills = Items.Instance.GenerateNewSkills();
        char[] charsSkills = newSkills.ToCharArray();
        char[] charsItem = newItem.ToCharArray();
        byte n = 8;
        foreach (char c in charsSkills)
        {
            charsItem[n++] = c;
        }
        string newItemID = "";
        foreach (char c in charsItem)
        {
            newItemID += c;
        }
        return newItemID;
    }

    private string ItemReStats(string itemID)
    {
        int fuerza = 1;
        int vida = 1;
        int skill = 1;
        int luck = 1;
        for (var x = 0; x < 5; x++)
        {
            switch (UnityEngine.Random.Range(1, 5))
            {
                case 1: fuerza++; break;
                case 2: vida++; break;
                case 3: skill++; break;
                case 4: luck++; break;
            }
        }

        char[] chars = itemID.ToCharArray();
        chars[4] = vida.ToString()[0];
        chars[5] = fuerza.ToString()[0];
        chars[6] = skill.ToString()[0];
        chars[7] = luck.ToString()[0];

        string newString = "";

        foreach (char c in chars)
        {
            newString += c;
        }

        return newString;
    }

    public void BTN_Vender()
    {
        DeselectLastItem();
        Open_Window_Vender();
    }

    public void BTN_Vender_Pieza()
    {
        if (selectedItem.ID_string != null) transform.Find("ConfirmarVenta").gameObject.SetActive(true);
    }

    private void DeselectLastItem()
    {
        selectedItem = new Equipable_Item();
        window_vender.Find("Vender").Find("Adn").Find("Text").GetComponent<Text>().text = "Selecciona para vender";
        StartCoroutine(ViewItemInfo());
    }

    public void BTN_ConfirmarVenta()
    {
        switch (currentEquipView)
        {
            case Equip_Position.Head:
                if (Items.Instance.inventory_headgear.Count > 1)
                {
                    Items.Instance.inventory_headgear.Remove(selectedItem);
                    currentEquipView = Equip_Position.None;
                    BTN_HEAD();
                }
                break;
            case Equip_Position.Body:
                if (Items.Instance.inventory_bodies.Count > 1)
                {
                    Items.Instance.inventory_bodies.Remove(selectedItem);
                    currentEquipView = Equip_Position.None;
                    BTN_BODY();
                }
                break;
            case Equip_Position.Arms:
                if (Items.Instance.inventory_arms.Count > 1)
                {
                    Items.Instance.inventory_arms.Remove(selectedItem);
                    currentEquipView = Equip_Position.None;
                    BTN_ARMS();
                }
                break;
            case Equip_Position.Legs:
                if (Items.Instance.inventory_legs.Count > 1)
                {
                    Items.Instance.inventory_legs.Remove(selectedItem);
                    currentEquipView = Equip_Position.None;
                    BTN_LEGS();
                }
                break;
        }
        print("Lo has vendido, chamo: " + selectedItem.ID);
        GuardarInventarioDB();
        transform.Find("ConfirmarVenta").gameObject.SetActive(false);
    }

    public void BTN_CancelarVenta()
    {
        transform.Find("ConfirmarVenta").gameObject.SetActive(false);
    }

    private void Open_Window_Mejorar()
    {

        window_mejorar.gameObject.SetActive(!window_mejorar.gameObject.activeSelf);
        DeselectLastItem();
        window_vender.gameObject.SetActive(false);
        UpdateAdn();
    }

    private void Open_Window_Vender()
    {
        window_vender.gameObject.SetActive(!window_vender.gameObject.activeSelf);
        window_mejorar.gameObject.SetActive(false);
        UpdateAdn();
    }

    void Update()
    {
        if (gameObject.activeSelf && !Input.GetMouseButton(0))
        {
            Vector3 vectorLimite = new Vector3(Mathf.Clamp(inventarioT.localPosition.x, clampLeft, 10), inventarioT.localPosition.y, inventarioT.localPosition.z);
            inventarioT.localPosition = Vector3.Lerp(inventarioT.localPosition, vectorLimite, Time.deltaTime * 10);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) CanvasBase.Instance.BTN_OK_Equipment();
    }

    private void OnDisable()
    {
        window_mejorar.gameObject.SetActive(false);
        window_vender.gameObject.SetActive(false);
    }

}
