Set-StrictMode -Version Latest

# -- This file path is used in `./Program.cs` to determine which .cs file to run and 
# which corresponding tests file to use. Therefore do NOT change the name of this variable.
[string] $problemFilePath = "./Problems/1_TwoSum.cs"

# -- Tests should be named with the same name as c# file and end with `.tests.cs`
# e.g. `./1_TwoSum.tests.cs`.
#
# -- If this variable `$testCasesFilePath` is not defined then the program will
# automatically try to find `tests` file that corresponds to the problem file name 
# e.g. for `./Problems/1_TwoSum.cs` program will search for `./Problems/1_TwoSum.tests.cs`,
# so don't worry if this variable is empty string. Tests will be tried to be provided
# dynamically.
[string] $testCasesFilePath = ""
[string] $coreProgramFilesPath = "./CSharp"

# -- Splits c# code file in 2 parts: headers and body.
# Has 2 sidefects: 
#   - adds to the list of `$headers`
#   - adds to `$bodySb` StringBuilder
function DisectScriptFile(
  [string] $path, 
  [System.Collections.ArrayList] $headers, 
  [System.Text.StringBuilder] $bodySb
)
{
  [System.IO.StreamReader] $sr = [System.IO.StreamReader]::new($path)
  # do not define type as `string`, because `$sr.readLine` returns `string` OR `$null`
  # if `$line` would be defined as `string` it would be impossible to read empty lines
  # because of type coercion - `$null` would be coerced as `""` (empty string) and 
  # we would run into infinite loop, so `[string | null]` is the type 
  $line = $null

  try
  {
    # collect headers
    while ($null -ne ($line = $sr.ReadLine()))
    {
      if (-not ($line -match "(?:\s*using\s+)([\w.]+)(?:\s*;\s*)"))
      {
        break;
      }
      
      if (-not $headers.Contains($Matches[1]))
      {
        $headers.Add($Matches[1]) | Out-Null
      }
    }

    while ($null -ne ($line = $sr.ReadLine()))
    {
      $bodySb.AppendLine($line) | Out-Null
    }
  } 
  catch
  {
    Write-Error "-- Failed tor read script from path `"$($filePath)`" --`nReason:`n$($_.Exception.Message)"
  }
  finally
  {
    $sr.Close()
    $sr.Dispose()
  }
}

function MakeCodeBundle(
  [string] $coreProgramFilesPath,
  [string] $problemFilePath,
  [string] $testCasesFilePath = "")
{
  # -- store all core script file paths --
  [string[]] $paths = Get-ChildItem -Path $coreProgramFilesPath -Filter "*.cs" | % { $_.FullName }
  

  if (-not $coreProgramFilesPath)
  {
    throw "-- No core scripts were found in directory `"$($coreProgramFilesPath)`" --"
  }

  # -- make a single script bundle (as text string) --
  # -- 1. Read script files line by line
  # -- 2. While reading, separate headers (with `using` statements) from code (body)
  # -- 3. Merge headers with body, return a single text string - full single script bundle

  [System.Collections.ArrayList] $headers = [System.Collections.ArrayList]::new()
  [System.Text.StringBuilder] $bodySb = [System.Text.StringBuilder]::new()
  
  # read core c# code files
  foreach ($filePath in $paths)
  {
    DisectScriptFile `
      -path $filePath `
      -headers $headers `
      -bodySb $bodySb
  }

  # read main leetcode problem file
  DisectScriptFile `
    -path $problemFilePath `
    -headers $headers `
    -bodySb $bodySb

  # read leetcode problem tests file. If tests are not not provided in
  # `$testCasesFilePath`, try to load them dynamically  
  try
  {
    Test-Path $testCasesFilePath 
  }
  catch
  {
    if ($problemFilePath -match "(.+)(?:.cs$)")
    {
      $testCasesFilePath = "$($Matches[1]).tests.cs"
    }
    else
    {
      throw "Failed to derive test file name from '$testCasesFilePath'"
    }
  }

  # read testcases file
  DisectScriptFile `
    -path $testCasesFilePath `
    -headers $headers `
    -bodySb $bodySb

  # -- Merge headers --
  [System.Text.StringBuilder] $headersSb = [System.Text.StringBuilder]::new() 
  foreach ($header in $headers)
  {
    $headersSb.AppendLine("using $($header);") | Out-Null
  }
  $headersSb.AppendLine() | Out-Null

  # -- Combine headers with body (rest of the code) --
  $headersSb.Append($bodySb.ToString()) | Out-Null

  return ($headersSb.ToString())
}

function RunScriptBundle([string] $bundledCode)
{
  try
  {
    Add-Type `
      -TypeDefinition $bundledCode `
      -ReferencedAssemblies System.Web.Extensions # System.Web.Script.Serialization

    [Program]::Main()
  }
  catch
  {
    Write-Error "X- Failed to compile assembly. Reason:`n$($_.Exception.Message)"
  }
}

$bundledCode = MakeCodeBundle `
  -coreProgramFilesPath $coreProgramFilesPath `
  -problemFilePath $problemFilePath `
  -testCasesFilePath $testCasesFilePath

# creates .cs file for debugging purposes if something from this pipeline didn't work 
Set-Content -Path "./Output/FullProgram.cs" -Value $bundledCode

RunScriptBundle -bundledCode $bundledCode