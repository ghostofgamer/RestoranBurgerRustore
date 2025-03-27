using System;
using UnityEngine;

public class WorkSpeedRecorder
{
    private DateTime startTime;

    private string workName;

    public WorkSpeedRecorder() {}

    public WorkSpeedRecorder(string workName)
    {
        StartRecord(workName);
    }
    
    public void StartRecord(string workName)
    {
        this.workName = workName;
        startTime = DateTime.Now;
    }
    
    public void StopRecord()
    {
        DateTime stopTime = DateTime.Now;
        TimeSpan elapsedTime = stopTime - startTime;
        Debug.Log($"{workName} speed: {elapsedTime.TotalMilliseconds}ms");
    }
}