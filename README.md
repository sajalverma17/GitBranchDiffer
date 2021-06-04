# Git Branch Differ

GitBranchDiffer is a Solution Explorer Filter which compares the working branch of your Git repo with the branch you choose to compare it with, and displays files that were Added/Modified/Renamed in the Solution Explorer Window. When the filter is applied on Solution Explorer, clicking a file will open a diff view of the file. 

[![Build_Status](https://github.com/sajalverma17/GitBranchDiffer/actions/workflows/ci-build.yml/badge.svg)](https://github.com/sajalverma17/GitBranchDiffer/actions/workflows/ci-build.yml)

## Features

* Click the "Show diff in solution" filter to only view list of files that were Added/Modified/Renamed in the checked-out branch compared to another branch in your Gir repo (usually, base branches like master/dev). 
  * You must set the "Branch To Diff Against" in Options -> Git Branch Differ -> Branch To Diff Against.

* Once the filter is appled, click the files in Solution Explorer to open a comparison window to view what changed in the file compared to the "Base Branch" version of the file.

* Supports Visual Studio 2019

![image](https://user-images.githubusercontent.com/25904133/118525755-d63bd480-b73f-11eb-884a-ddf86c63a70a.png)

![image](https://user-images.githubusercontent.com/25904133/118526577-ae00a580-b740-11eb-94a3-b3b3238c258e.png)

The plugin was written to make reviewing pull requests in .NET projects easier, as jumping/navigating code (of classes, methods, properties and their references inside the solution) is possible when the comparison view is "native" to Visual Studio, as opposed to text-based comparison of code.

Note that GitBranchDiffer filter does not support displaying files that were deleted in the working branch.

# Installation

You can also install the latest [CI build](https://github.com/sajalverma17/GitBranchDiffer/actions/workflows/ci-build.yml).
Open the latest CI run, download the artifact, unzip, run GitBranchDiffer.vsix.

## Contributions

All contributions welcome. 
Any suggestions/ideas can be discussed, and possibly assigned freely to whomever interested in contributing.
To report a bug, please use the [bug template](https://github.com/sajalverma17/GitBranchDiffer/issues/new?assignees=&labels=bug&template=bug-report.md&title=)

### Build 

```txt
git clone https://github.com/sajalverma17/GitBranchDiffer.git
cd GitBranchDiffer
```

Open the `GitBranchDiffer.sln`, build and then run.

## Roadmap
* Currently, GitBranchDiffer is not supported on versions older than Visual Studio 2019 (highly reducing the target audience of the plugin). This is mostly because I do not have the ability to test on older versions, and no experience developing Visual Studio plug-ins prior to this. Any contributions in this direction are highly appreciated!
* The plugin is designed for code reviews on .NET, and further improvements are expected to be in that direction.
* Ubiquitous dev-tasks (for example, git-pull before applying GitBranchDiffer filter) are expected to be done by users via existing tools in VS/CLI and are to be kept out of this plugin.
* General improvements in code quality, eg. test coverage of BranchDiffer.Git library.

