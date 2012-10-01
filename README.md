decrypt-toolbox
===============

Various .NET 4 tools that try to reverse a hashed (MD5, SHA1...) passwords (*decrypt* it) by trying the most likely passwords first. This set of tools hopes to draw awareness on current good and bad passwords.


Good passwords?
---------------

  - Everyone reading this should know a 7 characters long passwords is cracked very quickly no matter the characters. Example: "fP0$d2".
  - Everyone reading this also should know that any single word from a dictionary or common passwords like number sequences are very weak. Example: "encryption".
  - Many of you probably think that 'P4ssw0rd' is much safer than 'password', well it's kind of better just not *that* much.
  - Some of you think that a simple long password is good.

This tool's final goal is to help us all understand how we can have long and simple to remember passwords that are hard to generate without exhaustive generation.

  - Is a sentence safe? Example: "My name is: Werner Beroux."
  - Is a repeated character safe? Example: "password______________"

In other words, is there a way to generate first all passwords that are likely to be used by humans before trying all other possible password combinations?
When contributing to this project, please keep this as your goal.


Concept
-------

Using various independant tools piped together to adapt to any case without duplicating code.
All those tools are mostly to check against some brute force attack.

General form:

    dtGenerate... | dtTransform... | dtTranform... | dtHash | grep 5F4DCC3B5AA765D61D8327DEB882CF99

  - Generator: dtGenerate* - should be [string generation](http://en.wikipedia.org/wiki/String_generation) tools (taking no stdin usually).
  - Transformation: dtTransform* - should be middle tools that take from stdin and usually generate more stdout.
  - Hash: dtHash - hashes the inputs and outputs their hashed values.

Line order is usually not important so some tools may change it. Most tools also take advantage of multithreaded environments.


Usage Example
-------------

    <dictionary.txt dtTransformCombine | dtTransformCombine | dtTransformAlternatives | dtHash md5 | grep 5F4DCC3B5AA765D61D8327DEB882CF99

Supposing `dictionary.txt` contains only words in lower case, *dtTransformCombine* will first try each word and then combination or word, and then *dtTransformAlternatives* will not only ouput all possible cases but also some numeric alternatives which people took as being much more secure (so you can test how much more secure it is). Finally *dtHash* will hash that string and *grep* (or whatever tool you have) will compare it against some hashed value.


Tools in this Suite
-------------------

### dtGenerateRandomRegexMatch

*Reverse regular expression*, a port of [Xeger](http://code.google.com/p/xeger/) done in the [Fare](https://github.com/Vassiliki/Fare) project.
It generates random strings matching the given regular expression. Useful for some testing but not very useful in practice.

Example: `dtGenerateRandomRegexMatch [a-z]{5,12} | ...`


### dtTransformAlternatives

A tool that outputs all possible variations of each character for each input.

Example for `echo AE | dtTransformAlternatives`:

    ae
    aE
    a3
    Ae
    AE
    A3
    4e
    4E
    43


### dtTransformCombine

A tool that combines all possible sets of all input words.

Does *not* work if the inputs are unlimited like with `dtGenerateRandomRegexMatch` or such. 

Example for 2 input lines 'Foo' and 'Bar':

    Foo
    Bar
    FooBar
    BarFoo


### dtHash

Hash the inputs. Supports MD5 and SHA1 among others. This is probably the slowest pipe (obviously).

Example: `echo password | dtHash MD5`


### dtBenchmarkOutputs

Estimate how many rows per second a program can output.

Example: `... | dtBenchmarkOutputs`


### dtBenchmarkInputs

Estimate how many rows per second a program can process from the inputs.

Example: `dtBenchmarkInputs | ...`


Other Common Tools
------------------

Cygwin, MSysGit and others commonly have those useful transformation tools:

  - `... | sed 's/^/FOO/' | ...` - Linux (Cygwin and Msysgit) tool that can be used to add salt.
  - `... | grep -F '5F4DCC3B5AA765D61D8327DEB882CF99'` - Linux (Cygwin and Msysgit) string matching.
  - `... | find "5F4DCC3B5AA765D61D8327DEB882CF99"` - MSDOS string matching.


Development
-----------

    $ git clone --recurse-submodules ...

You'll need:

  - Visual Studio 2010 or later
  - .NET 4
