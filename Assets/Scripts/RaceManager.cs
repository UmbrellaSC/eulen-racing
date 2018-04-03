using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour {

    /// <summary>
    /// Alle Checkpoints die das Auto passieren muss
    /// </summary>
    public List<TriggerDetector> Checkpoints;

    /// <summary>
    /// Der Index des nächsten Checkpoints den das Auto passieren muss
    /// </summary>
    private Int32 _nextCheckpoint;

    /// <summary>
    /// Die Anzahl an Runden die das Rennen hat
    /// </summary>
    public Int32 Rounds;

    /// <summary>
    /// Die momentane Runde des Autos
    /// </summary>
    public Int32 CurrentRound;

    /// <summary>
    /// Die Zeit zu der das Rennen gestartet wurde
    /// </summary>
    private DateTime _startTime;

    /// <summary>
    /// Die Zeit bei der das Rennen beendet wurde, bzw. die momentane Zeit falls das Rennen noch läuft
    /// </summary>
    private DateTime _finishedTime;

    /// <summary>
    /// Ein Callback das ausgeführt wird wenn das Rennen beendet ist
    /// </summary>
    public Action RaceFinished;

    /// <summary>
    /// Die Zeit die seit dem Start des Rennens vergangen ist
    /// </summary>
    public TimeSpan Time
    {
        get { return _finishedTime - _startTime; }
    }

    private Boolean isRacing;

	// Use this for initialization
	void Start ()
    {
        for (Int32 l = 0; l < Checkpoints.Count; l++)
        {
            Int32 i = l;
            Checkpoints[i].Callback += (e) =>
            {
                Int32 current = i;
                if (current > Checkpoints.Count)
                {
                    current -= Checkpoints.Count;
                }
                if (current == _nextCheckpoint)
                {
                    if (current == 0)
                    {
                        if (CurrentRound == Rounds)
                        {
                            isRacing = false;
                            RaceFinished?.Invoke();
                            return;
                        }
                        CurrentRound++;
                    }
                    _nextCheckpoint++;
                }
                if (_nextCheckpoint == Checkpoints.Count)
                {
                    _nextCheckpoint = 0;
                }
                else
                {
                    _nextCheckpoint++;
                }

                ((Behaviour)Checkpoints[i].GetComponent("Halo")).enabled = false;
                ((Behaviour)Checkpoints[_nextCheckpoint].GetComponent("Halo")).enabled = true;

            };
            ((Behaviour)Checkpoints[i].GetComponent("Halo")).enabled = false;
        }
        ((Behaviour)Checkpoints[0].GetComponent("Halo")).enabled = true;
        _startTime = DateTime.Now;
        isRacing = true;
        _nextCheckpoint = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isRacing)
        {
            _finishedTime = DateTime.Now;
        }
#if NERVIGE_KINDER
        Debug.Log(187);
#endif
    }
}
