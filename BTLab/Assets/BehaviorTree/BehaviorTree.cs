using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree(){
        name = "Tree";
    }

    public BehaviorTree(string s){
        name = s;
    }

    struct NodeLevel{
        public int level;
        public Node node;

        public NodeLevel(int level, Node node)
        {
            this.level = level;
            this.node = node;
        }
    }

    public void PrintTree(){
        string treePrintout = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currNode = this;
        nodeStack.Push(new NodeLevel(0, currNode));
        while(nodeStack.Count != 0){
            NodeLevel nextNode = nodeStack.Pop();
            treePrintout += new string('-', nextNode.level) + nextNode.node.name + "\n";
            for(int i = nextNode.node.children.Count - 1; i >= 0; i--){
                nodeStack.Push(new NodeLevel(nextNode.level+1, nextNode.node.children[i]));
            }
        }
        Debug.Log(treePrintout);
    }

    public override Status Process()
    {
        return children[currentChild].Process();
    }
}
