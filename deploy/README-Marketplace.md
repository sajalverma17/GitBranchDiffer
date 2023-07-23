# Git Branch Differ

Git Branch Differ is a Solution Explorer Filter which, when applied, only shows those files in your Solution Explorer that were Added/Modified/Renamed in your working branch as compared to a "reference" branch/commit/tag of your choice, typically a master/main branch.

After the filter is applied, you can open a diff for any file from Solution Explorer to view what changed.

Supports VS2019 and VS2022

To download for VS2019, go to https://marketplace.visualstudio.com/items?itemName=SajalVerma.GitBranchDiffer

## Usage

1. Click the `Branch Diff Filter` in the Solution Explorer Filters dropdown to only see files that were changed in the working branch. <br>By default, the extension will use master/main or the first branch found in repo as the Git reference to compare with.

![image](https://user-images.githubusercontent.com/25904133/121787246-4b76ba00-cbc5-11eb-8033-7b06d92079d5.png)

2. Once the filter is applied, right-click on a file in Solution Explorer, then click `Open diff with Git reference`. <br>This opens a new tab, displaying the diff of the file's content in working branch compared to the file's content in reference branch/commit/tag.

![image](https://github.com/sajalverma17/GitBranchDiffer/assets/25904133/2b45fd37-503f-4870-a6c4-055617edd0fd)

3. You can set your own Git reference from the Git reference configuration window. <br>Click the button shown in the screenshot below. The button only appears in Solution Explorer toolbar _after_ the filter has been applied.

![image](https://github.com/sajalverma17/GitBranchDiffer/assets/25904133/05e7cb62-971e-4c74-885f-4f75fe5f707f)

4. Type in the branch name/commit-SHA/tag name in the text box at the bottom, or select a reference from the list of branches/recent commits/tags, then press OK. Re-apply the `Branch Diff Filter` filter in Solution Explorer.

![image](https://github.com/sajalverma17/GitBranchDiffer/assets/25904133/9a98b4b3-4c12-4114-9e6c-26f1b92e05f0)

## Motivation

The extension was created to make reviewing pull requests in .NET projects easier, as jumping/navigating via code (classes, methods, properties and their references inside the solution) is possible when the comparison view is "native" to Visual Studio, as opposed to only text-based code comparison on platforms like Github.

![120868239-e3582080-c593-11eb-9c62-d68d1a17b05f](https://user-images.githubusercontent.com/25904133/120868781-118a3000-c595-11eb-85f1-bd93a0116a52.png)

Note that GitBranchDiffer filter does not support displaying files that were deleted in the working branch.

# Report Issue

To report a bug, please use the [Issue Tracker](https://github.com/sajalverma17/GitBranchDiffer/issues/new?assignees=&labels=bug&template=bug-report.md&title=)