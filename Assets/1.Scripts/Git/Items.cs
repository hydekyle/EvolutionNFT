using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Enums;

public class Items : MonoBehaviour {

    public static Items Instance;

    //Sprites Esferas
    public Sprite sphere_alpha, sphere_assassin, sphere_pacifist, sphere_charming, sphere_null;
    public Sprite icon_attack, icon_spell, icon_buff;

    [SerializeField]
    public List<Equipable_Item> headgear_list;
    public List<Equipable_Item> bodies_list;
    public List<Equipable_Item> arms_list;
    public List<Equipable_Item> legs_list;
    public List<Equipable_Item> backs_list;
    public List<Equipable_Item> weapons_list;

    public List<Equipable_Item> inventory_headgear;
    public List<Equipable_Item> inventory_bodies;
    public List<Equipable_Item> inventory_arms;
    public List<Equipable_Item> inventory_legs;


    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            DameCofres();
        }
    }

    void DameCofres()
    {
        Database.Instance.ReferenceDataDB().GetValueAsync().ContinueWith(task => {
            UserDB userData = JsonUtility.FromJson<UserDB>(task.Result.GetRawJsonValue());
            userData.cofres = new List<int>()
            {
                1, 2, 2, 1
            };
            Database.Instance.ReferenceDataDB().SetRawJsonValueAsync(JsonUtility.ToJson(userData));
        });
    }

    void DameloTodoBaby()
    {
        List<Equipable_Item> arms = new List<Equipable_Item>();
        foreach (Equipable_Item e in arms_list)
        {
            arms.Add(ItemByID(e.ID + GetRandomItemID().Substring(3)));
        }
        inventory_arms = arms;

        List<Equipable_Item> bodis = new List<Equipable_Item>();
        foreach (Equipable_Item e in bodies_list)
        {
            bodis.Add(ItemByID(e.ID + GetRandomItemID().Substring(3)));
        }
        inventory_bodies = bodis;

        List<Equipable_Item> legs = new List<Equipable_Item>();
        foreach (Equipable_Item e in legs_list)
        {
            legs.Add(ItemByID(e.ID + GetRandomItemID().Substring(3)));
        }
        inventory_legs = legs;

        List<Equipable_Item> heads = new List<Equipable_Item>();
        foreach (Equipable_Item e in headgear_list)
        {
            heads.Add(ItemByID(e.ID + GetRandomItemID().Substring(3)));
        }
        inventory_headgear = heads;
    }

    public Sprite SphereBySkillID(int ID)
    {
        Sprite sprite = null;
        switch (Skills.Instance.SkillByID(ID).s_class)
        {
            case Skill_Class.Alpha: sprite = sphere_alpha; break;
            case Skill_Class.Assassin: sprite = sphere_assassin; break;
            case Skill_Class.Charming: sprite = sphere_charming; break;
            case Skill_Class.Pacifist: sprite = sphere_pacifist; break;
        }
        return sprite;
    }

    public Sprite IconBySkillID(int ID)
    {
        Sprite sprite = null;

        switch (Skills.Instance.SkillByID(ID).s_type)
        {
            case Skill_Type.Attack: sprite = icon_attack; break;
            case Skill_Type.Spell: sprite = icon_spell; break;
            default: sprite = icon_buff; break;
        }

        return sprite;
    }

    public BaseStats CalcularTotalBasePoints(Equipment equip)
    {
        BaseStats puntos = new BaseStats();

        List<Item> items = new List<Item>
        {
            GetItem(equip.head.ID_string),
            GetItem(equip.body.ID_string),
            GetItem(equip.arms.ID_string),
            GetItem(equip.legs.ID_string)
        };

        foreach (Item item in items)
        {
            puntos.strenght += item.attack;
            puntos.health += item.health;
            puntos.skill += item.skill;
            puntos.luck += item.luck;
            List<int> listaSkills = new List<int>
            {
                int.Parse(item.skill_1.ID.ToString().Substring(0, 1)),
                int.Parse(item.skill_2.ID.ToString().Substring(0, 1)),
                int.Parse(item.skill_3.ID.ToString().Substring(0, 1))
            };

            foreach (int i in listaSkills)
            {
                switch (i)
                {
                    case 1: puntos.assassin++; break;
                    case 2: puntos.alpha++; break;
                    case 3: puntos.charming++; break;
                    case 4: puntos.pacifist++; break;
                }
            }

        }

        return puntos;
    }

    public void StoreClear()
    {
        inventory_headgear = new List<Equipable_Item>();
        inventory_bodies = new List<Equipable_Item>();
        inventory_arms = new List<Equipable_Item>();
        inventory_legs = new List<Equipable_Item>();
    }

    public void StoreItem(string bigID)
    {
        int list = int.Parse(bigID.Substring(0, 1));
        switch (list)
        {
            case 1: inventory_headgear.Add(ItemByID(bigID)); break;
            case 2: inventory_bodies.Add(ItemByID(bigID)); break;
            case 3: inventory_arms.Add(ItemByID(bigID)); break;
            case 4: inventory_legs.Add(ItemByID(bigID)); break;
        }
    }

    public List<string> GetYourItems()
    {
        List<string> objetos = new List<string>();
        foreach (Equipable_Item e in inventory_headgear) objetos.Add(e.ID_string);
        foreach (Equipable_Item e in inventory_bodies) objetos.Add(e.ID_string);
        foreach (Equipable_Item e in inventory_arms) objetos.Add(e.ID_string);
        foreach (Equipable_Item e in inventory_legs) objetos.Add(e.ID_string);
        return objetos;
    }

    public Equipable_Item ItemByID(string bigID)
    {
        Equipable_Item item = new Equipable_Item();
        int listID = int.Parse(bigID.Substring(0, 1));
        int basicID = int.Parse(bigID.Substring(0, 3));
        int rarity = int.Parse(bigID.Substring(3, 1));
        try                                                                         //BUSCAR OBJETO EN LA LISTA CORRESPONDIENTE
        {
            switch (listID)
            {
                case 1: item = headgear_list.Find(e => e.ID == basicID); break;
                case 2: item = bodies_list.Find(e => e.ID == basicID); break;
                case 3: item = arms_list.Find(e => e.ID == basicID); break;
                case 4: item = legs_list.Find(e => e.ID == basicID); break;
                case 5: item = backs_list.Find(e => e.ID == basicID); break;
                case 6: item = weapons_list.Find(e => e.ID == basicID); break;
            }
        } catch { print("¡¡Item no encontrado!!"); }
        switch (rarity)
        {
            case 1: item.quality = Quality.Common; break;
            case 2: item.quality = Quality.Rare; break;
            case 3: item.quality = Quality.Epic; break;
            case 4: item.quality = Quality.Legendary; break;
        }
        item.ID = basicID;
        item.ID_string = bigID;
        return item;
    }

    public IEnumerator ItemSpriteByID(int id, System.Action<Sprite> result)
    {
        if (id > 0)
        {
            int listNumber = int.Parse(id.ToString().Substring(0, 1));
            string spriteNumber = int.Parse(id.ToString().Substring(1, 2)).ToString();
            string localizador = "";

            switch (listNumber)
            {
                case 1: localizador = "/Headgear/Cabeza" + spriteNumber; break;
                case 2: localizador = "/Bodies/Cuerpo" + spriteNumber; break;
                case 3: localizador = "/Arms/Brazo" + spriteNumber; break;
                case 4: localizador = "/Legs/Piernas" + spriteNumber; break;
                case 5: localizador = "/Backs/Traseras" + spriteNumber; break;
                case 6: localizador = "/Weapons/Arma" + spriteNumber; break;
            }

            Texture2D texture = (Texture2D)Resources.Load("Piezas" + localizador);
            Sprite mySprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
            result(mySprite);
            yield return new WaitForEndOfFrame();
            /*  CARGAR TEXTURAS ONLINE SI NO EXISTEN (FUNCIÓN DESCARTADA)
            if (File.Exists(Application.persistentDataPath + localizador))
            {
                byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + localizador);
                Texture2D texture = new Texture2D(1, 1);
                yield return texture.LoadImage(bytes);
                Sprite mySprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
                result(mySprite);
            }
            else
            {
                WWW www = new WWW(texturesWebFolder + localizador);
                yield return www;
                Texture2D texture = www.texture;
                byte[] bytes = texture.EncodeToPNG();
                try { File.WriteAllBytes(Application.persistentDataPath + localizador, bytes);} catch { print("Espacio en disco insuficiente"); }
                Sprite mySprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
                result(mySprite);
            }*/
        }

    }

    public string GetRandomItemID()
    {
        Equipable_Item newItem = new Equipable_Item();
        switch(Random.Range(1, 5)) //Equip Position
        {
            case 1: newItem = headgear_list[Random.Range(0, headgear_list.Count - 1)]; break;
            case 2: newItem = bodies_list[Random.Range(0, bodies_list.Count - 1)]; break;
            case 3: newItem = arms_list[Random.Range(0, arms_list.Count - 1)]; break;
            case 4: newItem = legs_list[Random.Range(0, legs_list.Count - 1)]; break;
        }
        int random = Random.Range(0, 101);
        int rarity = 1;
        if      (random >= 95) rarity = 4;
        else if (random >= 85) rarity = 3;
        else if (random >= 60) rarity = 2;
        int fuerza = 1;
        int vida = 1;
        int skill = 1;
        int luck = 1;
        for (var x = 0; x < 5; x++)
        {
            switch (Random.Range(1, 5))
            {
                case 1: fuerza++; break;
                case 2: vida++; break;
                case 3: skill++; break;
                case 4: luck++; break;
                default: luck++; break;
            }
        }

        string skills = GenerateNewSkills();

        newItem.ID_string = newItem.ID.ToString() + rarity.ToString() + vida.ToString() + fuerza.ToString() + 
                                 skill.ToString() + luck.ToString() + skills;

        return newItem.ID_string;
    }

    public string GetRandomItemID(Chest chestRarity)
    {
        Equipable_Item newItem = GetRandomItemByEvolution(GameManager.Instance.GetMyEvolution());
        int random = Random.Range(0, 101);
        int rarity = GetRarityValue(chestRarity);

        int fuerza = 1;
        int vida = 1;
        int skill = 1;
        int luck = 1;
        for (var x = 0; x < 5; x++)
        {
            switch (Random.Range(1, 5))
            {
                case 1: fuerza++; break;
                case 2: vida++; break;
                case 3: skill++; break;
                case 4: luck++; break;
                default: luck++; break;
            }
        }

        string skills = GenerateNewSkills();

        newItem.ID_string = newItem.ID.ToString() + rarity.ToString() + vida.ToString() + fuerza.ToString() +
                                 skill.ToString() + luck.ToString() + skills;

        return newItem.ID_string;
    }

    private int GetRarityValue(Chest chestLVL)
    {
        int random = Random.Range(1, 101);

        switch (chestLVL)
        {
            case Chest.Cutre1: if (random >= 95) return 3; else if (random >= 75) return 2; else return 1;
            case Chest.Cutre2: if (random >= 85) return 3; else if (random >= 65) return 2; else return 1;
            case Chest.VIP1: if (random >= 85) return 4; else if (random >= 65) return 3; else return 2;
            case Chest.VIP2: if (random >= 50) return 4; else return 3;
            case Chest.VIP3: if (random >= 20) return 4; else return 3;
            default: return 1;
        }
    }

    public string GenerateNewSkills()
    {
        string skills = "";

        int skill1ID;
        int skill2ID;
        int skill3ID;
        do
        {
            skill1ID = GetRandomSkillID();
            skill2ID = GetRandomSkillID();
            skill3ID = GetRandomSkillID();
        } while (skill1ID == skill2ID || skill1ID == skill3ID || skill2ID == skill3ID);

        skills = skill1ID.ToString() + skill2ID.ToString() + skill3ID.ToString();

        return skills;
    }

    public Equipable_Item GetRandomItemByEvolution(Evolution evo)
    {
        Equipable_Item newItem = new Equipable_Item();
        if (evo == Evolution.Creature)
        {
            switch (Random.Range(1, 5)) //Equip Position
            {
                case 1: newItem = headgear_list[Random.Range(0, (headgear_list.Count - 1) / 3)]; break;
                case 2: newItem = bodies_list[Random.Range(0, (bodies_list.Count - 1) / 3)]; break;
                case 3: newItem = arms_list[Random.Range(0, (arms_list.Count - 1)) / 3]; break;
                case 4: newItem = legs_list[Random.Range(0, (legs_list.Count - 1)) / 3]; break;
            }
        }
        else if(evo == Evolution.Tribal)
        {
            switch (Random.Range(1, 5)) //Equip Position
            {
                case 1: newItem = headgear_list[Random.Range((headgear_list.Count - 1) / 3, ((headgear_list.Count - 1) / 3) * 2)]; break;
                case 2: newItem = bodies_list[Random.Range((bodies_list.Count - 1) / 3, ((bodies_list.Count - 1) / 3) * 2)]; break;
                case 3: newItem = arms_list[Random.Range((arms_list.Count - 1) / 3, ((arms_list.Count - 1)) / 3) * 2]; break;
                case 4: newItem = legs_list[Random.Range((legs_list.Count - 1) / 3, ((legs_list.Count - 1) / 3) * 2)]; break;
            }
        }else
        {
            switch (Random.Range(1, 5)) //Equip Position
            {
                case 1: newItem = headgear_list[Random.Range(((headgear_list.Count - 1) / 3) * 2, headgear_list.Count - 1)]; break;
                case 2: newItem = bodies_list[Random.Range(((bodies_list.Count - 1) / 3) * 2, bodies_list.Count - 1)]; break;
                case 3: newItem = arms_list[Random.Range(((arms_list.Count - 1) / 3) * 2, arms_list.Count - 1)]; break;
                case 4: newItem = legs_list[Random.Range(((legs_list.Count - 1) / 3) * 2, legs_list.Count - 1)]; break;
            }
        }
        
        int random = Random.Range(0, 101);
        int rarity = 1;
        if (random >= 95) rarity = 4;
        else if (random >= 85) rarity = 3;
        else if (random >= 60) rarity = 2;
        int fuerza = 1;
        int vida = 1;
        int skill = 1;
        int luck = 1;
        for (var x = 0; x < 5; x++)
        {
            switch (Random.Range(1, 5))
            {
                case 1: fuerza++; break;
                case 2: vida++; break;
                case 3: skill++; break;
                case 4: luck++; break;
                default: luck++; break;
            }
        }

        string skills = GenerateNewSkills();

        newItem.ID_string = newItem.ID.ToString() + rarity.ToString() + vida.ToString() + fuerza.ToString() +
                                 skill.ToString() + luck.ToString() + skills;

        return newItem;
    }

    public Item GetItem(string bigID)
    {
        Item item = new Item() {
            ID = int.Parse(bigID.Substring(0, 3)),
            rarity = int.Parse(bigID.Substring(3, 1)),
            health = int.Parse(bigID.Substring(4, 1)),
            attack = int.Parse(bigID.Substring(5, 1)),
            skill = int.Parse(bigID.Substring(6, 1)),
            luck = int.Parse(bigID.Substring(7, 1)),
            skill_1 = Skills.Instance.GetSkillByID(bigID.Substring(8, 3)),
            skill_2 = Skills.Instance.GetSkillByID(bigID.Substring(11, 3)),
            skill_3 = Skills.Instance.GetSkillByID(bigID.Substring(14, 3))
        };
        item.attack *= item.rarity;
        item.health *= item.rarity;
        item.skill *= item.rarity;
        item.luck *= item.rarity;
        return item;
    }


    public string GetRandomItemID(Equip_Position posi, int rareLvl)
    {
        Equipable_Item newItem = new Equipable_Item();
        switch (posi) //Equip Position
        {
            case Equip_Position.Head: newItem = headgear_list[Random.Range(0, (headgear_list.Count - 1) / 3)]; break;
            case Equip_Position.Body: newItem = bodies_list[Random.Range(0, (bodies_list.Count - 1) / 3)]; break;
            case Equip_Position.Arms: newItem = arms_list[Random.Range(0, (arms_list.Count - 1) / 3)]; break;
            case Equip_Position.Legs: newItem = legs_list[Random.Range(0, (legs_list.Count - 1) / 3)]; break;
        }
        int rarity = Mathf.Clamp(rareLvl, 1, 5);
        int fuerza = 1;
        int vida = 1;
        int skill = 1;
        int luck = 1;
        for (var x = 0; x < 5; x++)
        {
            switch (Random.Range(1, 5))
            {
                case 1: fuerza++; break;
                case 2: vida++; break;
                case 3: skill++; break;
                case 4: luck++; break;
            }
        }

        string skills = GenerateNewSkills();

        newItem.ID_string = newItem.ID.ToString() + rarity.ToString() + vida.ToString() + fuerza.ToString() +
                                 skill.ToString() + luck.ToString() + skills;

        return newItem.ID_string;
    }

    int GetRandomSkillID()
    {
        int skill1ID = 0;
        int skillList = Random.Range(1, 5);
        switch (skillList)
        {
            case 1: skill1ID = Skills.Instance.skill_list_assassin[Random.Range(0, Skills.Instance.skill_list_assassin.Count)].ID; break;
            case 2: skill1ID = Skills.Instance.skill_list_alpha[Random.Range(0, Skills.Instance.skill_list_alpha.Count)].ID; break;
            case 3: skill1ID = Skills.Instance.skill_list_charming[Random.Range(0, Skills.Instance.skill_list_charming.Count)].ID; break;
            case 4: skill1ID = Skills.Instance.skill_list_pacifist[Random.Range(0, Skills.Instance.skill_list_pacifist.Count)].ID; break;
        }
        return skill1ID;
    }
    
}
