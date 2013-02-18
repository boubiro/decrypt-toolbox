decrypt-toolbox
===============

Various .NET 4 tools that try to reverse a hashed (MD5, SHA1...) passwords (*decrypt* it) by trying the most likely passwords first.

This tool's goal is to see which kind of passwords can quickly be checked via a computer and hence should not be used. It should help us all understand how we can have a good password that remains simple to remember.

In other words, is there a way to generate first all passwords that are likely to be used by humans before trying all other possible password combinations?
When contributing or using this project, please keep this as your goal.


Concept
-------

Using various independant tools piped together to adapt to any case without duplicating code.

Example:

    <dictionary.txt dtTransformCombine | dtTransformAlternatives | dtHash md5 | dtFind 5f4dcc3b5aa765d61d8327deb882cf99

Supposing `dictionary.txt` contains only words in lower case, *dtTransformCombine* will first try each word and then combination or word, and then *dtTransformAlternatives* will not only ouput all possible cases but also some numeric alternatives which people took as being much more secure (so you can test how much more secure it is). Finally *dtHash* will hash that string and *dtFind* will compare it against some hashed value.

See more details on the [decrypt-toolbox Wiki](https://github.com/wernight/decrypt-toolbox/wiki).


Download
--------

Prebuilt Windows binaries are available from http://download.beroux.com/decrypt-toolbox.zip


Development
-----------

    $ git clone --recurse-submodules ...

You'll need:

  - Visual Studio 2010 or later
  - .NET 4
