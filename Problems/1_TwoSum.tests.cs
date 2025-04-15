using System;
using System.Collections.Generic;

public static class TestCases
{
  public static readonly List<TestCase> All = new List<TestCase>{
    new TestCase {
      args = new object[] {new int[] { 2, 7, 11, 15 }, 9},
      expected = new int[] { 0, 1 }
    },
    new TestCase {
      args = new object[] {new int[] { 3, 2, 4 }, 6},
      expected = new int[] { 1, 2 }
    },
    new TestCase {
      args = new object[] {new int[] { 3, 3 }, 6},
      expected = new int[] {0, 1}
    }
  };
};
