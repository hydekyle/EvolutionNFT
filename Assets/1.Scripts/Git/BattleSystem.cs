using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.UI;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames.BasicApi.Multiplayer;
//using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using System.Linq;

public class BattleSystem : MonoBehaviour {

    public static BattleSystem Instance { get; set; }

    public int difficult = 1;

    public Sprite s_bleed, s_poison, s_dizziness, s_confusion, s_empty, s_buffBarrier, s_buffShield, s_buffAttack, s_buffSkill, s_buffLuck, s_buffHeal;
    public Material m_card_1, m_card_2, m_card_3, m_card_4;

    public Canvas canvas_player1, canvas_player2;

    [SerializeField]
    public Player player1, player2;

    [SerializeField]
    public Dictionary<string, object> online_dataTurn;

    #region variables
    Skill_Result lastSkillResult;
    float timeEspera = 4f;
    public int lastActivePlayer;
    bool yourTurn;
    int lastTurnNumber = 0;

    private bool hasWin = false;

    public bool cameraEnded;
    
    RectTransform rect_skills;
    Image fadeScreen;
    [HideInInspector]
    public SkillsButtons skill_buttons;
    int selectedButton = 0, lastSelection = 99;
    Text textoHP_player1, textoHP_player2;

    [HideInInspector]
    public int minigameFails = 0, lastSkill_ID, lastSkillOponent_ID;

    float fadeToAphaValue = 0;

    public int yMax, yMin;
    public float sMin, sMax;
    Vector3 scaleMin, scaleMax;

    Vector3 pos_focus;
    Vector3 pos_noFocusHead, pos_noFocusLegs;

    public Sprite s_button, s_button_red;

    Transform t_hand, t_Focus, t_discard, headT, bodyT, armsT, legsT;

    public Animator battleUI_animator;
    Animator infoAnim, infoAnimExtra;
    Image infoFlotante_image, infoFlotanteExtra_image;
    Text infoFlotante_text, infoFlotanteExtra_text;

    //readonly int tic_camera = 2;

    //Coroutine rutina_camara;

    public bool battleON = false;
    public bool acabando = false;

    
    Slider barra_player1, barra_player2, slider_back_player1, slider_back_player2;
    Transform buff_placement1, buff_placement2;
    Color barColorPlayer1 = Color.green, barColorPlayer2 = Color.green;
    float barBackValuePlayer1 = 1f, barBackValuePlayer2 = 1f;
    Image fillT_player1, fillT_player2;

    readonly int acumulacionesMAX = 9;

    #endregion

    #region Botones_Activables
    public void Head_BTN()
    {
        OnButtonClick(1);
    }

    public void Body_BTN()
    {
        OnButtonClick(2);
    }

    public void Arms_BTN()
    {
        OnButtonClick(3);
    }

    public void Legs_BTN()
    {
        OnButtonClick(4);
    }

    private void OnButtonClick(int n)
    {
        if (n == selectedButton)
        {
            switch (n)
            {
                case 1: LanzarSkill(skill_buttons.button1_skill_ID, 1); break;
                case 2: LanzarSkill(skill_buttons.button2_skill_ID, 2); break;
                case 3: LanzarSkill(skill_buttons.button3_skill_ID, 3); break;
                case 4: LanzarSkill(skill_buttons.button4_skill_ID, 4); break;
            }
            selectedButton = 0;
            lastSelection = n;
            StopCameraMove();
            StartCoroutine(CanvasBase.Instance.C_UsingSkill(player1.ID == GameManager.Instance.GetUserID() ? 1 : 2));                        //CHECK IT
        }
        else
        {
            AfterFirstClick(n);
        }
    }
    #endregion

    #region Engine

