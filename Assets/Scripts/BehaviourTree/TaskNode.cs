using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TaskState { Running, Success, Failure }
public abstract class TaskBT 
{
    public abstract TaskState Execute();
}

public class TaskNode : Node
{
    protected List<TaskBT> Tasks { get; private set; } = new();
    private int CurrentTaskIndex { get; set; }

    public TaskNode(string tag, IEnumerable<TaskBT> tasks)
        :base(tag)
    {
        foreach (TaskBT task in tasks)
        {
            AddTask(task);
        }
    }

    public void AddTask(TaskBT task) => Tasks.Add(task);

    protected override NodeState InnerEvaluate()
    {
        bool executeNextTask = true;
        int taskCount = Tasks.Count;

        while (executeNextTask)
        {
            TaskBT currentTask = Tasks[CurrentTaskIndex];
            TaskState currentTaskState = currentTask.Execute();

            switch (currentTaskState)
            {
                case TaskState.Failure:
                    State = NodeState.Failure;
                    return State;
                case TaskState.Running:
                    executeNextTask = false;
                    break;
                case TaskState.Success:
                    if (CurrentTaskIndex == taskCount - 1)
                    {
                        CurrentTaskIndex = 0;
                        State = NodeState.Success;
                        return State;
                    }
                  
                    ++CurrentTaskIndex;
                    break;
            }
        }

        State = NodeState.Running;
        return State;
    }
}

public class DummyTask : TaskBT
{
    private TaskState ReturnState { get; set; }
    private string Message { get; set; }
    public DummyTask(string message, TaskState returnState)
    {
        Message = message;
        ReturnState = returnState;
    }
    public override TaskState Execute()
    {
        Debug.Log(Message);
        return ReturnState;
    }
}

public class Wait : TaskBT
{
    private float ElapsedTime { get; set; } = 0;
    private float SecondsToWait { get; set; } = 0;
    private Action<float, float> OnUpdate { get; }
    private Action OnComplete { get; }
    private bool Scaled { get; }

    public Wait(float secondsToWait, Action<float, float> onUpdate = null, Action onComplete = null, bool scaled = true)
    {
        SecondsToWait = secondsToWait;
        OnUpdate = onUpdate;
        OnComplete = onComplete;
        Scaled = scaled;
    }

    //On tient pour acquis qu'Execute va être appeler à chaque frame
    public override TaskState Execute()
    {
        ElapsedTime += Scaled ? Time.deltaTime : Time.unscaledDeltaTime;

        if (ElapsedTime > SecondsToWait)
        {
            if (OnComplete != null)
                OnComplete();
            ElapsedTime = 0;
            return TaskState.Success;
        }
        if (OnUpdate != null)
            OnUpdate(ElapsedTime, SecondsToWait);
        return TaskState.Running;
    }
}
