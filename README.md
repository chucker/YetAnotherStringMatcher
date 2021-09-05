# YetAnotherStringMatcher
 
YetAnotherStringMatcher library tries to make string matching easier, more readable while being somewhat expressive.

It tries to replace Regex to some extent, mostly when it comes to relatively basic cases.

For now it is in very early version and apart from the errors, then it may lack of "obvious things" and be kinda inconsistent in its APIs - mostly due to maintainer being retard.

Source Code: https://github.com/xd-loler/YetAnotherStringMatcher/tree/master/src/Core

Tests: https://github.com/xd-loler/YetAnotherStringMatcher/blob/master/src/Tests/BasicTests.cs

## How to install

* .NET CLI: `dotnet add package YetAnotherStringMatcher --version 1.0.1`

* Package Manager: `Install-Package YetAnotherStringMatcher -Version 1.0.1`

* Package Reference: `<PackageReference Include="YetAnotherStringMatcher" Version="1.0.1" />`

* Nuget.org URL: https://www.nuget.org/packages/YetAnotherStringMatcher/

## Some random examples, more in specific API description below

```csharp
// Polish Postal Code
var input = "12-345";

var result = new Matcher(input)
                 .MatchDigitsOfLength(2)
                 .Then("-")
                 .ThenDigitsOfLength(3)
                 .Check();

Assert.True(result.Success);
```

```csharp
var matcher = new Matcher("[2021-09-05] ERROR: Message1! Exception!")
                  .Match("[2021-09-05]")
                  .ThenAnything()
                  .Then("Exception!")
                  .Check();
				  
Assert.True(matcher.Success);
```

```csharp
var input = new List<string>
{
	"[2021-09-05] ERROR: Message1",
	"[2021-09-05] WARNING: Message1",
	"[2021-09-05] INFO: Message1",
	"[2021-09-07] WARNING: Message1",
};

var pattern = new Matcher()
				  .Match("[2021-09-05] ")
				  .ThenAnyOf("WARNING:", "ERROR:");

Assert.True(pattern.Check(input[0]).Success);
Assert.True(pattern.Check(input[1]).Success);

Assert.False(pattern.Check(input[2]).Success);
Assert.False(pattern.Check(input[3]).Success);
```

```csharp
var matcher = new Matcher("Apple Watermelon")
                  .MatchAnyOf("Apple", "Banana")
                  .Then(" ")
                  .ThenAnyOf("Giraffe", "melon", "Watermelon")
                  .Check();

Assert.True(matcher.Success);
```

```csharp
var matcher = new Matcher("TEST")
                  .Match("test").IgnoreCase()
                  .Check();

Assert.True(matcher.Success);
```

```csharp
// Sample Phone Number / Reusable Pattern
var input = new List<string> { "+123 345 67 89", "+1424 345 67 89" };

var pattern = new Matcher()
                  .Match("+")
                  .ThenDigitsOfLength(3)
                  .Then(" ")
                  .ThenDigitsOfLength(3)
                  .Then(" ")
                  .ThenDigitsOfLength(2)
                  .Then(" ")
                  .ThenDigitsOfLength(2);

Assert.True(pattern.Check(input[0]).Success);
Assert.False(pattern.Check(input[1]).Success);
```


# Avaliable APIs:

**Match / Then** - tries to match exact string

```csharp
var matcher = new Matcher("ERROR: Exception 1...")
                  .Match("ERROR: Exception ")
                  .ThenAnyOf("1", "2", "3")
                  .Check();

Assert.True(matcher.Success);
```

___


**MatchAnyOf / ThenAnyOf** - tries to match longest possible element of list handed as parameter.

```csharp
var matcher = new Matcher("Apple Watermelon")
                  .MatchAnyOf("Apple", "Banana")
                  .Then(" ")
                  .ThenAnyOf("Giraffe", "melon", "Watermelon")
                  .Check();

Assert.True(matcher.Success);
```
___

**MatchAnything / ThenAnything** - matches anything non empty

```csharp
var matcher = new Matcher("[2021-09-05] ERROR: Message1! Exception!")
                  .Match("[2021-09-05]")
                  .ThenAnything()
                  .Then("Exception!")
                  .Check();
				  
Assert.True(matcher.Success);
```