    public void Initialize(Player jugador1, Player jugador2)
    {
        if (!battleON)
        {
            Destroy(CanvasBase.Instance.transform.Find("Start_Menu").gameObject);
            Destroy(CanvasBase.Instance.transform.Find("Tienda").gameObject);
            Destroy(EquipMenu.Instance.gameObject);
            Destroy(Cofres_System.Instance.gameObject);
            Message.Instance.SwitchMessagePosition();
            gameObject.SetActive(true);
            scaleMin = new Vector3(sMin, sMin, sMin);
            scaleMax = new Vector3(sMax, sMax, sMax);
            Transform t_habilidades = transform.Find("Canvas").Find("[Habilidades]");
            t_hand = t_habilidades.Find("Hand");
            t_discard = t_habilidades.Find("Discard");
            t_Focus = t_hand.parent.Find("Focus");
            fadeScreen = transform.GetChild(0).Find("FadeScreen").GetComponent<Image>();
            //textoHP_player1 = transform.GetChild(0).Find("HP_player1").GetComponent<Text>();
            //textoHP_player2 = transform.GetChild(0).Find("HP_player2").GetComponent<Text>();
            Transform battleUI = transform.GetChild(0).Find("BattleUI");

            infoFlotante_image = transform.GetChild(1).GetChild(0).Find("InfoFlotante").Find("Image").GetComponent<Image>();
            infoFlotante_text = transform.GetChild(1).GetChild(0).Find("InfoFlotante").Find("Text").GetComponent<Text>();
            infoAnim = infoFlotante_image.transform.parent.GetComponent<Animator>();

            infoFlotanteExtra_image = transform.GetChild(1).GetChild(1).Find("InfoFlotanteExtra").Find("Image").GetComponent<Image>();
            infoFlotanteExtra_text = transform.GetChild(1).GetChild(1).Find("InfoFlotanteExtra").Find("Text").GetComponent<Text>();
            infoAnimExtra = infoFlotanteExtra_image.transform.parent.GetComponent<Animator>();

            barra_player1 = battleUI.Find("LifeBar1").Find("Barra_player1").GetComponent<Slider>();
            barra_player2 = battleUI.Find("LifeBar2").Find("Barra_player2").GetComponent<Slider>();
            buff_placement1 = battleUI.Find("LifeBar1").Find("Buff_Placement");
            buff_placement2 = battleUI.Find("LifeBar2").Find("Buff_Placement");
            slider_back_player1 = barra_player1.transform.parent.Find("Barra_player1_Back").GetComponent<Slider>();
            slider_back_player2 = barra_player2.transform.parent.Find("Barra_player2_Back").GetComponent<Slider>();
            fillT_player1 = barra_player1.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
            fillT_player2 = barra_player2.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
            Transform habilidadesT = transform.Find("Canvas").Find("[Habilidades]");
            headT = habilidadesT.Find("Hand").Find("Skill_Head");
            bodyT = habilidadesT.Find("Hand").Find("Skill_Body");
            armsT = habilidadesT.Find("Hand").Find("Skill_Arms");
            legsT = habilidadesT.Find("Hand").Find("Skill_Legs");
            rect_skills = transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
            rect_skills.localPosition = rect_skills.localPosition + Vector3.down * 50;
            rect_skills.gameObject.SetActive(true);

            pos_noFocusHead = new Vector3(headT.localPosition.x, yMin, 0);
            pos_noFocusLegs = new Vector3(legsT.localPosition.x, yMin, 0);

            #region Skill_Buttons
            skill_buttons = new SkillsButtons()
            {
                button1 = new SkillButton()
                {
                    myImage = headT.GetComponent<Image>(),
                    myText = headT.Find("Text").GetComponent<Text>(),
                    orb = headT.Find("Orb").GetComponent<Image>(),
                    icon = headT.Find("Icon").GetComponent<Image>(),
                    descriptionText = headT.Find("Description").GetComponent<Text>()
                },
                button2 = new SkillButton()
                {
                    myImage = bodyT.GetComponent<Image>(),
                    myText = bodyT.Find("Text").GetComponent<Text>(),
                    orb = bodyT.Find("Orb").GetComponent<Image>(),
                    icon = bodyT.Find("Icon").GetComponent<Image>(),
                    descriptionText = bodyT.Find("Description").GetComponent<Text>()
                },
                button3 = new SkillButton()
                {
                    myImage = armsT.GetComponent<Image>(),
                    myText = armsT.Find("Text").GetComponent<Text>(),
                    orb = armsT.Find("Orb").GetComponent<Image>(),
                    icon = armsT.Find("Icon").GetComponent<Image>(),
                    descriptionText = armsT.Find("Description").GetComponent<Text>()
                },
                button4 = new SkillButton()
                {
                    myImage = legsT.GetComponent<Image>(),
                    myText = legsT.Find("Text").GetComponent<Text>(),
                    orb = legsT.Find("Orb").GetComponent<Image>(),
                    icon = legsT.Find("Icon").GetComponent<Image>(),
                    descriptionText = legsT.Find("Description").GetComponent<Text>()
                }
            };
            #endregion

            player1 = jugador1;
            player2 = jugador2;

            if (player2.ID == "IA")
            {
                int myNivel = int.Parse(GameManager.Instance.userdb.nivel.Substring(0, 2));
                if (myNivel >= 30) BuilderPlayerIA(4);
                else if (myNivel >= 20) BuilderPlayerIA(3);
                else if (myNivel >= 10) BuilderPlayerIA(2);
                else BuilderPlayerIA(1);
            }

            StartCoroutine(GameManager.Instance.MostrarJugador(player1, 1, new Vector3(-243, 15, 0), true)); //Visualizar jugador
            StartCoroutine(GameManager.Instance.MostrarJugador(player2, 2, new Vector3(160, 15, 0), false)); //Visualizar oponente

            StartCoroutine(ColocarBarrasDeSalud());

            LeerStats();
            AudioSystem.Instance.Music_Battle();
            //Resources.UnloadUnusedAssets();

            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                yMax -= 30;
                yMin -= 30;
            } //Position Fixer

            battleON = true;
            StartCoroutine(Animations.Instance.CargarHeadPose(2, jugador2.criatura.equipment.head.ID, onEnded => {
                Animations.Instance.blink_player2 = StartCoroutine(Animations.Instance.Pestañeo(2));
            }));

            //StartTurn();
        }
    }

    private void BuilderPlayerIA(int dificultad)
    {
        print("IA LVL: " + dificultad);
        player2.criatura.equipment.head = Items.Instance.ItemByID(Items.Instance.GetRandomItemID(Equip_Position.Head, dificultad));
        player2.criatura.equipment.body = Items.Instance.ItemByID(Items.Instance.GetRandomItemID(Equip_Position.Body, dificultad));
        player2.criatura.equipment.arms = Items.Instance.ItemByID(Items.Instance.GetRandomItemID(Equip_Position.Arms, dificultad));
        player2.criatura.equipment.legs = Items.Instance.ItemByID(Items.Instance.GetRandomItemID(Equip_Position.Legs, dificultad));
    }

    public void LeerStats() //Lee stats, y habilidades de los jugadores
    {
        player1.criatura.skills = new MySkylls();
        player1.criatura.skills.head.Add(int.Parse(player1.criatura.equipment.head.ID_string.Substring(8, 3)));
        player1.criatura.skills.head.Add(int.Parse(player1.criatura.equipment.head.ID_string.Substring(11, 3)));
        player1.criatura.skills.head.Add(int.Parse(player1.criatura.equipment.head.ID_string.Substring(14, 3)));

        player1.criatura.skills.body.Add(int.Parse(player1.criatura.equipment.body.ID_string.Substring(8, 3)));
        player1.criatura.skills.body.Add(int.Parse(player1.criatura.equipment.body.ID_string.Substring(11, 3)));
        player1.criatura.skills.body.Add(int.Parse(player1.criatura.equipment.body.ID_string.Substring(14, 3)));

        player1.criatura.skills.arms.Add(int.Parse(player1.criatura.equipment.arms.ID_string.Substring(8, 3)));
        player1.criatura.skills.arms.Add(int.Parse(player1.criatura.equipment.arms.ID_string.Substring(11, 3)));
        player1.criatura.skills.arms.Add(int.Parse(player1.criatura.equipment.arms.ID_string.Substring(14, 3)));

        player1.criatura.skills.legs.Add(int.Parse(player1.criatura.equipment.legs.ID_string.Substring(8, 3)));
        player1.criatura.skills.legs.Add(int.Parse(player1.criatura.equipment.legs.ID_string.Substring(11, 3)));
        player1.criatura.skills.legs.Add(int.Parse(player1.criatura.equipment.legs.ID_string.Substring(14, 3)));

        player2.criatura.skills = new MySkylls();
        player2.criatura.skills.head.Add(int.Parse(player2.criatura.equipment.head.ID_string.Substring(8, 3)));
        player2.criatura.skills.head.Add(int.Parse(player2.criatura.equipment.head.ID_string.Substring(11, 3)));
        player2.criatura.skills.head.Add(int.Parse(player2.criatura.equipment.head.ID_string.Substring(14, 3)));

        player2.criatura.skills.body.Add(int.Parse(player2.criatura.equipment.body.ID_string.Substring(8, 3)));
        player2.criatura.skills.body.Add(int.Parse(player2.criatura.equipment.body.ID_string.Substring(11, 3)));
        player2.criatura.skills.body.Add(int.Parse(player2.criatura.equipment.body.ID_string.Substring(14, 3)));

        player2.criatura.skills.arms.Add(int.Parse(player2.criatura.equipment.arms.ID_string.Substring(8, 3)));
        player2.criatura.skills.arms.Add(int.Parse(player2.criatura.equipment.arms.ID_string.Substring(11, 3)));
        player2.criatura.skills.arms.Add(int.Parse(player2.criatura.equipment.arms.ID_string.Substring(14, 3)));

        player2.criatura.skills.legs.Add(int.Parse(player2.criatura.equipment.legs.ID_string.Substring(8, 3)));
        player2.criatura.skills.legs.Add(int.Parse(player2.criatura.equipment.legs.ID_string.Substring(11, 3)));
        player2.criatura.skills.legs.Add(int.Parse(player2.criatura.equipment.legs.ID_string.Substring(14, 3)));

        RenovarDeck();

        BaseStats statsPlayer1 = Items.Instance.CalcularTotalBasePoints(player1.criatura.equipment);
        BaseStats statsPlayer2 = Items.Instance.CalcularTotalBasePoints(player2.criatura.equipment);

        player1.status = new Stats()                                                            //Buildea a los jugadores
        {
            health_base = 150 + statsPlayer1.health * 4,
            health_now =  150 + statsPlayer1.health * 4,
            skill_base = statsPlayer1.skill,
            attack_base = statsPlayer1.strenght,
            luck_base = statsPlayer1.luck,
        };
        player2.status = new Stats()
        {
            health_base = 150 + statsPlayer2.health * 4,
            health_now =  150 + statsPlayer2.health * 4,
            skill_base = statsPlayer2.skill,
            attack_base = statsPlayer2.strenght,
            luck_base = statsPlayer2.luck,
        };

        if (player2.ID == "IA")
        {
            int lvlIA = MatchMaking.Instance.GetLvlIA();
            print("Building IA "+lvlIA);

            int vida_IA = Mathf.Clamp(70 + lvlIA * 10 + statsPlayer2.health * 4, 80, 200);
            int skill_IA = Mathf.Clamp(lvlIA, 1, 11);
            int attack_IA = Mathf.Clamp(lvlIA, 1, 11);
            int luck_IA = Mathf.Clamp(lvlIA, 1, 11);

            player2.status = new Stats()
            {
                health_base = vida_IA,
                health_now  = vida_IA,
                skill_base  = skill_IA,
                attack_base = attack_IA,
                luck_base   = luck_IA
            };
        }
    }

    IEnumerator Waiter(System.Action<bool> onEnded)
    {
        while (player1.ID == "" || player2.ID == "") yield return new WaitForEndOfFrame();
        onEnded(true);
    }

    private int ExpByLvl(int nivel, bool win)
    {
        int exp = 0;
        if (nivel >= 90)      exp = 2;
        else if (nivel >= 80) exp = 4;
        else if (nivel >= 70) exp = 6;
        else if (nivel >= 60) exp = 8;
        else if (nivel >= 50) exp = 10;
        else if (nivel >= 40) exp = 12;
        else if (nivel >= 30) exp = 16;
        else if (nivel >= 20) exp = 24;
        else if (nivel >= 10) exp = 32;
        else                  exp = 60;

        if (!win) exp /= 2;

        return exp;
    }

    public string CalcularPuntosVictoria(string nivelData, bool victoria)
    {
        int nivel = int.Parse(nivelData.Substring(0, 2));
        int myExp = int.Parse(nivelData.Substring(2, 2));

        int exp = ExpByLvl(nivel, victoria);

        if (myExp + exp >= 100)
        {
            myExp = (myExp + exp) - 100;
            nivel++;
        }
        else
        {
            myExp = myExp + exp;
        }

        string resultado = "";
        resultado = nivel < 10 ? "0" + nivel : nivel.ToString();
        resultado += myExp < 10 ? "0" + myExp : myExp.ToString();

        return resultado;
    }

    public List<Equipable_Item> ObtenerListaEquipamiento(Player player)
    {
        List<Equipable_Item> lista = new List<Equipable_Item>() {
            player.criatura.equipment.head,
            player.criatura.equipment.body,
            player.criatura.equipment.arms,
            player.criatura.equipment.legs,
            player.criatura.equipment.back
        };

        return lista;
    }

    private void AfterFirstClick(int n)
    {
        lastSelection = selectedButton;
        selectedButton = n;
        PutCardOnFocus(n);
    }

    public List<int> deck = new List<int>();

    int h = 0;

    public void RenovarDeck()
    {
        List<int> nDeck = new List<int>();
        foreach (int i in MySelf().criatura.skills.head) nDeck.Add(i);
        foreach (int i in MySelf().criatura.skills.body) nDeck.Add(i);
        foreach (int i in MySelf().criatura.skills.arms) nDeck.Add(i);
        foreach (int i in MySelf().criatura.skills.legs) nDeck.Add(i);
        deck = nDeck.OrderBy(x => Random.value).ToList();
    }

    private int NextCardN()
    {
        int skillID = 0;
        if (h < 0)
        {
            skillID = deck[deck.Count - Mathf.Abs(h++)];
        }
        else
        {
            try
            {
                skillID = deck[h++];
            }
            catch
            {
                h = 0;
                skillID = deck[h++];
            }
        }
        return skillID;
    }
    
    public void UpdateSkillButtons()
    {
        Player player = MySelf();
        skill_buttons.button1_skill_ID = NextCardN();
        skill_buttons.button2_skill_ID = NextCardN();
        skill_buttons.button3_skill_ID = NextCardN();
        skill_buttons.button4_skill_ID = NextCardN();

        skill_buttons.button1.myText.text = Lenguaje.Instance.SkillNameByID(skill_buttons.button1_skill_ID);
        skill_buttons.button1.descriptionText.text = Lenguaje.Instance.SkillDescriptionByID(skill_buttons.button1_skill_ID);
        skill_buttons.button1.orb.sprite = Items.Instance.SphereBySkillID(skill_buttons.button1_skill_ID);
        skill_buttons.button1.icon.sprite = Items.Instance.IconBySkillID(skill_buttons.button1_skill_ID);


        skill_buttons.button2.myText.text = Lenguaje.Instance.SkillNameByID(skill_buttons.button2_skill_ID);
        skill_buttons.button2.descriptionText.text = Lenguaje.Instance.SkillDescriptionByID(skill_buttons.button2_skill_ID);
        skill_buttons.button2.orb.sprite = Items.Instance.SphereBySkillID(skill_buttons.button2_skill_ID);
        skill_buttons.button2.icon.sprite = Items.Instance.IconBySkillID(skill_buttons.button2_skill_ID);

        skill_buttons.button3.myText.text = Lenguaje.Instance.SkillNameByID(skill_buttons.button3_skill_ID);
        skill_buttons.button3.descriptionText.text = Lenguaje.Instance.SkillDescriptionByID(skill_buttons.button3_skill_ID);
        skill_buttons.button3.orb.sprite = Items.Instance.SphereBySkillID(skill_buttons.button3_skill_ID);
        skill_buttons.button3.icon.sprite = Items.Instance.IconBySkillID(skill_buttons.button3_skill_ID);

        skill_buttons.button4.myText.text = Lenguaje.Instance.SkillNameByID(skill_buttons.button4_skill_ID);
        skill_buttons.button4.descriptionText.text = Lenguaje.Instance.SkillDescriptionByID(skill_buttons.button4_skill_ID);
        skill_buttons.button4.orb.sprite = Items.Instance.SphereBySkillID(skill_buttons.button4_skill_ID);
        skill_buttons.button4.icon.sprite = Items.Instance.IconBySkillID(skill_buttons.button4_skill_ID);


    }  //Refrescar mis habilidades

    public void NewDataTurnReceived(Dictionary<string, object> newDataTurn)
    {
        print("New Data Turn");
        NewTurn(newDataTurn);
    }

    IEnumerator TurnIA()
    {
        while (!battleON) yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        Database.Instance.ReferenceDataTurn().GetValueAsync().ContinueWith(task =>
        {
            Dictionary<string, object> dic = task.Result.Value as Dictionary<string, object>;
            int usedSkill = 101;
            switch (Random.Range(1, 5))
            {
                case 1: usedSkill = player2.criatura.skills.head[Random.Range(0, 3)]; break;
                case 2: usedSkill = player2.criatura.skills.body[Random.Range(0, 3)]; break;
                case 3: usedSkill = player2.criatura.skills.arms[Random.Range(0, 3)]; break;
                case 4: usedSkill = player2.criatura.skills.legs[Random.Range(0, 3)]; break;
                default: usedSkill = 101; break;
            }

            int failsIA = 0;
            int random1 = Mathf.Clamp(player2.status.confusion + player2.status.dizziness, 0, 15);

            if (player2.status.health_now > player1.status.health_now) failsIA++; // Si va ganando, un fail.
            if (Random.Range(random1, 15) > 8) failsIA++;                         // Aleatorio según conf y dizz
            if (Random.Range(0, 4) == 3) failsIA++;                               // 1/4 de fallar

            DataTurn newDataTurn = new DataTurn()
            {
                player1 = (string)dic["player1"],
                player2 = (string)dic["player2"],
                turn_number = int.Parse(dic["turn_number"].ToString()) + 1,
                minigame_fails = Random.Range(0, player2.status.dizziness),
                random_seed = Random.Range(1, 100),
                used_skill = usedSkill
            };
            Database.Instance.ReferenceDataTurn().SetRawJsonValueAsync(JsonUtility.ToJson(newDataTurn));
        });
    }

    void FadeAlpha(float alphaValue)
    {
        fadeToAphaValue = alphaValue;
    }

    void LanzarSkill(int ID_skill, int nSlot)
    {
        if (yourTurn)
        {
            yourTurn = false;
            FadeAlpha(0.5f);
            difficult = MyDifficultLvl();
            difficult = Mathf.Clamp(difficult, 1, 10);
            switch (Skills.Instance.SkillClassByID(ID_skill))
            {
                case Skill_Class.Assassin: OsuSystem.Instance.Bolas(); break;
                case Skill_Class.Alpha: TapFast.Instance.Iniciar(); break;
                case Skill_Class.Charming: AccuracySystem.Instance.Iniciar(); break;
                case Skill_Class.Pacifist: SpellSystem.Instance.Iniciar(); break;
                default: OsuSystem.Instance.Bolas(); break;
            }
            lastSkill_ID = ID_skill;
            h -= (4 - nSlot);
        }
    }

    private int MyDifficultLvl()
    {
        Player player = MyPlayerN() == 1 ? player1 : player2;
        int dificultad = 0;
        dificultad += Mathf.Clamp(player.status.dizziness, 0, 6);
        for(var x = 0; x < player.status.confusion; x++)
        {
            dificultad += Random.Range(0, 2) > 0 ? 1 : 0;
        }


        return dificultad;
    }

    public void EndMinigame()
    {
        if (battleON)
        {
            FadeAlpha(0);
            RefrescarBuffs(MyPlayerN() == 1 ? player1 : player2);
            Skill_Result result = Skills.Instance.SkillResolve(lastSkill_ID, MySelf(), YourEnemy(), GetRandomSeed(MySelf().status.luck_base), minigameFails); 
            ApplyResult(result, false);
            PutCardOnFocus(0);
            lastActivePlayer = player1.ID == GameManager.Instance.GetUserID() ? 1 : 2;
            PutPlayerOnTop(lastActivePlayer);
            Animate(lastSkill_ID);
            AcabarYActualizarTurno();
            UpdateBuffsUI(MyPlayerN());
        }
    }

    int GetRandomSeed(int luckStat)
    {
        return Random.Range(luckStat, 101);
    }

    public void ResolverTurnoEnemigo(int usedSkill, int eRandom, int eFails)
    {
        if (usedSkill > 0 && battleON)
        {
            Skill_Result result = Skills.Instance.SkillResolve(usedSkill, YourEnemy(), MySelf(), eRandom, eFails); //RESOLVER SKILL
            ApplyResult(result, true);
            lastActivePlayer = OpponentPlayerN();
            PutPlayerOnTop(lastActivePlayer);
            Animate(usedSkill);
            StopCameraMove();
            StartCoroutine(CanvasBase.Instance.C_UsingSkill(player1.ID == GameManager.Instance.GetUserID() ? 2 : 1));
            Message.Instance.NewMessage("El enemigo usa " + Skills.Instance.SkillByID(usedSkill).name_spanish);
        }
    }

    public void EndAnimation()
    {
        //if (!autoCamera)
        //{
        //    cameraEnded = true;
        //    autoCamera = true;
        //}
    }

    public void StopCameraMove()
    {
        //autoCamera = false;
        //StopCoroutine(rutina_camara);
    }

    void Animate(int skillID)
    {
        Skill usedSkill = Skills.Instance.SkillByID(skillID);
        switch (usedSkill.s_type)
        {
            case Skill_Type.Attack: Animations.Instance.PlayAnim(0, lastActivePlayer, usedSkill); break;
            case Skill_Type.Spell: Animations.Instance.PlayAnim(2, lastActivePlayer, usedSkill); break;
            case Skill_Type.Buff: Animations.Instance.PlayAnim(7, lastActivePlayer, usedSkill); break;
            case Skill_Type.Heal: Animations.Instance.PlayAnim(7, lastActivePlayer, usedSkill); break;
        }
    }

    public void HitEnemyAnim()
    {
        StartCoroutine(RutinaEstados(OppositeActivePlayerN(lastActivePlayer)));

        if(lastSkillResult.damage > 0)
        {
            if (lastActivePlayer == 2) battleUI_animator.Play("Hit_Barra1");
            else battleUI_animator.Play("Hit_Barra2");
        }
        
    }

    void UpdateBarraDeSalud(int playerN)
    {
        Player player = playerN == 1 ? player1 : player2;
        Slider player_slider = playerN == 1 ? barra_player1 : barra_player2;
        Slider player_slider_back = playerN == 1 ? slider_back_player1 : slider_back_player2;
        int maxHP = player.status.health_base;
        int nowHP = player.status.health_now;

        float newBarValue = (nowHP * 100 / maxHP) / 100f;

        player_slider.value = newBarValue;

        float newBarValue_back = newBarValue;
        Mathf.Clamp(newBarValue_back, 0.0f, 1.0f);

        if (playerN == 1) barBackValuePlayer1 = newBarValue_back;
        else barBackValuePlayer2 = newBarValue_back;

        if (playerN == 1) barColorPlayer1 = new Color(0, 1, 0, newBarValue);
        else barColorPlayer2 = new Color(0, 1, 0, newBarValue);

        if (nowHP < 1) //¡Has muerto!
        {
            player_slider.transform.parent.Find("Buff_Placement").gameObject.SetActive(false);
            battleUI_animator.SetBool("Muerto", true);
            if (playerN == 1) battleUI_animator.Play("Reventar_Barra1");
                         else battleUI_animator.Play("Reventar_Barra2");
            if (playerN == MyPlayerN()) PerderPartida();
                                   else GanarPartida();
        }
    }

    public void AcabarYActualizarTurno()
    {
        Dictionary<string, object> actualDataTurn = online_dataTurn;
        actualDataTurn["turn_number"] = int.Parse(actualDataTurn["turn_number"].ToString()) + 1;
        actualDataTurn["used_skill"] = lastSkill_ID;
        actualDataTurn["minigame_fails"] = minigameFails;
        Database.Instance.ReferenceDataTurn().SetValueAsync(actualDataTurn).ContinueWith(task => {
            minigameFails = 0;
            StartCoroutine(BeginningPhase(OpponentPlayerN(), onEnded => {
                if (onEnded)
                {
                    print("ACABO TURNO");
                    if (player2.ID == "IA") StartCoroutine(TurnIA());

                }
                else
                {
                    print("El enemigo la penca");
                }
            }));
        });
    }

    public Player MySelf()
    {
        return player1.ID == GameManager.Instance.GetUserID() ? player1 : player2;
    }

    public Player YourEnemy()
    {
        return player1.ID == GameManager.Instance.GetUserID() ? player2 : player1;
    }

    int MyPlayerN()
    {
        return player1.ID == GameManager.Instance.GetUserID() ? 1 : 2;
    }

    int OpponentPlayerN()
    {
        return player1.ID == GameManager.Instance.GetUserID() ? 2 : 1;
    }

    int OppositeActivePlayerN(int actualPlayer)
    {
        return actualPlayer == 1 ? 2 : 1;
    }

    #endregion

    void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (battleON)
        {
            fillT_player1.color = Color.Lerp(fillT_player1.color, barColorPlayer1, Time.deltaTime * 5);
            fillT_player2.color = Color.Lerp(fillT_player2.color, barColorPlayer2, Time.deltaTime * 5);
            slider_back_player1.value = Mathf.Lerp(slider_back_player1.value, barBackValuePlayer1, Time.deltaTime * 5);
            slider_back_player2.value = Mathf.Lerp(slider_back_player2.value, barBackValuePlayer2, Time.deltaTime * 5);
            RectPositions();
        }
        //if (Input.GetKeyDown(KeyCode.Alpha1)) InfoFlotante(Debuffs.Bleed);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) InfoFlotante(Debuffs.Poison);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) InfoFlotante(Debuffs.Dizziness);
        //if (Input.GetKeyDown(KeyCode.Alpha4)) InfoFlotante(Debuffs.Confusion);

    }

    void StartTurn()
    {
        StartCoroutine(BeginningPhase(MyPlayerN(), onEnded => {
            if (onEnded)
            {
                print("COMIENZA MI TURNO");
                PutCardOnFocus(0);
                UpdateSkillButtons();
                //Message.Instance.NewMessage(Lenguaje.Instance.Text_YourTurn());
                yourTurn = true;
            }
        }));
    }

    private void FixEspera()
    {
        if (lastSkillResult != null)
        {
            print(lastSkillResult.s_type);
            switch (lastSkillResult.s_type)
            {
                case Skill_Type.Attack: timeEspera = 1.5f; break;
                case Skill_Type.Spell: timeEspera = 3.4f; break;
                default: timeEspera = 4f; break;
            }

            if (lastSkillResult.damage > 0) timeEspera += 1f;
            if (lastSkillResult.poison > 0) timeEspera += 1f;
            if (lastSkillResult.bleed > 0) timeEspera += 1.1f;
            if (lastSkillResult.dizziness > 0) timeEspera += 1f;
            if (lastSkillResult.confusion > 0) timeEspera += 1f;

        }
    }

    IEnumerator BeginningPhase(int playerN, System.Action<bool> onEnded)
    {
        while (!battleON) yield return new WaitForSeconds(1f);
        FixEspera();
        Player activePlayer = playerN == 1 ? player1 : player2;
        if (!acabando)
        {
            yield return new WaitForSeconds(timeEspera);
            StartCoroutine(CanvasBase.Instance.C_UsingSkill(playerN));
            if (activePlayer.status.bleed > 0 && activePlayer.status.health_now > 0)
            {
                yield return new WaitForSeconds(0.5f);
                int damageBleed = activePlayer.status.bleed * 8;
                InfoFlotante(Debuffs.Bleed, damageBleed);
                Animations.Instance.PlayAnim(6, playerN);
                activePlayer.status.health_now -= damageBleed;
                UpdateBarraDeSalud(playerN);
                yield return new WaitForSeconds(1f);
            }
            if (activePlayer.status.poison > 0 && activePlayer.status.health_now > 0)
            {
                int damagePoison = activePlayer.status.poison * 6;
                InfoFlotante(Debuffs.Poison, damagePoison);
                Animations.Instance.PlayAnim(5, playerN);
                activePlayer.status.health_now -= damagePoison;
                UpdateBarraDeSalud(playerN);
                yield return new WaitForSeconds(1f);
            }

            if (activePlayer.status.health_now >= 1)
            {
                onEnded(true);
            }
            else onEnded(false);
        }
        yield return null;
    }

    public void NewTurn(Dictionary<string, object> newTurnData)
    {
        if (int.Parse(newTurnData["turn_number"].ToString()) != lastTurnNumber) //Comprueba que el turno ha cambiado
        {
            lastSkill_ID = int.Parse(newTurnData["used_skill"].ToString());
            lastTurnNumber = int.Parse(newTurnData["turn_number"].ToString());
            online_dataTurn = newTurnData;
            StartCoroutine(Waiter(onEnded => {
                if (int.Parse(newTurnData["turn_number"].ToString()) % 2 != 0)
                {
                    if (player1.ID == GameManager.Instance.GetUserID())
                    {
                        RefrescarBuffs(player2);
                        ResolverTurnoEnemigo
                        (
                            int.Parse(newTurnData["used_skill"].ToString()),
                            int.Parse(newTurnData["random_seed"].ToString()),
                            int.Parse(newTurnData["minigame_fails"].ToString())
                        );
                        StartTurn();
                    }
                    else
                    {
                        //Message.Instance.NewMessage("Turno enemigo");
                    }
                }
                else
                {
                    if (player2.ID == GameManager.Instance.GetUserID())
                    {
                        RefrescarBuffs(player1);
                        ResolverTurnoEnemigo
                        (
                            int.Parse(newTurnData["used_skill"].ToString()),
                            int.Parse(newTurnData["random_seed"].ToString()),
                            int.Parse(newTurnData["minigame_fails"].ToString())
                        );
                        StartTurn();
                    }
                    else
                    {
                        if (player2.ID == "IA" && newTurnData["used_skill"].ToString() == "0") StartCoroutine(TurnIA());
                    }
                }
            }));
        }
    }

    void RefrescarBuffs(Player activePlayer)
    {
        activePlayer.status.bleed--;
        activePlayer.status.poison--;
        activePlayer.status.dizziness--;
        activePlayer.status.buff_attack--;
        activePlayer.status.buff_skill--;
        activePlayer.status.confusion--;
        activePlayer.status.buff_barrier--;
        activePlayer.status.buff_shield--;
        ClampValues();
        UpdateBuffsUI(1);
        UpdateBuffsUI(2);
    }

    private void ClampValues()
    {
        player1.status.health_now = Mathf.Clamp(player1.status.health_now, 0, player1.status.health_base);
        player1.status.dizziness = Mathf.Clamp(player1.status.dizziness, 0, acumulacionesMAX);
        player1.status.bleed = Mathf.Clamp(player1.status.bleed, 0, acumulacionesMAX);
        player1.status.poison = Mathf.Clamp(player1.status.poison, 0, acumulacionesMAX);
        player1.status.confusion = Mathf.Clamp(player1.status.confusion, 0, acumulacionesMAX);
        player1.status.buff_attack = Mathf.Clamp(player1.status.buff_attack, 0, acumulacionesMAX);
        player1.status.buff_skill = Mathf.Clamp(player1.status.buff_skill, 0, acumulacionesMAX);
        player1.status.buff_shield = Mathf.Clamp(player1.status.buff_shield, 0, acumulacionesMAX);
        player1.status.buff_barrier = Mathf.Clamp(player1.status.buff_barrier, 0, acumulacionesMAX);


        player2.status.health_now = Mathf.Clamp(player2.status.health_now, 0, player2.status.health_base);
        player2.status.dizziness = Mathf.Clamp(player2.status.dizziness, 0, acumulacionesMAX);
        player2.status.bleed = Mathf.Clamp(player2.status.bleed, 0, acumulacionesMAX);
        player2.status.poison = Mathf.Clamp(player2.status.poison, 0, acumulacionesMAX);
        player2.status.confusion = Mathf.Clamp(player2.status.confusion, 0, acumulacionesMAX);
        player2.status.buff_attack = Mathf.Clamp(player2.status.buff_attack, 0, acumulacionesMAX);
        player2.status.buff_skill = Mathf.Clamp(player2.status.buff_skill, 0, acumulacionesMAX);
        player2.status.buff_shield = Mathf.Clamp(player2.status.buff_shield, 0, acumulacionesMAX);
        player2.status.buff_barrier = Mathf.Clamp(player2.status.buff_barrier, 0, acumulacionesMAX);
    }

    public void InfoFlotante(Debuffs debuff)
    {
        infoAnim.StopPlayback();
        infoAnim.transform.parent.position = Menu.Instance.GetPlayerVisor(OppositeActivePlayerN(lastActivePlayer)).myTransform.position + Vector3.forward;
        string s = "";
        switch (debuff)
        {
            case Debuffs.Bleed:
                infoFlotante_image.sprite = s_bleed;
                for (var x = 0; x < lastSkillResult.bleed; x++) s += "+";
                infoFlotante_text.text = s + " Sangrado";
                infoFlotante_text.color = Color.red;
                break;
            case Debuffs.Poison:
                infoFlotante_image.sprite = s_poison;
                for (var x = 0; x < lastSkillResult.poison; x++) s += "+";
                infoFlotante_text.text = s + " Veneno";
                infoFlotante_text.color = Color.green;
                break;
            case Debuffs.Dizziness:
                infoFlotante_image.sprite = s_dizziness;
                for (var x = 0; x < lastSkillResult.dizziness; x++) s += "+";
                infoFlotante_text.text = s + " Mareo";
                infoFlotante_text.color = Color.yellow;
                break;
            case Debuffs.Confusion:
                infoFlotante_image.sprite = s_confusion;
                for (var x = 0; x < lastSkillResult.confusion; x++) s += "+";
                infoFlotante_text.text = s + " Confusión";
                infoFlotante_text.color = Color.blue;
                break;
        }
        infoAnim.Play("TextoFlotante2", -1, 0f);

    }

    public void InfoFlotante(Debuffs debuff, int damage)
    {
        infoAnim.StopPlayback();
        infoAnim.transform.parent.position = Menu.Instance.GetPlayerVisor(OppositeActivePlayerN(lastActivePlayer)).myTransform.position + Vector3.forward;
        infoFlotante_image.color = Color.white;
        switch (debuff)
        {
            case Debuffs.Bleed:
                infoFlotante_image.sprite = s_bleed;
                infoFlotante_text.text = "- "+damage.ToString();
                infoFlotante_text.color = Color.red;
                break;
            case Debuffs.Poison:
                infoFlotante_image.sprite = s_poison;
                infoFlotante_text.text = "- " + damage.ToString();
                infoFlotante_text.color = Color.green;
                break;
            case Debuffs.Dizziness:
                infoFlotante_image.sprite = s_dizziness;
                infoFlotante_text.text = "- " + damage.ToString();
                infoFlotante_text.color = Color.yellow;
                break;
            case Debuffs.Confusion:
                infoFlotante_image.sprite = s_confusion;
                infoFlotante_text.text = "- " + damage.ToString();
                infoFlotante_text.color = Color.blue;
                break;
            default:
                infoFlotante_image.sprite = s_empty;
                infoFlotante_text.text = "- " + damage.ToString();
                infoFlotante_text.color = Color.yellow;
                break;

        }
        infoAnim.Play("TextoFlotante2", -1, 0f);
    }

    public void InfoExtraBuff(Buffs buffs)
    {
        infoAnimExtra.StopPlayback();
        infoAnimExtra.transform.parent.position = Menu.Instance.GetPlayerVisor(lastActivePlayer).myTransform.position + Vector3.forward;
        string s = "";
        infoFlotanteExtra_image.color = Color.white;
        switch (buffs)
        {
            case Buffs.Barrier:
                infoFlotanteExtra_image.sprite = s_buffShield;
                for (var x = 0; x < lastSkillResult.buff_barrier; x++) s += "+";
                infoFlotanteExtra_text.text = s;
                infoFlotanteExtra_text.color = Color.green;
                break;

            case Buffs.Shield:
                infoFlotanteExtra_image.sprite = s_buffShield;
                for (var x = 0; x < lastSkillResult.buff_shield; x++) s += "+";
                infoFlotanteExtra_text.text = s;
                infoFlotanteExtra_text.color = Color.green;
                break;

            case Buffs.Attack:
                infoFlotanteExtra_image.sprite = s_buffAttack;
                for (var x = 0; x < lastSkillResult.buff_attack; x++) s += "+";
                infoFlotanteExtra_text.text = s;
                infoFlotanteExtra_text.color = Color.green;
                break;

            case Buffs.Skill:
                infoFlotanteExtra_image.sprite = s_buffSkill;
                for (var x = 0; x < lastSkillResult.buff_skill; x++) s += "+";
                infoFlotanteExtra_text.text = s;
                infoFlotanteExtra_text.color = Color.green;
                break;

            case Buffs.AutoDamage:
                infoFlotanteExtra_image.sprite = s_empty;
                infoFlotanteExtra_text.text = "- " + lastSkillResult.myself_damage.ToString();
                infoFlotanteExtra_text.color = Color.red;
                break;

            case Buffs.Heal:
                infoFlotanteExtra_image.sprite = s_buffHeal;
                //infoFlotanteExtra_image.color = Color.green;
                infoFlotanteExtra_text.text = lastSkillResult.recoverHP.ToString();
                infoFlotanteExtra_text.color = Color.green;
                break;

            case Buffs.Luck:
                infoFlotanteExtra_image.sprite = s_buffLuck;
                infoFlotanteExtra_image.color = Color.green;
                infoFlotanteExtra_text.text = "¡Lucky!";
                infoFlotanteExtra_text.color = Color.green;
                break;
        }
        infoAnimExtra.Play("TextoFlotante2", -1, 0f);
    }

    public void InfoExtraDebuff(Debuffs debuff)
    {
        infoAnimExtra.StopPlayback();
        infoAnimExtra.transform.parent.position = Menu.Instance.GetPlayerVisor(lastActivePlayer).myTransform.position + Vector3.forward;
        string s = "";
        infoFlotanteExtra_image.color = Color.white;
        switch (debuff)
        {
            case Debuffs.Bleed:
                infoFlotanteExtra_image.sprite = s_bleed;
                for (var x = 0; x < lastSkillResult.myself_bleed; x++) s += "+";
                infoFlotanteExtra_text.text = s + " Sangrado";
                infoFlotanteExtra_text.color = Color.red;
                break;
            case Debuffs.Poison:
                infoFlotanteExtra_image.sprite = s_poison;
                for (var x = 0; x < lastSkillResult.myself_poison; x++) s += "+";
                infoFlotanteExtra_text.text = s + " Veneno";
                infoFlotanteExtra_text.color = Color.green;
                break;
            case Debuffs.Dizziness:
                infoFlotanteExtra_image.sprite = s_dizziness;
                for (var x = 0; x < lastSkillResult.myself_dizziness; x++) s += "+";
                infoFlotanteExtra_text.text = s + " Mareo";
                infoFlotanteExtra_text.color = Color.yellow;
                break;
            case Debuffs.Confusion:
                infoFlotanteExtra_image.sprite = s_confusion;
                for (var x = 0; x < lastSkillResult.myself_confusion; x++) s += "+";
                infoFlotanteExtra_text.text = s + " Confusión";
                infoFlotanteExtra_text.color = Color.blue;
                break;
        }
        infoAnimExtra.Play("TextoFlotante2", -1, 0f);

    }

    void ApplyResult(Skill_Result result, bool inverse)
    {
        lastSkillResult = result;
        Player myself;
        Player enemigo;

        if (inverse)
        {
            myself = YourEnemy();
            enemigo = MySelf();
        }
        else
        {
            myself = MySelf();
            enemigo = YourEnemy();
        }
        //Procesado del caster
        myself.status.health_now -= result.myself_damage;
        myself.status.health_now += result.recoverHP;

        myself.status.bleed += result.myself_bleed;
        myself.status.dizziness += result.myself_dizziness;
        myself.status.poison += result.myself_poison;
        myself.status.confusion += result.myself_confusion;

        myself.status.buff_attack += result.buff_attack;
        myself.status.buff_skill += result.buff_skill;
        myself.status.buff_shield += result.buff_shield;
        myself.status.buff_barrier += result.buff_barrier;

        //Procesado del oponente
        enemigo.status.health_now -= result.damage;

        enemigo.status.bleed += result.bleed;
        enemigo.status.dizziness += result.dizziness;
        enemigo.status.poison += result.poison;
        enemigo.status.confusion += result.confusion;

        enemigo.status.buff_attack += result.enemy_buff_attack;
        enemigo.status.buff_skill += result.enemy_buff_skill;
        enemigo.status.buff_shield += result.enemy_buff_shield;
        enemigo.status.buff_barrier += result.enemy_buff_barrier;

        //Clampear valores y actualizar barras
        ClampValues();
    }

    void UpdateBuffsUI(int playerN)
    {
        Player player = playerN == 1 ? player1 : player2;
        Transform buffT = playerN == 1 ? buff_placement1 : buff_placement2;
        List<Debuffs> debuffs = new List<Debuffs>();
        if (player.status.dizziness > 0) debuffs.Add(Debuffs.Dizziness);
        if (player.status.confusion > 0) debuffs.Add(Debuffs.Confusion);
        if (player.status.bleed > 0) debuffs.Add(Debuffs.Bleed);
        if (player.status.poison > 0) debuffs.Add(Debuffs.Poison);
        List<Buffs> buffs = new List<Buffs>();
        if (player.status.buff_attack > 0) buffs.Add(Buffs.Attack);
        if (player.status.buff_skill > 0) buffs.Add(Buffs.Skill);
        if (player.status.buff_shield > 0) buffs.Add(Buffs.Shield);
        if (player.status.buff_barrier > 0) buffs.Add(Buffs.Barrier);

        int n = 0;

        foreach (Buffs b in buffs)
        {
            Transform t = buffT.GetChild(n);
            t.GetComponent<Image>().sprite = ImageByBuff(b);
            t.GetChild(0).GetComponent<Text>().text = BuffAmount(b, playerN).ToString();
            n++;
        }

        foreach (Debuffs d in debuffs)
        {
            Transform t = buffT.GetChild(n);
            t.GetComponent<Image>().sprite = ImageByDebuff(d);
            t.GetChild(0).GetComponent<Text>().text = DebuffAmount(d, playerN).ToString();
            n++;
        }

        for (var c = n; c < 8; c++)
        {
            Transform t = buffT.GetChild(c);
            t.GetComponent<Image>().sprite = s_empty;
            t.GetChild(0).GetComponent<Text>().text = "";
        }
    }

    private int BuffAmount(Buffs buff, int playerN)
    {
        Player player = playerN == 1 ? player1 : player2;
        switch (buff)
        {
            case Buffs.Attack: return player.status.buff_attack;
            case Buffs.Skill: return player.status.buff_skill;
            case Buffs.Shield: return player.status.buff_shield;
            default: return player.status.buff_barrier;
        }
    }

    private int DebuffAmount(Debuffs debuff, int playerN)
    {
        Player player = playerN == 1 ? player1 : player2;
        switch (debuff)
        {
            case Debuffs.Bleed: return player.status.bleed;
            case Debuffs.Confusion: return player.status.confusion;
            case Debuffs.Dizziness: return player.status.dizziness;
            default: return player.status.poison;
        }
    }

    private Sprite ImageByBuff(Buffs buff)
    {
        switch (buff)
        {
            case Buffs.Attack: return s_buffAttack;
            case Buffs.Skill: return s_buffSkill;
            case Buffs.Barrier: return s_buffBarrier;
            default: return s_buffShield;
        }
    }

    private Sprite ImageByDebuff(Debuffs debuff)
    {
        switch (debuff)
        {
            case Debuffs.Bleed: return s_bleed;
            case Debuffs.Confusion: return s_confusion;
            case Debuffs.Dizziness: return s_dizziness;
            default: return s_poison;
        }
    }

    public void DeadAnimEnded()
    {
        StartCoroutine(ShowAfterBattleWindow());
    }

    private void GanarPartida()
    {
        if (!acabando)
        {
            acabando = true;
            hasWin = true;
            rect_skills.gameObject.SetActive(false);
            StartCoroutine(DeadAnim());
        }
    }

    private void PerderPartida()
    {
        if (!acabando)
        {
            acabando = true;
            rect_skills.gameObject.SetActive(false);
            StartCoroutine(DeadAnim());
        }
    }

    private IEnumerator DeadAnim()
    {
        StopCoroutine(Animations.Instance.blink_player1);
        StopCoroutine(Animations.Instance.blink_player2);
        yield return new WaitForSeconds(0.5f);
        Animations.Instance.CerrarOjos(hasWin ? OpponentPlayerN() : MyPlayerN());
        Animations.Instance.AbrirOjos(!hasWin ? OpponentPlayerN() : MyPlayerN());
        Animations.Instance.PlayAnim(10, hasWin ? OpponentPlayerN() : MyPlayerN());
    }

    private IEnumerator ShowAfterBattleWindow()
    {
        yield return new WaitForSeconds(1f);
        if (hasWin) Animations.Instance.PlayAnim(9, MyPlayerN());
        CanvasBase.Instance.SetCamTargetSpecial(CanvasBase.Instance.Posi(Equip_Position.Body, MyPlayerN()));
        UserDB dataPlayer = GameManager.Instance.userdb;

        int nivel = int.Parse(dataPlayer.nivel.Substring(0, 2));
        int expNow = int.Parse(dataPlayer.nivel.Substring(2, 2));
        int expAdquired = ExpByLvl(int.Parse(dataPlayer.nivel.Substring(0, 2)), hasWin);

        Image back_img = transform.Find("Canvas").Find("Back_Panel").GetComponent<Image>();
        Transform after_battle = transform.Find("Canvas").Find("AfterBattle");
        Transform big_window = after_battle.Find("BigWindow");
        Transform letrero = after_battle.Find("Letrero");
        Slider actualExp_slider = big_window.Find("Bar_level").Find("ActualExp").GetComponent<Slider>();
        Slider newExp_slider = big_window.Find("Bar_level").Find("NewExp").GetComponent<Slider>();
        Text txtLvl = big_window.Find("Text_Lvl").GetComponent<Text>();
        back_img.color = new Color(back_img.color.r, back_img.color.g, back_img.color.b, 0.5f); // Fondo negro alfa
        letrero.GetComponent<Image>().color = hasWin ? Color.green : Color.red;                 // Letrero color
        letrero.Find("Text").GetComponent<Text>().text = hasWin ? "Victoria" : "Derrota";       // Letrero texto
        txtLvl.text = "Lvl " + nivel.ToString();
        big_window.Find("Exp_holder").GetComponent<Text>().text = string.Format("+{0} exp", expAdquired).ToString();
        big_window.Find("Adn_holder").Find("text_adn").GetComponent<Text>().text = hasWin ? "+10" : "+5";
        
        actualExp_slider.value = expNow / 100f;
        newExp_slider.value = (expNow + expAdquired) / 100f;

        bool windowShowed = false;

        yield return Items.Instance.ItemSpriteByID(GameManager.Instance.player.criatura.equipment.head.ID, result =>
        {
            big_window.Find("MyFace").GetComponent<Image>().sprite = result;
            after_battle.gameObject.SetActive(true);
            windowShowed = true;
        });

        while (!windowShowed) yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(1f); // Esperar un segundo después de mostrarse la ventana.

        float t = 0f;
        while (t < 1) // Mover barra
        {
            actualExp_slider.value = Mathf.Lerp(actualExp_slider.value, newExp_slider.value, t);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (expNow + expAdquired >= 100) // Si subes de nivel
        {
            txtLvl.text = "Lvl " + (nivel + 1).ToString();
            newExp_slider.value = (expNow + expAdquired - 100) / 100f;
            actualExp_slider.value = 0.0f;
            yield return new WaitForSeconds(0.8f);
            t = 0f;
            while (t < 1)
            {
                actualExp_slider.value = Mathf.Lerp(actualExp_slider.value, newExp_slider.value, t);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        big_window.Find("Button_Ok").GetComponent<Button>().interactable = true;

    }

    public void ProcesarScoreAndEnd()
    {
        Database.Instance.DesactivarListenersDB();
        Player oponente = OpponentPlayerN() == 1 ? player1 : player2;
        UserDB userData = GameManager.Instance.userdb;

        if (userData.cofres.Count < 4) userData.coronas += hasWin ? 1 : 0;
        userData.nivel = CalcularPuntosVictoria(userData.nivel, hasWin);

        if (oponente.ID == "IA")
        {
            userData.gold += hasWin ? 15 : 8;
            if (hasWin) PlayerPrefs.SetInt("victorias", PlayerPrefs.GetInt("victorias") + 1);
                   else PlayerPrefs.SetInt("derrotas", PlayerPrefs.GetInt("derrotas") + 1);
        }
        else
        {
            userData.victorias += hasWin ? 1 : 0;
            userData.derrotas += !hasWin ? 1 : 0;
            userData.elo += hasWin ? 15 : -10;
            userData.gold += hasWin ? 30 : 16;
        }

        Database.Instance.ReferenceDB().Child("data").SetRawJsonValueAsync(JsonUtility.ToJson(userData)).ContinueWith(task2 => {
            Destroy(GameManager.Instance.gameObject);
            SceneManager.LoadScene(0);
        });
    }

    public void Go_back()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene(0);
    }

    // Posición cosas

    IEnumerator ColocarBarrasDeSalud()
    {

        Image image_player1 = barra_player1.transform.Find("Image").GetComponent<Image>();
        Image image_player2 = barra_player2.transform.Find("Image").GetComponent<Image>();

        yield return StartCoroutine(Items.Instance.ItemSpriteByID(player1.criatura.equipment.head.ID, result => {
            image_player1.sprite = result;
            image_player1.color = Color.white;
            image_player1.preserveAspect = true;
        }));

        yield return StartCoroutine(Items.Instance.ItemSpriteByID(player2.criatura.equipment.head.ID, result => {
            image_player2.sprite = result;
            image_player2.color = Color.white;
            image_player2.preserveAspect = true;
            barra_player1.transform.parent.parent.gameObject.SetActive(true);
        }));
    }

    void PutCardOnFocus(int n)
    {
        Color colorDescarting = new Color(0.4f, 0.2f, 0.2f);
        switch (n)
        {
            case 0:
                m_card_1.color = Color.white;
                m_card_2.color = Color.white;
                m_card_3.color = Color.white;
                m_card_4.color = Color.white;
                break;
            case 1:
                m_card_1.color = Color.white;
                m_card_2.color = Color.white;
                m_card_3.color = Color.white;
                m_card_4.color = Color.white;
                break;
            case 2:
                m_card_1.color = colorDescarting;
                m_card_2.color = Color.white;
                m_card_3.color = Color.white;
                m_card_4.color = Color.white;
                break;
            case 3:
                m_card_1.color = colorDescarting;
                m_card_2.color = colorDescarting;
                m_card_3.color = Color.white;
                m_card_4.color = Color.white;
                break;
            case 4:
                m_card_1.color = colorDescarting;
                m_card_2.color = colorDescarting;
                m_card_3.color = colorDescarting;
                m_card_4.color = Color.white;
                break;
        }
        //foreach (Transform t in t_discard)
        //{
        //    t.GetComponent<Image>().sprite = s_button_red;
        //}
        //foreach (Transform t in t_hand)
        //{
        //    t.GetComponent<Image>().sprite = s_button;
        //}
        //foreach (Transform t in t_Focus)
        //{
        //    t.GetComponent<Image>().sprite = s_button;
        //}

    }

    void MoveCard(Transform t, bool focus)
    {
        if (focus)
        {
            t.localPosition = Vector3.Lerp(t.localPosition, new Vector3(t.localPosition.x, yMax, 0), Time.deltaTime * 10);
            t.localScale = Vector3.Lerp(t.localScale, scaleMax, Time.deltaTime * 10);
        }
        else
        {
            t.localPosition = Vector3.Lerp(t.localPosition, new Vector3(t.localPosition.x, yMin, 0), Time.deltaTime * 10);
            t.localScale = Vector3.Lerp(t.localScale, scaleMin, Time.deltaTime * 10);
        }
    }

    void MoveCard(Transform t, bool focus, Vector3 specialVector)
    {
        if (focus)
        {
            t.localPosition = Vector3.Lerp(t.localPosition, specialVector, Time.deltaTime * 10);
            t.localScale = Vector3.Lerp(t.localScale, scaleMax, Time.deltaTime * 10);
        }
        else
        {
            t.localPosition = Vector3.Lerp(t.localPosition, specialVector, Time.deltaTime * 10);
            t.localScale = Vector3.Lerp(t.localScale, scaleMin, Time.deltaTime * 10);
        }
    }

    void RectPositions()
    {
        if (yourTurn)
        {
            rect_skills.localPosition = Vector3.Lerp(rect_skills.localPosition, Vector3.zero, Time.deltaTime * 5);
            if (selectedButton == 1) MoveCard(headT, true); else if (lastSelection == 1) MoveCard(headT, false); else MoveCard(headT, false);
            if (selectedButton == 2) MoveCard(bodyT, true); else MoveCard(bodyT, false);
            if (selectedButton == 3) MoveCard(armsT, true); else MoveCard(armsT, false);
            if (selectedButton == 4) MoveCard(legsT, true); else if (lastSelection == 4) MoveCard(legsT, false); else MoveCard(legsT, false);
        }
        else rect_skills.localPosition = Vector3.Lerp(rect_skills.localPosition, new Vector3(0, -160, 0), Time.deltaTime * 5);

        if (!Mathf.Approximately(fadeScreen.color.a, fadeToAphaValue))
        {
            fadeScreen.color = Color.Lerp(fadeScreen.color, new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, fadeToAphaValue), Time.deltaTime * 2);
        }
    }

    void PutPlayerOnTop(int playerN)
    {
        Canvas topCanvas = playerN == 1 ? canvas_player1 : canvas_player2;
        Canvas lowCanvas = playerN == 1 ? canvas_player2 : canvas_player1;
        topCanvas.sortingOrder = 2;
        lowCanvas.sortingOrder = 1;
    }

    IEnumerator RutinaEstados(int playerN)
    {
        if (!acabando)
        {
            // Myself
            if (lastSkillResult.isLucky)
            {
                InfoExtraBuff(Buffs.Luck);
            }

            if (lastSkillResult.recoverHP > 0)
            {
                InfoExtraBuff(Buffs.Heal);
            }

            if (lastSkillResult.myself_damage > 0)
            {
                InfoExtraBuff(Buffs.AutoDamage);
            }

            if (lastSkillResult.buff_attack > 0)
            {
                InfoExtraBuff(Buffs.Attack);
            }

            if (lastSkillResult.buff_skill > 0)
            {
                InfoExtraBuff(Buffs.Skill);
            }

            if (lastSkillResult.buff_barrier > 0)
            {
                InfoExtraBuff(Buffs.Barrier);
            }

            if (lastSkillResult.buff_shield > 0)
            {
                InfoExtraBuff(Buffs.Shield);
            }

            if (lastSkillResult.myself_bleed > 0)
            {
                InfoExtraDebuff(Debuffs.Bleed);
            }

            if (lastSkillResult.myself_confusion > 0)
            {
                InfoExtraDebuff(Debuffs.Confusion);
            }

            if (lastSkillResult.myself_poison > 0)
            {
                InfoExtraDebuff(Debuffs.Poison);
            }
            
            if (lastSkillResult.myself_dizziness > 0)
            {
                InfoExtraDebuff(Debuffs.Dizziness);
            }

            // Enemigo
            UpdateBarraDeSalud(1);
            UpdateBarraDeSalud(2);
            UpdateBuffsUI(1);
            UpdateBuffsUI(2);
            int vida = playerN == 1 ? player1.status.health_now : player2.status.health_now;

            if (lastSkillResult.damage > 0)
            {
                Animations.Instance.PlayAnim(3, playerN);
                InfoFlotante(Debuffs.Damage, lastSkillResult.damage);
                yield return new WaitForSeconds(1f);
            }
            
            if (lastSkillResult.bleed > 0 && vida > 0 && !acabando)
            {
                Animations.Instance.PlayAnim(6, playerN);
                InfoFlotante(Debuffs.Bleed);
                yield return new WaitForSeconds(1f);
            }
            if (lastSkillResult.poison > 0 && vida > 0 && !acabando)
            {
                Animations.Instance.PlayAnim(5, playerN);
                InfoFlotante(Debuffs.Poison);
                yield return new WaitForSeconds(1f);
            }
            if (lastSkillResult.dizziness > 0 && vida > 0 && !acabando)
            {
                Animations.Instance.PlayAnim(4, playerN);
                InfoFlotante(Debuffs.Dizziness);
                yield return new WaitForSeconds(1f);
            }
            if (lastSkillResult.confusion > 0 && vida > 0 && !acabando)
            {
                Animations.Instance.PlayAnim(4, playerN);
                InfoFlotante(Debuffs.Confusion);
                yield return new WaitForSeconds(1f);
            }

            if (vida < 1) print("Se murió");
        }
    }

}
