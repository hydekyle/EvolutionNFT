using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;

public class MatchMaking : MonoBehaviour {

    public static MatchMaking Instance;
    public bool searching = false;
    Image background;
    long myTimeTics;
    public Text victorias_IA, derrotas_IA, lvl_IA;

    private void Start()
    {
        Instance = this;
        background = transform.Find("BackBlack").GetComponent<Image>();
    }

    IEnumerator FadeTo(float value)
    {
        Color fadeToColor = new Color(background.color.r, background.color.g, background.color.b, value);

        float t = 0f;

        while (t < 1)
        {
            background.color = Color.Lerp(background.color, fadeToColor, t);
            t += Time.deltaTime * 2;
            yield return new WaitForFixedUpdate();
        }
        background.color = fadeToColor;
    }

    public void BTN_vsIA()
    {
        QuitarVentanaBattle();
        GameManager.Instance.PlayVsBot();
    }

    public void BTN_Play()
    {
        background.raycastTarget = true;
        StartCoroutine(FadeTo(0.5f));
        GetComponent<Animator>().Play("MatchMaking_On");
    }

    public void BTN_Background()
    {
        QuitarVentanaBattle();
    }

    public void QuitarVentanaBattle()
    {
        StartCoroutine(FadeTo(0f));
        background.raycastTarget = false;
        GetComponent<Animator>().Play("MatchMaking_Off");
    }

    public void BTN_CancelarOnline() // Cancelar búsqueda
    {
        if (searching)
        {
            transform.Find("Searching").gameObject.SetActive(false);
            searching = false;
            FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").ChildAdded -= MatchMaking_ChildAdded;
            Database.Instance.ReferenceMyInvitation().ChildAdded -= MatchMaking_NewInvitation;

            FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").RunTransaction(data => {

                Dictionary<string, object> info = new Dictionary<string, object>();
                info = data.Value as Dictionary<string, object>;

                data.Child(GameManager.Instance.GetUserID()).Value = null;

                return TransactionResult.Success(data);
            }); // Eliminarse al salir de cola
        }
    }

    public void BTN_PlayOnline() // Buscar partida
    {
        if (!searching)
        {
            searching = true;
            transform.Find("Searching").gameObject.SetActive(true);
            QuitarVentanaBattle();
            FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").ChildAdded += MatchMaking_ChildAdded;
            Database.Instance.ReferenceMyInvitation().ChildAdded += MatchMaking_NewInvitation;

            StartCoroutine(Database.Instance.GetRealTime(result => {

                myTimeTics = result.Ticks;

                Dictionary<string, object> info = new Dictionary<string, object>();

                FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").GetValueAsync().ContinueWith(task => // Leer sala
                {
                    if (task.IsCompleted) 
                    {
                        info = task.Result.Value as Dictionary<string, object>;

                        FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").RunTransaction(data => {

                            Client myClient = new Client()
                            {
                                elo = GameManager.Instance.userdb.elo,
                                time_tics = myTimeTics
                            };

                            if (info.ContainsKey(GameManager.Instance.GetUserID()))
                            {
                                print("Estoy suscrito");
                                myClient = JsonUtility.FromJson<Client>((string)info[GameManager.Instance.GetUserID()]);
                            }
                            else
                            {
                                print("No estoy suscrito");
                                data.Child(GameManager.Instance.GetUserID()).Value = JsonUtility.ToJson(myClient); // Suscribirse a la lista si no estoy
                            }

                            return TransactionResult.Success(data);
                        });
                    }
                });
            }));
        }
    }

    private void MatchMaking_NewInvitation(object sender, ChildChangedEventArgs e)
    {
        print(e.Snapshot.Value);
    }

    private void LeerClientes(Dictionary<string, object> lista)
    {

    }

    private void InsertClientTest()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").RunTransaction(data => {

            TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.AddDays(-3).Ticks);

            Client client = new Client()
            {
                elo = 1166,
                time_tics = timeSpan.Ticks
            };
            data.Child("HydePambito6").Value = JsonUtility.ToJson(client);

            return TransactionResult.Success(data);

        });
    }

    //private void Invitation_Received(object sender, ValueChangedEventArgs e)
    //{
    //    if (e.Snapshot.Value.ToString().Length > 1)
    //    {
    //        print("Invitación recibida de " + e.Snapshot.Value);
    //        Dictionary<string, object> info = new Dictionary<string, object>();
    //        FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").RunTransaction(data => {

    //            info = data.Value as Dictionary<string, object>;

    //            if (info.ContainsKey(GameManager.Instance.GetUserID()))
    //            {
    //                data.Child(GameManager.Instance.GetUserID()).Value = null; // Desuscribirse e ir a batalla
    //            }

    //            return TransactionResult.Success(data);
    //        });
    //    }
    //}

    private void SendInvitationTo(string id)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("Inventario").Child(id).Child("invitation").SetValueAsync(GameManager.Instance.GetUserID()).ContinueWith(task => 
        {
            print("Invitación enviada");
        });
    }

    private void MatchMaking_ChildAdded(object sender, ChildChangedEventArgs e)
    {
        if (e.Snapshot.Key == "Options")
        {
            print("Cargo las opciones");
        }
        else if (e.Snapshot.Key != GameManager.Instance.GetUserID())
        {
            #region Cliente_Nuevo

            Client cliente = JsonUtility.FromJson<Client>(e.Snapshot.Value.ToString());
            print("Nuevo usuario " + e.Snapshot.Key);
            print("Elo: " +cliente.elo);
            TimeSpan timeCliente = new TimeSpan(cliente.time_tics);
            TimeSpan timeNow = new TimeSpan(myTimeTics);

            if (timeCliente.Days != timeNow.Days)
            {
                print("El cliente lleva más de un día ahí "+ timeNow.Subtract(timeCliente));
                FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").RunTransaction(data => { 

                    data.Child(e.Snapshot.Key).Value = null;
                    return TransactionResult.Success(data);
                });
            }
            if (Math.Abs(GameManager.Instance.userdb.elo - cliente.elo) < 100) // Si la diferencia de elo es menor que 100
            {
                SendInvitationTo(e.Snapshot.Key);
            }

            #endregion
        }
        else if(e.Snapshot.Key == GameManager.Instance.GetUserID())
        {
            Client client = new Client()
            {
                elo = GameManager.Instance.userdb.elo,
                time_tics = myTimeTics
            };
            FirebaseDatabase.DefaultInstance.RootReference.Child("Matchmaking").Child(e.Snapshot.Key).SetValueAsync(JsonUtility.ToJson(client));
        }
    }

    public void EstadisticasIA()
    {
        int victorias = PlayerPrefs.GetInt("victorias");
        victorias_IA.text = victorias.ToString();
        derrotas_IA.text = PlayerPrefs.GetInt("derrotas").ToString();
        lvl_IA.text = "Lvl " + GetLvlIA().ToString();
    }

    public int GetLvlIA()
    {
        int victorias = PlayerPrefs.GetInt("victorias");

        return victorias > 3 ? victorias / 4 + 1 : 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && background.raycastTarget) QuitarVentanaBattle();
        if (Input.GetKeyDown(KeyCode.T)) InsertClientTest();
    }

}
