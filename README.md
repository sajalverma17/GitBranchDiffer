# GitBranchDiffer
A Visual Studio 2019 Plugin that displays source files Added/Modified/Renamed between a base branch you choose to diff against, and the current working branch in your Git repo.
The Added/Modified/Renamed files are displayed in the Solution Explorer once the "Show diff in Solution" filter is applied, and clicking these files opens a diff of each file.

The plugin was written to hopefully make reviewing changes done by other .NET developers easier, and aims to simulate a Pull Request/Change Log "natively" inside Visual Studio. As Visual Studio provides look-ups of references of variables, methods or classes in Document Windows of classes, viewing a diff of a class inside a Visual Studio Comparison window is more convenient than a regular text based diff.   

GitBranchDiffer does not support Deleted-file changes, as files deleted in working branch can not be shown in Solution Explorer, even though deleted files can be part of the branch diff generated by GitBranchDiffer.