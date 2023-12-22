using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class WaitUntilCarClose : TaskBT
{
    Dictionary<string, bool> blackboard;
    NavMeshAgent Agent { get; }
    float DistanceThreshold { get; }
    Animator Animator { get; }

    List<Transform> destinations;
    int curDestination = 0;
    public WaitUntilCarClose(Dictionary<string, bool> blackboard, NavMeshAgent agent, float distanceThreshold, Animator animator)
    {
        this.blackboard = blackboard;
        Agent = agent;
        DistanceThreshold = distanceThreshold;
        Animator = animator;
        var patrolPointsParent = Agent.transform.parent.GetChildByName("NPCPatrolPoints");
        List<Transform> patrolPoints = new List<Transform>();
        for (int i = 0; i < patrolPointsParent.transform.childCount; i++)
            patrolPoints.Add(patrolPointsParent.transform.GetChild(i));
        destinations = patrolPoints;
        if (destinations.Count == 0)
        {
            Debug.Log("No destinations found !");
            return;
        }
        Agent.destination = destinations[0].position;
    }
    public override TaskState Execute()
    {
        Animator.SetBool("IsWalking", true);
        if (Vector3.Distance(Agent.destination, Agent.transform.position) < DistanceThreshold)
        {
            curDestination = (curDestination + 1) % destinations.Count;
            var newDestination = destinations[curDestination];
            Debug.Log($"Changing destination for {Agent.gameObject.name} to destination {newDestination.name}");
            Agent.destination = newDestination.position;
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
    [SerializeField] bool isMale;
    [SerializeField] float speed = 2f;
    const float DISTANCE_THRESHOLD = 0.5f;
    private Node rootBT;
    public Dictionary<string, bool> blackboard = new Dictionary<string, bool>();
    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        blackboard["isAboutToBeHit"] = false;
        animator.SetBool("IsMale", isMale);
        agent.speed = speed;

        TaskBT[] tasks0 = new TaskBT[]
        {
            new WaitUntilCarClose(blackboard, agent, DISTANCE_THRESHOLD, animator)
        };
        TaskBT[] tasks1 = new TaskBT[]
        {
            new RunAwayInFear(blackboard, animator)
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
