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