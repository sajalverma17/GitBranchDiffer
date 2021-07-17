# Git Branch Differ

Git Branch Differ is a Solution Explorer Filter which compares the working branch of your Git repo with the branch you choose to compare it with, and displays files that were Added/Modified/Renamed in the Solution Explorer Window.

After the filter is applied, you can open a comparison between the version of the file in working branch Vs. the version of file in the branch you choose to compare it with. 

## Features

* Click the "Branch Diff Filter" in the list of Solution Explorer Filters. This filters the Solution Explorer down to files that were Added/Modified/Renamed in the working branch compared to any branch in your Git repository you choose to diff against (usually, "Base Branches" like master/dev). 
  * You must set the "Branch To Diff Against" in Options -> Git Branch Differ -> Branch To Diff Against.

* Once the filter is applied, right-click on a file in Solution Explorer, click "Open Diff With Base" to open a comparison of the file's content in the working branch with that of the "Base Branch".

* Supports Visual Studio 2019

![image](https://user-images.githubusercontent.com/25904133/118525755-d63bd480-b73f-11eb-884a-ddf86c63a70a.png)

![image](https://user-images.githubusercontent.com/25904133/121787246-4b76ba00-cbc5-11eb-8033-7b06d92079d5.png)

![image](https://user-images.githubusercontent.com/25904133/121787519-c8566380-cbc6-11eb-9dd2-d378a9f61775.png)

![120868239-e3582080-c593-11eb-9c62-d68d1a17b05f](https://user-images.githubusercontent.com/25904133/120868781-118a3000-c595-11eb-85f1-bd93a0116a52.png)

The plugin was written to make reviewing pull requests in .NET projects easier, as jumping/navigating code (classes, methods, properties and their references inside the solution) is possible when the comparison view is "native" to Visual Studio, as opposed to text-based comparison of code.

Note that GitBranchDiffer filter does not support displaying files that were deleted in the working branch.

# Report Issue

To report a bug, please use the [Issue Tracker](https://github.com/sajalverma17/GitBranchDiffer/issues/new?assignees=&labels=bug&template=bug-report.md&title=)
