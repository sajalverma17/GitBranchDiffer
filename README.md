# Git Branch Differ

Git Branch Differ is a Solution Explorer Filter which compares the working branch of your Git repo with the branch you choose to compare it with, and displays files that were Added/Modified/Renamed in the Solution Explorer Window.

After the filter is applied, you can open a comparison between the version of the file in working branch Vs. the version of file in the branch you choose to compare it with. 

### Installation
Install the plugin from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=SajalVerma.GitBranchDiffer), or grab it from within Visual Studio's Manage Extensions window by searching "Git Branch Differ". 
You can also try the latest [CI build](https://github.com/sajalverma17/GitBranchDiffer/actions/workflows/ci-build.yml).
Open the latest CI run, download the artifact, unzip and run GitBranchDiffer.vsix.

[![Build_Status](https://github.com/sajalverma17/GitBranchDiffer/actions/workflows/ci-build.yml/badge.svg)](https://github.com/sajalverma17/GitBranchDiffer/actions/workflows/ci-build.yml)

## Features

* Click the "Branch Diff Filter" in the list of Solution Explorer Filters. This filters the Solution Explorer down to files that were Added/Modified/Renamed in the working branch compared to any branch in your Git repository you choose to diff against (usually, "Base Branches" like master/dev). 
  * You must set the "Branch To Diff Against" in Options -> Git Branch Differ -> Branch To Diff Against.

* Once the filter is applied, right-click on a file in Solution Explorer, click "Open Diff With Base" to open a comparison of the file's content in the working branch with that of the "Base Branch".

* Supports Visual Studio 2019 and Visual Studio 2022

![image](https://user-images.githubusercontent.com/25904133/118525755-d63bd480-b73f-11eb-884a-ddf86c63a70a.png)

![image](https://user-images.githubusercontent.com/25904133/121787246-4b76ba00-cbc5-11eb-8033-7b06d92079d5.png)

![image](https://user-images.githubusercontent.com/25904133/121787519-c8566380-cbc6-11eb-9dd2-d378a9f61775.png)

![120868239-e3582080-c593-11eb-9c62-d68d1a17b05f](https://user-images.githubusercontent.com/25904133/120868781-118a3000-c595-11eb-85f1-bd93a0116a52.png)

The plugin was written to make reviewing pull requests in .NET projects easier, as jumping/navigating code (classes, methods, properties and their references inside the solution) is possible when the comparison view is "native" to Visual Studio, as opposed to text-based comparison of code.

Note that GitBranchDiffer filter does not support displaying files that were deleted in the working branch.

## Contributions

All contributions welcome.
Any suggestions/ideas can be discussed, and possibly assigned freely to whomever interested in contributing.
To report a bug, please use the [bug template](https://github.com/sajalverma17/GitBranchDiffer/issues/new?assignees=&labels=bug&template=bug-report.md&title=)

### Build 

```txt
git clone --recurse-submodule https://github.com/sajalverma17/GitBranchDiffer.git
cd GitBranchDiffer
```

Open the `GitBranchDiffer.sln` and build the solution.

### Test
To test, use the `dotnet test` command on project inside the `\tests` directory. A [test repository](https://github.com/sajalverma17/GitBranchDiffer-TestAsset) is used to run tests on, and is added as a submodule to this repository.

Remember to pull and update submodule everytime you fetch latest code from `master` branch.  

```txt
git pull --recurse-submodule origin master
```

### Run

To debug the extension in VS2019, open `GitBranchDiffer.sln` in VS2019 or VS2022 and set `GitBranchDiffer2019` as the Startup Project.  
To debug the extension in VS2022, open `GitBranchDiffer.sln` in VS2022 and set `GitBranchDiffer` as the Startup Project.   


When opened in VS2022, you must switch which VS version to debug in, after switching the Startup Projects:  
![image](https://user-images.githubusercontent.com/25904133/149598754-4d51c9f2-793d-476a-b976-cb10fd3a0d47.png)


A common issue I face everytime I debug in the Experimental Instance of VS after freshly cloning it on my disk is the [TypeInitializationException](https://stackoverflow.com/questions/59926253/libgit2sharp-dllnotfoundexception-unable-to-load-dll-git2-106a5f2) from the `LibGit2Sharp` dependency. If you run into this, try again after deleting the `Extensions` folder found under this directory: `%LocalAppData%\Microsoft\VisualStudio\16.0_XXXXXXXXExp`. If you are debugging Experimental Instance of VS2022, the path would be `%LocalAppData%\Microsoft\VisualStudio\17.0_XXXXXXXXExp`. 
 

## Credits

Thanks to Thomas Koch [@tommes](https://github.com/tommes) for the bugfix

## Roadmap
* The plugin was designed for easier code reviews in Visual Studio, by comparing Git branches (a feature Visual Studio didn't have) and listing the changes in a Solution Explorer view. However, Microsoft added the [Git Branch Compare feature](https://devblogs.microsoft.com/visualstudio/visual-studio-17-1-preview-2-is-now-available/#be-more-productive-with-git) in Visual Studio 2022 (Version 17.2), so this plugin might not add much value for those who can upgrade to VS2022. With that said, I will continue to maintain it, fixing bugs and accepting PRs.
* Currently, GitBranchDiffer is not supported on versions older than Visual Studio 2019, highly reducing the target audience of the plugin since Git Branch Comparison is lacking in those versions. I don't have the ability to use older versions, and no experience developing Visual Studio plug-ins prior to this. Any contributions in this direction are highly appreciated!

