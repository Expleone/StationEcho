## 1. Installing LFS
For Git to be able to handle large files like Unity scene, textures, or models Git Large File Storage has to be installed on the server and by the user.

Download link:
https://git-lfs.github.com/

Installation is straightforward: 1) Run the downloaded .exe installer and 2) when finished, run in your repository command ***git lfs install***. 

## 2. git ssh-authentication agent on Win
You can enable authentication agent for auto-password in Git-bash: [git-ssh-auth-win-setup.md](./git-ssh-auth-win-setup.md). It is very usefull for **lfs**.

## 3. Unity Smart Merge
Unity serializes all assets into somewhat readable form, that can be manually merged when a conflict occurs, but it's tedious. Unity provides its own merge tool and you can specify it in the git config, that can be found in ***YOUR PROJECT/.git/config***. Simply add these following lines at the end of the file:

```c#
[merge]
tool = unityyamlmerge

[mergetool "unityyamlmerge"]
trustExitCode = false
cmd = 'PATH_TO_YOUR_UNITY_FOLDER\\Editor\\Data\\Tools\\UnityYAMLMerge.exe' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
```

If you use tools like SourceTree, TortoiseGit, or other GUI you can read additional information on this page https://docs.unity3d.com/Manual/SmartMerge.html.