**This one will FAIL:**
```csharp
var matcher = new Matcher("12")
                  .Match("1")
                  .ThenAnything()
                  .Then("2")
                  .Check();
				  
Assert.False(matcher.Success);
```

___

**MatchAnythingOfLength / ThenAnythingOfLength** - matches anything that has expected length

```csharp
var matcher = new Matcher("01-000 London")
                  .Match("01")
                  .ThenAnythingOfLength(5)
                  .Then("London")
                  .Check();
				  
Assert.True(matcher.Success);
```

___

**MatchDigitsOfLength / ThenDigitsOfLength** - matches digits of expected length

```csharp
var matcher = new Matcher("01-000 London")
                  .MatchDigitsOfLength(2)
                  .Then("-")
                  .MatchDigitsOfLength(3)
                  .Check();
				  
Assert.True(matcher.Success);
```

**This one will FAIL:**

```csharp
var matcher = new Matcher("aa-000 London")
                  .MatchDigitsOfLength(2)
                  .Then("-")
                  .MatchDigitsOfLength(3)
                  .Check();

Assert.False(matcher.Success);
```

___

**MatchDigitsWithLengthBetween / ThenDigitsWithLengthBetween** - matches digits with length between [A...B]

```csharp
var input = new List<string> { "12-000 London", "2-00 Zurich" };

var pattern = new Matcher()
                  .MatchDigitsWithLengthBetween(1, 2)
                  .Then("-")
                  .MatchDigitsWithLengthBetween(1, 3);

Assert.True(pattern.Check(input[0]).Success);
Assert.True(pattern.Check(input[1]).Success);
```

**This one will FAIL:**

```csharp
var input = new List<string> { "121-000 London", "44-0011 Zurich" };

var pattern = new Matcher()
                  .MatchDigitsWithLengthBetween(1, 2)
                  .Then("-") // because it isn't on 3rd position
                  .MatchDigitsWithLengthBetween(1, 3);

Assert.False(pattern.Check(input[0]).Success);
```

___

**MatchSymbolsOfLength / ThenSymbolsOfLength** - matches symbols that are provided

```csharp
var input = new List<string> { "+-123" };

var pattern = new Matcher()
                  .MatchSymbolsOfLength(new[] { '+','-'}, 2)
                  .MatchDigitsWithLengthBetween(1, 3)
                  .NoMore();

Assert.True(pattern.Check(input[0]).Success);
```

**This one will FAIL:**

```csharp
var input = new List<string> { "+-123" };

var pattern = new Matcher()
                  .MatchSymbolsOfLength(new[] { '!','@'}, 2)
                  .MatchDigitsWithLengthBetween(1, 3)
                  .NoMore();

Assert.False(pattern.Check(input[0]).Success);
```

___

**MatchCustomOfLength / ThenCustomOfLength** - matches symbols that satisfy given predicate

```csharp
Func<char, CheckOptions, bool> func = (char c, CheckOptions _) => char.IsUpper(c);

var matcher = new Matcher("ABC")
                  .MatchCustomOfLength(func, 3)
                  .Check();

Assert.True(matcher.Success);
```

**This one will FAIL:**

```csharp
Func<char, CheckOptions, bool> func = (char c, CheckOptions _) => char.IsUpper(c);

var matcher = new Matcher("abc")
                  .MatchCustomOfLength(func, 3)
                  .Check();

Assert.False(matcher.Success);
```

___

**NoMore** - string must end here

```csharp
var matcher = new Matcher("TEST123")
                  .Match("TEST")
                  .NoMore()
                  .Check();

Assert.False(matcher.Success);
```

___

**IgnoreCase** - Ignores Case

```csharp
var matcher = new Matcher("TEST")
                  .Match("test").IgnoreCase()
                  .Check();

Assert.True(matcher.Success);
```

___

**IgnoreCaseForAllExisting** - Ignores Case for all previous requirements

```csharp
var matcher = new Matcher("TEST_ABC")
                  .Match("test_")
                  .Match("abc")
                  .IgnoreCaseForAllExisting()
                  .Check();

Assert.True(matcher.Success);
```

___

**ThenCustom** - You can provide your own implementation of IRequirement interface.

___

# To Do List

* Improve error handling / add some additional information why given step failed

* Support more complex operations like ThenAnyOf(IRequirement, IRequirement, IRequirement...)