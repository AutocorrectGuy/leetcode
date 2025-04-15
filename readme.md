# Leetcode C# Playground

Run and test C# Leetcode problems using PowerShell.

### Example:

Solution code from leetcode execise

```cs
using System;
using System.Collections.Generic;

public class Solution
{
  public int[] TwoSum(int[] nums, int target)
  {
    Dictionary<int, int> map = new Dictionary<int, int> { { target - nums[0], 0 } };
    
    for (int i = 1; i < nums.Length; i++) {
      if (map.ContainsKey(nums[i]))
        return new[] { map[nums[i]], i };
      else
        map[target - nums[i]] = i;
    }
    
    return new int[0];
  }
}
```

---

### Tests data
```cs
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

```
### Test results
```ps1
-- Testing results of problem "TwoSum" --

Test 0: [successfull]
Expected: {"val":7,"next":{"val":0,"next":{"val":8,"next":null}}}
Result: {"val":7,"next":{"val":0,"next":{"val":8,"next":null}}}

Test 1: [successfull]
Expected: {"val":0,"next":null}
Result: {"val":0,"next":null}

Test 2: [successfull]
Expected: {"val":8,"next":{"val":9,"next":{"val":9,"next":{"val":9,"next":{"val":0,"next":{"val":0,"next":{"val":0,"next":{"val":1,"next":null}}}}}}}}
Result: {"val":8,"next":{"val":9,"next":{"val":9,"next":{"val":9,"next":{"val":0,"next":{"val":0,"next":{"val":0,"next":{"val":1,"next":null}}}}}}}}
```

## How it works

Problems are stored in the `./Problems` directory.

To select which problem to run, change the value of the variable at the top of `index.ps1`:

`[string] $problemFilePath = "./Problems/1_TwoSum.cs"`

The corresponding test file must follow the same naming convention but use the `.tests.cs` suffix:

- `./Problems/1_TwoSum.cs` – main problem file  
- `./Problems/1_TwoSum.tests.cs` – test cases for the problem

You can leave `$testCasesFilePath` empty. The script will automatically look for a matching `.tests.cs` file based on the problem file name.

## Running

Run the script using PowerShell:

`powershell -ep Bypass -File index.ps1`

You can also use `-ep Unrestricted`. Both options work the same.

## How it works under the hood

- `index.ps1` reads all `.cs` files from `./CSharp` and combines them with the selected problem and test files.
- All `using` directives (headers) are deduplicated and separated from the main code.
- The combined code is compiled in memory using `Add-Type`.
- The `Program.Main()` method is executed, which runs all test cases.
- Test execution is handled by a simple in-house framework defined in `TestingFramework`.

If something breaks or you want to debug, the full generated `.cs` file is written to:

`./Output/FullProgram.cs`

## Conventions

- Every problem must be in a static class named `Solution`.
- The method name is derived from the problem filename. For example, `2_TwoSum.cs` → `TwoSum` method.
- Tests must be declared in a static `TestCases` class using `TestCase` instances.
- Arguments are passed as object arrays, and results are compared using stringified JSON.
