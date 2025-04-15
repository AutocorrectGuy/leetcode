using System;
using System.Collections.Generic;

public static class TestCases
{
  public static List<TestCase> All = new List<TestCase> {
    
    // First test case
    new TestCase {
      args = new object[] {
        new ListNode(new List<int> { 2, 4, 3 }),
        new ListNode(new List<int> { 5, 6, 4 })
      },
      expected = new ListNode(new List<int> { 7, 0, 8 })
    },

    // Second test case
    new TestCase
    {
      args = new object[] {
        new ListNode(new List<int> { 0 }),
        new ListNode(new List<int> { 0 })
      },
      expected = new ListNode(new List<int> { 0 })
    },

    // Third test case
    new TestCase
    {
      args = new object[] {
        new ListNode(new List<int> { 9, 9, 9, 9, 9, 9, 9 }),
        new ListNode(new List<int> { 9, 9, 9, 9 })
      },
      expected = new ListNode(new List<int> { 8, 9, 9, 9, 0, 0, 0, 1 })
    },
  };
}