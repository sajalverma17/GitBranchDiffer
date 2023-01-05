# Git Branch Differ

Git Branch Differ is a Solution Explorer Filter which, when applied, only shows those files in your Solution Explorer that were Added/Modified/Renamed in your working branch as compared to a "base" branch of your choice, typically master or main.

After the filter is applied, you can open a diff for any file from Solution Explorer to view what changed.

Supports VS2019 and VS2022

To download for VS2019, go to https://marketplace.visualstudio.com/items?itemName=SajalVerma.GitBranchDiffer

## Usage

1. Set the "base" branch or "base" commit's SHA to compare your working branch with. <br>Go to: Tools -> Options -> Git Branch Differ -> Branch or Commit To Diff Against.

![image](https://user-images.githubusercontent.com/25904133/210828164-b4af9a6c-2bc0-40d6-8a9c-a6837c6f2210.png)

2. Click the `Branch Diff Filter` in the dropdown of Solution Explorer Filters. <br>This filters the Solution Explorer such that only files that were Added/Modified/Renamed in the working branch are shown in your Solution Explorer hierarchy.

![image](https://user-images.githubusercontent.com/25904133/121787246-4b76ba00-cbc5-11eb-8033-7b06d92079d5.png)

3. Once the filter is applied, right-click on a file in Solution Explorer, then click `Open Diff With Base`. <br>This opens a new tab in Visual Studio, showing you the diff of the file's content in the working branch compared to it's content in the "base" branch/commit.

![image](https://user-images.githubusercontent.com/25904133/121787519-c8566380-cbc6-11eb-9dd2-d378a9f61775.png)

## Motivation

The extension was created to make reviewing pull requests in .NET projects easier, as jumping/navigating via code (classes, methods, properties and their references inside the solution) is possible when the comparison view is "native" to Visual Studio, as opposed to only text-based code comparison on platforms like Github.

![120868239-e3582080-c593-11eb-9c62-d68d1a17b05f](https://user-images.githubusercontent.com/25904133/120868781-118a3000-c595-11eb-85f1-bd93a0116a52.png)

Note that GitBranchDiffer filter does not support displaying files that were deleted in the working branch.

# Report Issue

To report a bug, please use the [Issue Tracker](https://github.com/sajalverma17/GitBranchDiffer/issues/new?assignees=&labels=bug&template=bug-report.md&title=)