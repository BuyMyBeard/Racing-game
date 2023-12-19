using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaitUntilCarClose : TaskBT
{
    Dictionary<string, bool> blackboard;
    Transform Self { get; }
    NavMeshAgent Agent { get; }
    float DistanceThreshold { get; }

    List<Transform> destinations;
    int curDestination = 0;
    public WaitUntilCarClose(Dictionary<string, bool> blackboard, NavMeshAgent agent, float distanceThreshold)
    {
        Agent = agent;
        Self = agent.transform;
        DistanceThreshold = distanceThreshold;
        var patrolPointsParent = GameObject.FindGameObjectWithTag("NPCPatrolPoints");
        List<Transform> patrolPoints = new List<Transform>();
        for (int i = 0; i < patrolPointsParent.transform.childCount; i++)
            patrolPointsParent.transform.GetChild(i);
        destinations = patrolPoints;
        this.blackboard = blackboard;
        agent.destination = destinations[0].position;
    }
    public override TaskState Execute()
    {
        if (Vector3.Distance(Agent.destination, Self.position) < DistanceThreshold)
        {
            Agent.destination = destinations[(++curDestination) % destinations.Count].position;
        }
        return blackboard["isAboutToBeHit"] ? TaskState.Success : TaskState.Running;
    }
}

public class RunAwayInFear : TaskBT
{
    bool hasStartedOnce;
    Dictionary<string, bool> blackboard;
    Animator Animator { get; }
    public RunAwayInFear(Dictionary<string, bool> blackboard, Animator animator)
    {
        this.blackboard = blackboard;
        Animator = animator;
    }
    public override TaskState Execute()
    {
        if (!hasStartedOnce)
        {
            Animator.SetBool("IsRunning", true);
            hasStartedOnce = true;
        }
        if (Animator.GetBool("IsRunning"))
            return TaskState.Running;
        else
        {
            hasStartedOnce = false;
            blackboard["isAboutToGetHit"] = false;
            return TaskState.Success;
        }
    }
}
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class PedestrianBehaviourTree : MonoBehaviour
{
    const float DISTANCE_THRESHOLD = 0.05f;
    private Node rootBT;
    public Dictionary<string, bool> blackboard = new Dictionary<string, bool>();
    private void Awake()
    {
        blackboard["isAboutToBeHit"] = false;

        TaskBT[] tasks0 = new TaskBT[]
        {
            new WaitUntilCarClose(blackboard, GetComponent<NavMeshAgent>(), DISTANCE_THRESHOLD)
        };
        TaskBT[] tasks1 = new TaskBT[]
        {
            new RunAwayInFear(blackboard, GetComponent<Animator>())
        };

        TaskNode waitToGetHitNode = new TaskNode("waitToGetHit", tasks0);
        TaskNode runawayFromCarNode = new TaskNode("runawayFromCar", tasks1);

        rootBT = new Sequence("seq1", new[] { waitToGetHitNode, runawayFromCarNode });
    }

    void Update()
    {
        rootBT.Evaluate();
    }
}
