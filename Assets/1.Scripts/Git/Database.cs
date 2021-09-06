using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Database : MonoBehaviour {

    public string[] items;
    public static Database Instance { get; set; }
    public Button button_room1;
    Text text_room1;

    string master_player_ID;
    bool buscando_match = false;

    public bool invRecibida = false;

    void Awake()
    {
        Instance = this;
        //StartCoroutine(GetRealTime());
        //text_room1 = button_room1.transform.Find("Text").GetComponent<Text>();
    }

    public void ActiveListenersDB()
    {
        master_player_ID = GameManager.Instance.GetUserID();

        ReferenceDataTurn().SetValueAsync("").ContinueWith(task2 =>
        {
            Firebase.Auth.FirebaseAuth.DefaultInstance.StateChanged += CanvasBase.Instance.TryRelog;
            ReferenceRoom().ValueChanged += OnRoomValueChanged;
            ReferenceDB().ValueChanged += OnDataDBChanged;
        });
    }

    public void DesactivarListenersDB()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.StateChanged -= CanvasBase.Instance.TryRelog;
        ReferenceDataTurn().ValueChanged -= OnDataTurnUpdate;
        ReferenceRoom().ValueChanged -= OnRoomValueChanged;
        //ReferenceMyInvitation().ValueChanged -= OnInvitationReceived;
        ReferenceDB().ValueChanged -= OnDataDBChanged;
    }

    private void OnDataDBChanged(object sender, ValueChangedEventArgs e)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("data").GetValueAsync().ContinueWith(task =>
        {
            string json = task.Result.GetRawJsonValue();
            GameManager.Instance.userdb = JsonUtility.FromJson<UserDB>(json);
        });
        //print("Cambio: " + e.Snapshot);
    }

    public DatabaseReference ReferenceDataTurn()
    {
        return FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(master_player_ID).Child("dataTurn");
    }

    public DatabaseReference ReferenceMyInvitation()
    {
        return FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("invitation");
    }

    public DatabaseReference ReferenceDB()
    {
        return FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID());
    }

    public DatabaseReference ReferenceDataDB()
    {
        return ReferenceDB().Child("data");
    }

    public DatabaseReference ReferenceRoom()
    {
        return FirebaseDatabase.DefaultInstance.RootReference.Child("Rooms").Child("Room1");
    }

    public IEnumerator GetRealTime(Action<TimeSpan> result)
    {
        WWW www = new WWW("http://evolution-battle.com/EvolutionPortable/GetTime.php");
        yield return www;
        if (www.isDone)
        {
            try
            {
                string[] info = www.text.Split('T');
                string[] fecha = info[0].Split('-');
                string[] hora = info[1].Replace("Z", string.Empty).Split(':');
                
                Fecha fechaActual = new Fecha()
                {
                    month = int.Parse(fecha[1]),
                    day = int.Parse(fecha[2]),
                    hour = int.Parse(hora[0]),
                    min = int.Parse(hora[1])
                };

                TimeSpan timeSpan = new TimeSpan(fechaActual.day, fechaActual.hour, fechaActual.min, 0);
                result(timeSpan);
            }catch { print("ERROR EN TIME WWW: " + www.text); }
        }
    }

    private void OnRoomValueChanged(object sender, ValueChangedEventArgs e)         //LISTENER
    {
        Room room = JsonUtility.FromJson<Room>(e.Snapshot.GetRawJsonValue());
        if (room.guest == GameManager.Instance.GetUserID())          //Si he entrado yo como guest, mando invitación al owner y limpio la sala.
        {
            master_player_ID = room.owner;
            //ReferenceDataTurn().ValueChanged += OnDataTurnUpdate; 
            MandarInvitacion(room.owner);
        }

        if (buscando_match) //Si estaba la sala llena, de que haya un cambio intentar entrar.
        {
            InsertDataRoom();
        }
        
        //text_room1.text = room.owner;
    }

    //private void OnInvitationReceived(object sender, ValueChangedEventArgs e)         //LISTENER
    //{
    //    if(e.Snapshot.Value.ToString() != "" && !invRecibida)
    //    {
    //        if (e.Snapshot.Value.ToString().Substring(0, 1) == "!") //Alguien ya ha creado la partida y me invita
    //        {
    //            invRecibida = true;
    //            string playerID = e.Snapshot.Value.ToString().Substring(1);
    //            master_player_ID = playerID;

    //            ReferenceDataTurn().GetValueAsync().ContinueWith(task =>
    //            {
    //                Dictionary<string, object> dic = task.Result.Value as Dictionary<string, object>;
    //                ReferenceDataTurn().ValueChanged += OnDataTurnUpdate; //LISTENER
    //                DataTurn dataTurn = new DataTurn()
    //                {
    //                    player1 = (string)dic["player1"],
    //                    player2 = (string)dic["player2"],
    //                    turn_number = int.Parse(dic["turn_number"].ToString()),
    //                    minigame_fails = 0,
    //                    random_seed = 0,
    //                    used_skill = 0
    //                };
    //                print(dataTurn.player1);
    //                ReferenceMyInvitation().SetValueAsync("");
    //            });
    //        }
    //        else //Soy el host y recibo invitación del guest
    //        {
    //            invRecibida = true;
    //            print("¡Creando partida!");

    //            ReferenceMyInvitation().SetValueAsync("");
    //            DataTurn dataTurn = new DataTurn()
    //            {
    //                turn_number = UnityEngine.Random.Range(1, 3), //EMPIEZA UN JUGADOR ALEATORIO
    //                minigame_fails = 0,
    //                random_seed = 0,
    //                used_skill = 0,
    //                player1 = GameManager.Instance.GetUserID(),
    //                player2 = e.Snapshot.Value.ToString()
    //            };
    //            ReferenceDataTurn().SetRawJsonValueAsync(JsonUtility.ToJson(dataTurn)).ContinueWith(task => {
    //                ReferenceDataTurn().ValueChanged += OnDataTurnUpdate; //LISTENER
    //                FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(e.Snapshot.Value.ToString()).Child("invitation").SetValueAsync("!" + GameManager.Instance.GetUserID());
    //            });
    //        }
    //    }      
    //}

    public void OnDataTurnUpdate(object sender, ValueChangedEventArgs e)   //DataTurn LISTENER
    {
        Dictionary<string, object> dataTurn = e.Snapshot.Value as Dictionary<string, object>;

        if(dataTurn["player1"].ToString() == GameManager.Instance.GetUserID() || dataTurn["player2"].ToString() == GameManager.Instance.GetUserID()) //Comprueba que soy uno de los participantes
        {
            if (e.Snapshot.Value.ToString() != "")
            {
                if (!BattleSystem.Instance.battleON)
                {
                    Player p1 = new Player();
                    Player p2 = new Player();

                    FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child((string)dataTurn["player1"]).Child("equipamiento").GetValueAsync()
                    .ContinueWith(task =>
                    {
                        Dictionary<string, object> dicEquip = task.Result.Value as Dictionary<string, object>;
                        Equipment equip_player1 = new Equipment()
                        {
                            head = Items.Instance.ItemByID((string)dicEquip["head"]),
                            body = Items.Instance.ItemByID((string)dicEquip["body"]),
                            arms = Items.Instance.ItemByID((string)dicEquip["arms"]),
                            legs = Items.Instance.ItemByID((string)dicEquip["legs"]),
                        };
                        p1 = new Player()
                        {
                            ID = (string)dataTurn["player1"],
                            criatura = new Criatura()
                            {
                                equipment = new Equipment()
                                {
                                    head = equip_player1.head,
                                    body = equip_player1.body,
                                    arms = equip_player1.arms,
                                    legs = equip_player1.legs
                                }
                            }
                        };
                    }).ContinueWith(task2 =>
                    {
                        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child((string)dataTurn["player2"]).Child("equipamiento").GetValueAsync()
                        .ContinueWith(task3 =>
                        {
                            Dictionary<string, object> dicEquip2 = task3.Result.Value as Dictionary<string, object>;
                            Equipment equip_player2 = new Equipment()
                            {
                                head = Items.Instance.ItemByID((string)dicEquip2["head"]),
                                body = Items.Instance.ItemByID((string)dicEquip2["body"]),
                                arms = Items.Instance.ItemByID((string)dicEquip2["arms"]),
                                legs = Items.Instance.ItemByID((string)dicEquip2["legs"]),
                            };
                            p2 = new Player()
                            {
                                ID = (string)dataTurn["player2"],
                                criatura = new Criatura()
                                {
                                    equipment = new Equipment()
                                    {
                                        head = equip_player2.head,
                                        body = equip_player2.body,
                                        arms = equip_player2.arms,
                                        legs = equip_player2.legs
                                    }
                                }
                            };
                        }).ContinueWith(task4 =>
                        {
                            BattleSystem.Instance.Initialize(p1, p2);
                        });
                    });
                }  //La partida está en marcha, iniciar batalla y recuperar datos de ambos jugadores.

                //print("Recibo el data");
                BattleSystem.Instance.NewDataTurnReceived(dataTurn);
            }
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void InsertDataRoom()    //MACHTMAKING
    {
        Message.Instance.NewMessage("Buscando oponente");

        Dictionary<string, object> info = new Dictionary<string, object>();

        ReferenceRoom().RunTransaction(data =>
        {
            info = data.Value as Dictionary<string, object>;

            if ((string)info["owner"] == GameManager.Instance.GetUserID()) return TransactionResult.Abort();

            if ((string)info["owner"] == "")
            {
                info["owner"] = GameManager.Instance.GetUserID().ToString();
                buscando_match = false;
            }
            else if ((string)info["guest"] == "")
            {
                info["guest"] = GameManager.Instance.GetUserID().ToString();
                buscando_match = false;
            }
            else return TransactionResult.Abort();

            data.Value = info;

            return TransactionResult.Success(data);
        }).ContinueWith(task => {

            if(task.Exception.Message != string.Empty && (string)info["owner"] != GameManager.Instance.GetUserID())
            {
                print("Hay alguien en guest atascado");
                ExtraFunction((string)info["guest"]);
            }
            else
            {
                print("Todo va bien");
            }

        });
    }

    private void ExtraFunction(string last_guest_user)
    {
        StartCoroutine(SalaOcupada(last_guest_user));
    }

    private IEnumerator SalaOcupada(string last_guest_user)
    {
        print("Tocaría esperar");
        yield return new WaitForSeconds(1f); //Esperar a que los usuarios hagan match.
        buscando_match = true;
        ReferenceRoom().RunTransaction(data => {
            Dictionary<string, object> info = data.Value as Dictionary<string, object>;

            if ((string)info["guest"] == last_guest_user)           //Si sigue la misma persona, se ha quedado bugeada. Limpiarla.
            {
                info["guest"] = "";
                data.Value = info;
                return TransactionResult.Success(data);
            }
            else
            {
                return TransactionResult.Abort();
            }
            
        });

    }

    void MandarInvitacion(string invitedUserID)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(invitedUserID).Child("invitation").SetValueAsync(GameManager.Instance.GetUserID()).ContinueWith(task => {
            LimpiarSala();
        });
        
    }

    private void LimpiarSala()
    {
        Room roomVacia = new Room()
        {
            guest = "",
            owner = ""
        };
        ReferenceRoom().SetRawJsonValueAsync(JsonUtility.ToJson(roomVacia));
        
        //ReferenceRoom().RunTransaction(data =>
        //{
        //    Dictionary<string, object> dir = data.Value as Dictionary<string, object>;
        //    dir["guest"] = "";
        //    dir["owner"] = "";
        //    data.Value = dir;

        //    return TransactionResult.Success(data);
        //});
    }

    #region TEMP
    public void GuardarBodyBounds(BodyBounds bounds)
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            string json = JsonUtility.ToJson(bounds);
            File.WriteAllText("Assets/Resources/BodyBounds/" + GameManager.Instance.player.criatura.equipment.body.ID.ToString() + ".txt", json);
            print("Guardado BodyBounds de " + "ID: " + GameManager.Instance.player.criatura.equipment.body.ID);
        }
        
    }
    public BodyBounds LeerBodyBounds(int bodyID)
    {
        BodyBounds bounds = new BodyBounds();
        string ruta = "BodyBounds/" + bodyID.ToString();
        TextAsset txt = (TextAsset)Resources.Load(ruta);
        try { bounds = JsonUtility.FromJson<BodyBounds>(txt.text); }
        catch { print("BODY BOUNDS NOT FOUND"); }
        return bounds;
    }

    public void GuardarEquipSetting(Equipment equipment)
    {
        EquipDB equip = new EquipDB()
        {
            head = equipment.head.ID_string,
            body = equipment.body.ID_string,
            arms = equipment.arms.ID_string,
            legs = equipment.legs.ID_string
        };
        string json = JsonUtility.ToJson(equip);
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("equipamiento").SetRawJsonValueAsync(json);
    }



    public Equipment ObtenerEquipSetting()
    {
        Equipment e = new Equipment();
        EquipDB equip = new EquipDB();
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(GameManager.Instance.GetUserID()).Child("equipamiento").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                equip = JsonUtility.FromJson<EquipDB>(task.Result.GetRawJsonValue());
                e = new Equipment()
                {
                    head = Items.Instance.ItemByID(equip.head),
                    body = Items.Instance.ItemByID(equip.body),
                    arms = Items.Instance.ItemByID(equip.arms),
                    legs = Items.Instance.ItemByID(equip.legs)
                };

                Player player = new Player()
                {
                    nombre = Social.localUser.userName,
                    ID = GameManager.Instance.GetUserID(),
                    criatura = new Criatura()
                    {
                        nombre = GameManager.Instance.GetUserName(),
                        attack_att = 0,
                        defense_att = 0,
                        luck_att = 0,
                        skill_att = 0,
                        equipment = e
                    }
                };

                Menu.Instance.loadedEquipment = new Equipment()
                {
                    head = new Equipable_Item()
                    {
                        ID_string = e.head.ID_string
                    },
                    arms = new Equipable_Item()
                    {
                        ID_string = e.arms.ID_string
                    },
                    back = new Equipable_Item()
                    {
                        ID_string = e.back.ID_string
                    },
                    body = new Equipable_Item()
                    {
                        ID_string = e.body.ID_string
                    },
                    legs = new Equipable_Item()
                    {
                        ID_string = e.legs.ID_string
                    }
                };
                GameManager.Instance.player = player;
                BattleSystem.Instance.player1 = player;
                PaseBatalla.Instance.MostrarPlayerInfo();
                CanvasBase.Instance.StatsRefresh();
                StartCoroutine(Animations.Instance.CargarHeadPose(1, player.criatura.equipment.head.ID, onEnded => {
                    StartCoroutine(GameManager.Instance.MostrarJugador(player, 1, new Vector3(8, 35, -1), false));
                    Animations.Instance.blink_player1 = StartCoroutine(Animations.Instance.Pestañeo(1));
                }));
            }
        });
        return e;
    }

#endregion
}
