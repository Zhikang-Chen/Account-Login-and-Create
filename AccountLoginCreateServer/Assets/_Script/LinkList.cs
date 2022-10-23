using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkList<T>
{
    public class Node
    {
        public T Data;
        public Node Previous = null;
        public Node Next = null;
    }

    public int Count = 0;
    public Node Root;
    public Node LastNode;

    public LinkList()
    {
        Root = new Node();
        LastNode = Root;
    }
        
    public void Add(T Data)
    {
        Node newNode = new Node();
        newNode.Data = Data;
        newNode.Previous = LastNode;
        LastNode.Next = newNode;
        LastNode = newNode;
        Count++;
        newNode = null;
    }

    public bool DeleteData(T Data)
    {
        Node previousNode = null;
        Node currentNode = Root;
        while(currentNode != null)
        {
            currentNode = currentNode.Next;
            if(currentNode.Data.Equals(Data))
            {
                if(currentNode == Root)
                {
                    Root = currentNode.Next;
                }
                else if (currentNode == LastNode)
                {
                    LastNode = currentNode.Previous;
                }
                else
                {
                    currentNode.Previous.Next = currentNode.Next;
                    currentNode.Next.Previous = currentNode.Previous;
                }
                Count--;
                return true;
            }
            previousNode = currentNode;
        }
        return false;
    }


}
