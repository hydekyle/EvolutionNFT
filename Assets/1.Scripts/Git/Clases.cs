using Enums;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace Enums {
    public enum Equip_Position { Head, Body, Arms, Legs, Back, None };
    public enum Quality { Common, Rare, Epic, Legendary };
    public enum Stat { Strenght, Health, Dextery, Luck};
    public enum Skill_Class { Assassin, Pacifist, Charming, Alpha};
    public enum Skill_Type { Attack, Spell, Buff, Heal };
    public enum Speed { Slow, Normal, Fast};
    public enum Buffs { Attack, Skill, Luck, Shield, Barrier, Heal, AutoDamage}
    public enum Debuffs { Bleed, Poison, Dizziness, Confusion, Damage};
    public enum Velocity { Slow, Normal, Fast}
    public enum Evolution { Creature, Tribal, Civilization}
    public enum Chest { None,Cutre1, Cutre2, VIP1, VIP2, VIP3}
}

public class Room
{
    public string guest;
    public string owner;
}

[Serializable]
public class Client
{
    public int elo;
    public long time_tics;
}

public class Item
{
    public int ID, rarity, attack, health, skill, luck;
    public Skill skill_1, skill_2, skill_3;
}

public class BaseStats
{
    public int alpha = 0, assassin = 0, pacifist = 0, charming = 0;
    public int strenght = 0, health = 0, skill = 0, luck = 0;
}

public class StatsWindow
{
    public Text strenght, health, skill, luck,
        alpha, assassin, pacifist, charming;
}

public class WaitingRoom
{
    public string ID;
    public string owner;
    public string guest;
    public string status;
}

public struct BodyBounds
{
    public Vector3 head_POS;
    public Vector3 leg_right_POS;
    public Vector3 leg_left_POS;
    public Vector3 arm_right_POS;
    public Vector3 arm_left_POS;
    public Vector3 back_POS;
    public Vector3 special_scale;
    public Vector3 special_postion;
}

public class FixedScale
{
    public Vector3 customScale;
    public Vector3 customPosition;
    public Vector3 customPosition_right;
    public Vector3 customRotation_right = Vector3.zero;
}

[Serializable]
public class Visor
{
    public Image headgear;
    public Image body;
    public Image back;
    public Image arm_left;
    public Image arm_right;
    public Image leg_left;
    public Image leg_right;
    public Image shadow;
    public Transform myTransform;
}

[Serializable]
public struct GiveStat {
    public Stat stat_type;
    public int value;
}

[SerializeField]
public struct Equipment
{
    public Equipable_Item head;
    public Equipable_Item body;
    public Equipable_Item arms;
    public Equipable_Item legs;
    public Equipable_Item back;
}

[Serializable]
public class MySkylls
{
    public List<int> head = new List<int>();
    public List<int> body = new List<int>();
    public List<int> arms = new List<int>();
    public List<int> legs = new List<int>();
}

[Serializable]
public class SkillsButtons
{
    public SkillButton button1;
    public SkillButton button2;
    public SkillButton button3;
    public SkillButton button4;
    public int button1_skill_ID;
    public int button2_skill_ID;
    public int button3_skill_ID;
    public int button4_skill_ID;
}

[Serializable]
public class SkillButton
{
    public Image myImage;
    public Text myText;
    public Text descriptionText;
    public Image orb;
    public Image icon;
}

[Serializable]
public struct Equipable_Item
{
    public int ID;
    public string ID_string;
    public Quality quality;
}

[Serializable]
public class Criatura
{
    public string nombre;
    public Equipment equipment;
    public MySkylls skills;
    public int attack_att;
    public int defense_att;
    public int skill_att;
    public int luck_att;
}

[Serializable]
public class Player
{
    public string ID;
    public string nombre;
    public Criatura criatura;
    public Stats status;
}

[Serializable]
public class UserDB
{
    public int gold;
    public int gold_VIP;
    public int coronas;
    public string last_time_reward;
    public int elo;
    public int victorias;
    public int derrotas;
    public string nivel;
    public List<int> cofres;
    public List<int> cofres_VIP;
}
[Serializable]
public class HeadPose
{
    public Sprite openedOpened;
    public Sprite openedClosed;

    public Sprite closedClosed;
    public Sprite closedOpened;
}

public class Fecha
{
    public int month;
    public int day;
    public int hour;
    public int min;
}

public class PlayerDB {
    public List<string> items;
    public UserDB data;
    public EquipDB equipamiento;
    public List<string> invitation;
    public string dataTurn;
}

[Serializable]
public class EquipDB
{
    public string head;
    public string body;
    public string arms;
    public string legs;
}

[Serializable]
public class Skill
{
    public int ID;
    public string ID_string;
    public string name_spanish;
    public string name_english;
    public string description_spanish;
    public string description_english;
    public Skill_Class s_class;
    public Skill_Type s_type;
}

[Serializable]
public class DataTurn
{
    public int turn_number;
    public int used_skill;
    public int random_seed;
    public int minigame_fails;
    public string player1;
    public string player2;
}

[Serializable]
public class Stats
{
    public int health_now;
    // Stats
    public int health_base;
    public int attack_base;
    public int skill_base;
    public int luck_base;
    // DeBuffs
    public int bleed;
    public int dizziness;
    public int confusion;
    public int poison;
    // Buffs
    public int buff_attack;
    public int buff_skill;
    public int buff_shield;
    public int buff_barrier;
}

public class Skill_Result : Skill
{
    public int damage;
    public bool isCritic;
    public bool isLucky;

    public int buff_attack;
    public int buff_skill;
    public int buff_shield;
    public int buff_barrier;

    public int bleed;
    public int poison;
    public int dizziness;
    public int confusion;

    public int enemy_buff_attack;
    public int enemy_buff_skill;
   
    public int enemy_buff_shield;
    public int enemy_buff_barrier;

    public int myself_poison;
    public int myself_dizziness;
    public int myself_confusion;
    public int myself_bleed;

    public int myself_damage;
    
    public int recoverHP;
    public int cleans;
}

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost, hCost;
    public MeshRenderer mySprite;
    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}



