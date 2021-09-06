using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Animations : MonoBehaviour {

    public static Animations Instance;
    [HideInInspector]
    public Animator a_player1, a_player2;
    public Animator powerAnim_player1, powerAnim_player2;
    public int p;
    int n = 0;
    public bool blinkingON = false;
    public Coroutine blink_player1, blink_player2;


    [SerializeField]
    public HeadPose pose_player1, pose_player2;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        a_player1 = Menu.Instance.visor_player1.myTransform.Find("Player").GetComponent<Animator>();
        a_player2 = Menu.Instance.visor_player2.myTransform.Find("Player").GetComponent<Animator>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A)) PlayAnim(n++, 1, false);

        //if (Input.GetKeyDown(KeyCode.P)) CerrarOjos(1);
        //if (Input.GetKeyUp(KeyCode.P)) AbrirOjos(1);

    }

    public void EndAnimation()
    {
        print("Ended");
    }

    public void PlayAnim(int c, int playerN, Skill usedSkill)
    {
        Animator anim = playerN == 1 ? a_player1 : a_player2;
        Animator powerAnim = playerN == 1 ? powerAnim_player1 : powerAnim_player2;
        if (usedSkill.s_type == Skill_Type.Spell)
        {
            switch (usedSkill.s_class)
            {
                case Skill_Class.Alpha: powerAnim.Play("Spell_Alpha"); break;
                case Skill_Class.Assassin: powerAnim.Play("Spell_Assassin"); break;
                case Skill_Class.Charming: powerAnim.Play("Spell_Charming"); break;
                case Skill_Class.Pacifist: powerAnim.Play("Spell_Pacifist"); break;
            }
        }
        
        SelectAndPlayerAnim(c, anim);
    }

    public void PlayAnim(int c, int playerN)
    {
        Animator anim = playerN == 1 ? a_player1 : a_player2;
        SelectAndPlayerAnim(c, anim);
    }

    private void SelectAndPlayerAnim(int c, Animator anim)
    {
        switch (c)
        {
            case 0: anim.Play("a2_PhysicalAttackNormal"); break;
            case 1: anim.Play("a2_PhysicalAttackSpecial"); break;
            case 2: anim.Play("a2_MagicalAttackNormal"); break;
            case 3: anim.Play("a2_RecibirDamage"); break;
            case 4: anim.Play("a2_Dizzy"); break;
            case 5: anim.Play("a2_Toxic"); break;
            case 6: anim.Play("a2_Injured"); break;
            case 7: anim.Play("a2_SupportSkill"); break;
            case 8: anim.Play("a2_SupportSkillSpecial"); break;
            case 9: anim.Play("a2_Win"); break;
            case 10: anim.Play("a2_Lose"); break;

            default: n = 0; anim.Play("a2_RecibirDamage"); break;
        }
    }

    IEnumerator AnimationEnded()
    {
        yield return new WaitForSeconds(0.2f);
        //print(a_player1.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        yield return new WaitForSeconds(a_player1.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        if (BattleSystem.Instance) BattleSystem.Instance.EndAnimation();
    }

    public IEnumerator CargarHeadPose(int playerN, int headID, System.Action<bool> onEnded)
    {
        Player player;
        if (!BattleSystem.Instance.battleON)
        {
            player = playerN == 1 ? GameManager.Instance.player : BattleSystem.Instance.player2;
        }
        else
        {
            player = playerN == 1 ? BattleSystem.Instance.player1 : BattleSystem.Instance.player2;
        }

        string spriteID = int.Parse(headID.ToString().Substring(1)) < 10 ? headID.ToString().Substring(2) : headID.ToString().Substring(1);
        string localizador = "/HeadgearPose/" + spriteID + "_";

        Texture2D textureC_C = (Texture2D)Resources.Load("Piezas" + localizador + "C_C");
        Sprite mySpriteC_C = Sprite.Create(textureC_C, new Rect(0f, 0f, textureC_C.width, textureC_C.height), Vector2.zero);
        while (mySpriteC_C == null) yield return new WaitForEndOfFrame();

        //Texture2D textureC_O = (Texture2D)Resources.Load("Piezas" + localizador + "C_O");
        //Sprite mySpriteC_O = Sprite.Create(textureC_O, new Rect(0f, 0f, textureC_O.width, textureC_O.height), Vector2.zero);
        //while (mySpriteC_O == null) yield return new WaitForEndOfFrame();

        //Texture2D textureO_O = (Texture2D)Resources.Load("Piezas" + localizador + "C_O");
        //Sprite mySpriteO_O = Sprite.Create(textureO_O, new Rect(0f, 0f, textureO_O.width, textureO_O.height), Vector2.zero);
        //while (mySpriteO_O == null) yield return new WaitForEndOfFrame();

        Sprite mySpriteO_C;

        yield return Items.Instance.ItemSpriteByID(player.criatura.equipment.head.ID, result => {

            mySpriteO_C = result;
            if (playerN == 1)
            {
                pose_player1 = new HeadPose()
                {
                    openedClosed = mySpriteO_C,
                    closedClosed = mySpriteC_C,
                };
            }
            else
            {
                pose_player2 = new HeadPose()
                {
                    openedClosed = mySpriteO_C,
                    closedClosed = mySpriteC_C,
                };
            }
        });
        onEnded(true);
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator Pestañeo(int playerN)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 6.66f));
            if (blinkingON) CerrarOjos(playerN);
            yield return new WaitForSeconds(Random.Range(0.08f, 0.17f));
            if (blinkingON) AbrirOjos(playerN);
        }
    }

    public void CerrarOjos(int playerN)
    {
        if (playerN == 1) Menu.Instance.visor_player1.headgear.sprite = pose_player1.closedClosed;
        else Menu.Instance.visor_player2.headgear.sprite = pose_player2.closedClosed;
    }

    public void AbrirOjos(int playerN)
    {
        if (playerN == 1) Menu.Instance.visor_player1.headgear.sprite = pose_player1.openedClosed;
        else Menu.Instance.visor_player2.headgear.sprite = pose_player2.openedClosed;
    }


}
