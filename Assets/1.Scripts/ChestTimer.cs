using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ChestTimer : MonoBehaviour {

    [HideInInspector]
    public GameObject timeWindow;
    public Text timeTxt;
    public static ChestTimer Instance;

    private void Awake()
    {
        timeWindow = transform.Find("ChestTimer").gameObject;
        Instance = this;
        //Database.Instance.ReferenceDataDB().Child("last_time_reward").SetValueAsync(newRewardTime.Ticks).ContinueWith(task2 =>
        //{
        //    //Cofres_System.Instance.AbrirCofre();
        //});
    }

    private void OnDisable()
    {
        timeWindow.SetActive(false);
    }

    public void CompareTimes()
    {
        Database.Instance.ReferenceDataDB().Child("last_time_reward").GetValueAsync().ContinueWith(task => {
            if (!task.IsFaulted)
            {
                print(task.Result.Value);
                TimeSpan rewardTime = new TimeSpan(long.Parse(task.Result.Value.ToString()));
                TimeSpan actualTime = new TimeSpan(DateTime.UtcNow.Ticks);

                print(TimeSpan.Compare(rewardTime, actualTime));
                print(rewardTime);
                print(DateTime.UtcNow);
                if (TimeSpan.Compare(rewardTime, actualTime) == 1) //Fecha reward es mayor
                {
                    TimeSpan tSpan = rewardTime.Subtract(new TimeSpan(DateTime.UtcNow.Ticks));
                    //print(string.Format("Hay que esperar {0} horas y {1}", tSpan.Hours, tSpan.Minutes));
                    string horas = tSpan.Hours.ToString().Length < 2 ? "0" + tSpan.Hours.ToString() : tSpan.Hours.ToString();
                    string minutos = tSpan.Minutes.ToString().Length < 2 ? "0" + tSpan.Minutes.ToString() : tSpan.Minutes.ToString();
                    timeTxt.text = string.Format("{0}:{1}", horas, minutos);
                    timeWindow.SetActive(true);
                }
                else
                {
                    TimeSpan newRewardTime = new TimeSpan(DateTime.UtcNow.AddMinutes(240).Ticks);
                    Database.Instance.ReferenceDataDB().Child("last_time_reward").SetValueAsync(newRewardTime.Ticks).ContinueWith(task2 =>
                    {
                        GameManager.Instance.userdb.last_time_reward = newRewardTime.Ticks.ToString();
                        timeWindow.SetActive(false);
                        Cofres_System.Instance.AbrirCofre();
                    });
                    
                }
            }
        });
    }

    public void RestarTiempo()
    {
        Database.Instance.ReferenceDataDB().Child("last_time_reward").GetValueAsync().ContinueWith(task =>
        {
            if (!task.IsFaulted)
            {
                DateTime rewardTime = new DateTime(long.Parse(task.Result.Value.ToString()));
                DateTime newTime = rewardTime.AddMinutes(-30);
                Database.Instance.ReferenceDataDB().Child("last_time_reward").SetValueAsync(new TimeSpan(newTime.Ticks).Ticks);
                CompareTimes();
            }
        });
    }
}
