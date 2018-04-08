using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Skript, das die internen Werte mit den Anzeigen der UI synchronisiert
/// </summary>
public class UIUpdater : MonoBehaviour
{
    public CarController controller;

    public RaceManager manager;

    public Text speedDisplay;

    public Text roundDisplay;

    public Text timeDisplay;

    void Start()
    {
        manager.RaceFinished += () => { timeDisplay.color = Color.green; };
    }

    // Update is called once per frame
    void Update ()
    {
        speedDisplay.text = (Int32) (controller.CarRigidbody.velocity.magnitude * 3.6f) + " km/h";
        roundDisplay.text = manager.CurrentRound + "/" + manager.Rounds;
        timeDisplay.text = manager.Time.Minutes.ToString("00") + ":" + manager.Time.Seconds.ToString("00");
	}
}
