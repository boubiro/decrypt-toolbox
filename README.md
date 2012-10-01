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

    dtGenerate... | dtTransform... | dtTranform... | dtCompare...

  - Generator: dtGenerate* - should be [string generation](http://en.wikipedia.org/wiki/String_generation) tools (taking no stdin usually).
  - Transformation: dtTransform* - should be middle tools that take from stdin and usually generate more stdout.
  - Comparison: dtCompare* - should be decryption tools taking as input the dictionary (outputing nearly nothing).

Line order is usually not important so some tools may change it. Most tools also take advantage of multithreaded environments.


Usage Example
-------------

    <dictionary.txt dtTransformCombine | dtTransformCombine | dtTransformAlternatives | dtCompareHash md5 5f4dcc3b5aa765d61d8327deb882cf99

Supposing dictionary.txt contains only words in lower case, *dtTransformCombine* will first try each word and then combination or word and *dtTransformAlternatives* will not only ouput all possible cases but also some numeric alternatives which people took as being much more secure (so you can test how much more secure it is).

    dtGenerateRandomRegexMatch [a-z]{5-20} | dtTransformAlternatives | dtCompareHash md5 5f4dcc3b5aa765d61d8327deb882cf99

Supposing the dictionary failed, this second version will generate random alphabetic characters and then again their possible numeric counterparties. It's very close to a brute force with random inputs, so may not be the best example here.


Tools in this Suite
-------------------

### dtGenerateRandomRegexMatch

*Reverse regular expression*, a port of [Xeger](http://code.google.com/p/xeger/) done in the [Fare](https://github.com/Vassiliki/Fare) project.
It generates random strings matching the given regular expression.


### dtTransformAlternatives

A tool that outputs all possible variations of each character for each input.

Example for a single character 'é': é, É, e, E, 3.

Example for two characters 'AE': ae, aE, a3, Ae, AE, A3, 4e, 4E, 43.


### dtTransformCombine

A tool that combines all possible sets of all input words.

Does *not* work if the inputs are unlimited like with `dtGenerateRandomRegexMatch` or such. 

Example for 2 lines input 'Foo' and 'Bar':

  - Foo
  - Bar
  - FooBar
  - BarFoo


### dtCompareHash

Output only inputs from stdin which hashed value matches a given hashed parameter.


Other Common Tools
------------------

Cygwin, MSysGit and others commonly have those useful transformation tools:

  - `... | sed 's/^/FOO/' | ...` - can be used to add salt.


Development
-----------

    $ git clone --recurse-submodules ...

You'll need:

  - Visual Studio 2010 or later
  - .NET 4
