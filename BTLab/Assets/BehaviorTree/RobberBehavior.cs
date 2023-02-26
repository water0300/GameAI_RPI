using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : MonoBehaviour
{
    public enum ActionState {IDLE, WORKING}
    ActionState state = ActionState.IDLE;
    public GameObject diamond;
    public GameObject van;
    public GameObject frontdoor;
    public GameObject backdoor;
    NavMeshAgent agent;
    BehaviorTree tree;
    [Range(0, 1000)]
    public int money = 800;

    Node.Status treeStatus = Node.Status.RUNNING;
    private void Start() {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviorTree();
        Sequence steal = new Sequence("Steal something");
        Selector openDoor = new Selector("Select door");
        Leaf hasMoney = new Leaf("Has Money", HasMoney);
        Leaf goToBackdoor = new Leaf("Go to back Door", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go to front Door", GoToFrontdoor);
        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond);
        Leaf goToVan = new Leaf("Go to Van", GoToVan);

        openDoor.AddChild(goToFrontdoor);
        openDoor.AddChild(goToBackdoor);
        
        steal.AddChild(hasMoney);
        steal.AddChild(openDoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();
        tree.Process();

    }

    public Node.Status GoToDiamond(){
        Node.Status s = GoToLocation(diamond.transform.position);
        if(s == Node.Status.SUCCESS){
            diamond.transform.parent = this.transform;
        }
        return s;
    }
    public Node.Status HasMoney(){
        if(money >= 500){
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }
    public Node.Status GoToVan(){
        Node.Status s = GoToLocation(van.transform.position);
        if(s == Node.Status.SUCCESS){
            money += 300;
            diamond.SetActive(false);
        }
        return s;    
    }
    public Node.Status GoToBackdoor() => GoToDoor(backdoor);
    public Node.Status GoToFrontdoor() => GoToDoor(frontdoor);
    public Node.Status GoToDoor(GameObject door){
        Node.Status s = GoToLocation(door.transform.position);
        if(s == Node.Status.SUCCESS){
            if(!door.GetComponent<Lock>().isLocked){
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else {
            return s;
        }
    }

    Node.Status GoToLocation(Vector3 destination){
        float ddt = Vector3.Distance(destination, this.transform.position);
        if(state == ActionState.IDLE){
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        } else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2f){
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        } else if (ddt < 2){
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    void Update(){
        if(treeStatus != Node.Status.SUCCESS)
            treeStatus = tree.Process();
        
    }
}
