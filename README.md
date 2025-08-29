# QuickTXTSplitter
A simple command line utility that splits text files at lines matching a given regular expression.
This was originally made to split text archives of the English Wikipedia that consist of multiple articles per file.

Arguments:
```
--source, -s: ....... Path to the directory containing the source files to read from. Defaults to the working directory.
--dest, -d: ......... Path to the directory into which the split files will be written. Defaults to the working directory.
--regex, -r: ........ Regular expression to match lines where splits will occur.
--capturingGroup, -c: Which capturing group within the regex contains the title for each file. Defaults to 0.
--prefix, -p: ....... String to prefix each filename with. Defaults to empty.
```

Given a regular expression, it reads each source file and attempts to start a new file at each line that matches that regex.
It generates a directory structure under the destination directory that breaks down the files by first two characters (or just the first character if there's only one).
The structure also includes a folder for non-Latin characters and for files that had no match to the regex (i.e. if the source file had no matches, or if there wasn't a match on the first line, the text before the first match would go here). Non-match filenames use a UUID for uniqueness.
For non-letter symbols (e.g. ```#```), those fall under the ```$``` category.

Suppose you used this regex:
```
/\[\[(.*)\]\]/g
```

And this source file:

```
here is some text
that does not match the regex
[[@a-symbol]]
some text
[[826]]
some more text
[[827]]
etc
[[#symbols]]
[[!symbols!]]
[[да-символы]]
[[hello world]]
[[hi there]]
```

With the prefix ```p-```.
If you called the function with ```QuickTXTSplitter -s ./Source/ -d ./Destination/ -r "/\[\[(.*)\]\]/g" -c 1 -p "p-"```, accordingly, the resulting tree structure would look like this (not showing empty folders for clarity, in reality every possible combination of ```0-9a-z$``` will exist):

```
.
└── ./Destination/
    ├── ./Destination/$/
    │   ├── ./Destination/$/$a/
    │   │   └── ./Destination/$/$a/p-@a-symbol.txt
    │   └── ./Destination/$/$s/
    │       ├── ./Destination/$/$s/p-#symbols.txt
    │       └── ./Destination/$/$s/p-!symbols!.txt
    ├── ./Destination/[!Foreign]/
    │   └── ./Destination/[!Foreign]/p-да-символы.txt
    ├── ./Destination/[!NoMatch]/
    │   └── ./Destination/[!NoMatch]/p-[!NoMatch]0e216c88-63f8-452d-80b6-cb90a87f00c5.txt
    ├── ./Destination/8/
    │   └── ./Destination/8/82/
    │       ├── ./Destination/8/82/p-826.txt
    │       └── ./Destination/8/82/p-827.txt
    └── ./Destination/h/
        ├── ./Destination/h/he/
        │   └── ./Destination/h/he/p-hello world.txt
        └── ./Destination/h/hi/
            └── ./Destination/h/hi/p-hi there.txt
```

```p-@a-symbol.txt``` would contain:
```
[[@a-symbol]]
some text
```
```p-да-символы.txt``` would contain
```
[[да-символы]]
```
And ```p-[!NoMatch]0e216c88-63f8-452d-80b6-cb90a87f00c5.txt``` would contain
```
here is some text
that does not match the regex
```
