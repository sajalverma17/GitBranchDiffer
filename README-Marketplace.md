# Git Branch Differ

GitBranchDiffer is a Solution Explorer Filter which compares the working branch of your Git repo with the branch you choose to compare it with, and displays files that were Added/Modified/Renamed in the Solution Explorer Window. When the filter is applied on Solution Explorer, clicking a file will open a diff view of the file. 

## Features

* Click the "Show diff in solution" filter to only view list of files that were Added/Modified/Renamed in the checked-out branch compared to another branch in your Gir repo (usually, base branches like master/dev). 
  * You must set the "Branch To Diff Against" in Options -> Git Branch Differ -> Branch To Diff Against.

* Once the filter is appled, click the files in Solution Explorer to open a comparison window to view what changed in the file compared to the "Base Branch" version of the file.

* Supports Visual Studio 2019

![image](https://user-images.githubusercontent.com/25904133/118525755-d63bd480-b73f-11eb-884a-ddf86c63a70a.png)

![image](https://user-images.githubusercontent.com/25904133/118526577-ae00a580-b740-11eb-94a3-b3b3238c258e.png)

The plugin was written to make reviewing pull requests in .NET projects easier, as jumping/navigating code (of classes, methods, properties and their references inside the solution) is possible when the comparison view is "native" to Visual Studio, as opposed to text-based comparison of code.

Note that GitBranchDiffer filter does not support displaying files that were deleted in the working branch.