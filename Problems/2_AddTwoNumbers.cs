using System;
using System.Collections.Generic;

// -- ListNode model taken from leetcode, then updated to read test cases
public class ListNode {
  public int val;
  public ListNode next;

  public ListNode(int val = 0, ListNode next = null) {
    this.val = val;
    this.next = next;
  }

  // for reading inputs from test cases
  public ListNode(List<int> inputs) {
    if(inputs.Count == 0) return;
    
    this.val = inputs[0];
    
    if(inputs.Count == 1) 
      return;

    ListNode currentNode = this;
    for (int i = 1; i < inputs.Count; i++) {
      currentNode.next = new ListNode(inputs[i]);
      currentNode = currentNode.next;
    }
  }
}

public class Solution
{
  public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
  {
    ListNode dummyListNode = new ListNode();
    ListNode currentNode = dummyListNode;
    int carry = 0;

    while (l1 != null || l2 != null || carry != 0)
    {
      int val1 = l1 != null ? l1.val : 0;
      int val2 = l2 != null ? l2.val : 0;
      int totalSum = val1 + val2 + carry;
      carry = (totalSum / 10);

      currentNode.next = new ListNode(totalSum % 10);
      currentNode = currentNode.next;

      if (l1 != null) l1 = l1.next;
      if (l2 != null) l2 = l2.next;
    }

    return dummyListNode.next;
  }
}