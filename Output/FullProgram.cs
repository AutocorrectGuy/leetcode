using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Web.Script.Serialization;

public class Program
{
  public static void Main()
  {
    TestingFramework.TestMany();
  }
}

/**
 -- Custom simple testing framework --
    1. Checks if class and method exists. Must be static method in this case.
    2. Tries to invoke the method from class.
    3. Stringifies result and expected output.
    4. Compares both strings.
    5. Logs results
**/
public class TestingFramework
{
    /**
     -- This path points to the PowerShell script that will be executed.
        When PowerShell runs this file, it treats the script's directory as the working directory.
    **/
    private static readonly string _ps1ScriptPath = "./index.ps1";

    /**
     -- Class name for the leetcode problems method. Must allways remain as "Solution".
    **/
    private static string _problemClassName = "Solution";

    /**
     -- Runs tests by selecting class and method names and providing test cases
    **/
    public static void TestMany()
    {
        Type classType = Type.GetType(_problemClassName);

        if (classType == null)
        {
            Console.WriteLine(String.Format("Error: Class \"{0}\" not found", _problemClassName));
            return;
        }

        string methodName = FindProblemMethodName();
        var method = classType.GetMethod(methodName);

        if (method == null)
        {
            Console.WriteLine(String.Format("Error: Method \"{0}\" not found", methodName));
            return;
        }
        
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(string.Format("-- Testing results of problem \"{0}\" --", methodName));
        Console.ResetColor();
        Console.WriteLine("");
        
        // At this point powershell script should have selected the corresponding test cases
        // so the static `TestCases` class is being used 
        for (int i = 0; i < TestCases.All.Count; i++)
        {
            TestCase testCase = TestCases.All[i];

            object[] args = testCase.args;
            string argsStr = _stringify(args);
            object result = null;

            try
            {
                object target = !method.IsStatic
                    ? Activator.CreateInstance(classType)
                    : null;
                result = method.Invoke(target, args);
            }
            catch
            {
                Console.WriteLine(String.Concat(
                    "Failed to invoke method \"",
                    methodName,
                    "\" from class \"",
                    _problemClassName,
                    "\" with arguments: ",
                    argsStr
                ));
                continue;
            }

            string resultStr = _stringify(result);
            string expectedStr = _stringify(testCase.expected);
            bool passed = resultStr == expectedStr;

            Console.Write(String.Format("\nTest {0}: ", i));
            if (passed)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write("[successfull]");
            }
            else
            {
                Console.Write("[failed]");
                Console.BackgroundColor = ConsoleColor.Red;
            }
            Console.ResetColor();
            Console.WriteLine(String.Format("\nExpected: {0}\nResult: {1}", expectedStr, resultStr));
        }

    }

    private static readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

    /**
        -- Converts any input to text string
    **/
    private static string _stringify(object value)
    {
        return _serializer.Serialize(value);
    }

    /**
     -- Returns method name which is going to be ran. Problem name corresponds to 
        static method name that will be ran e.g. `./Problems/1_TwoSum.cs` would 
        correspond to method from `Solution` class method `TwoSum`. The first digit 
        and underscore is removed from file name to obtain the method name.
    **/
    private static string FindProblemMethodName()
    {
        Regex regex = new Regex(@"(?:\d+_)(.+)(?:.cs)");

        using (StreamReader sr = new StreamReader(_ps1ScriptPath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Match match = regex.Match(line);
                if (match.Success)
                    return match.Groups[1].Value;

            }
        }

        throw new Exception(@"Problem method name could not be extracted from index.ps1.
            Check if valid file path is stored in `index.ps1` variable `[string] $problemFilePath`.");
    }
}

/**
 -- Custom testcase model --
    Arguments allways must be stored in array.
    Unknown types and expected result, so they are stored as objects.
**/
public class TestCase
{
    public object[] args;
    public object expected;

    public TestCase() { }

    public TestCase(object[] args, object expected)
    {
        this.args = args;
        this.expected = expected;
    }
}
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

