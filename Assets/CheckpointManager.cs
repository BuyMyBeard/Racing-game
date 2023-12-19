using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] Checkpoint finishLine;
    [SerializeField] Checkpoint[] checkpoints;
    [Range(1, 10)]
    [SerializeField] int lapsToComplete = 2;

    int currentCheckpoint = -1;
    int currentLap = 1;
    [SerializeField] TextMeshProUGUI lapCounter;

    public Transform CurrentRespawnPosition => currentCheckpoint == -1 ? finishLine.RespawnPosition : checkpoints[currentCheckpoint].RespawnPosition;

    private void Start()
    {
        UpdateLapCounter();
    }
    public void PassCheckpoint(Checkpoint checkpoint)
    {
        int checkpointIndex = Array.FindIndex(checkpoints, (check) => check == checkpoint);
        if (checkpointIndex == currentCheckpoint + 1)
        {
            currentCheckpoint++;
        }
        else if (checkpointIndex == -1)
        {
            if (checkpoint == finishLine && currentCheckpoint == checkpoints.Length - 1)
            {
                currentCheckpoint = -1;
                currentLap++;
                if (currentLap > lapsToComplete) Win();
                else UpdateLapCounter();
                    
            }
            else if (checkpoint != finishLine)
                Debug.Log("Checkpoint is not registered");
        }
    }
    void UpdateLapCounter() => lapCounter.SetText($"Laps: {currentLap} / {lapsToComplete}");

    public void Win()
    {
        Debug.Log("YOU COMPLETED THE RACE!!!!");
    }
}
