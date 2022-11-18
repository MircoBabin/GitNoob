# Committer configuration file

Via commitname-settings-via-filename in [project ini](ProjectConfigurationFile.md "Project Configuration File") this file can be specified. The purpose of this file is to seperatly specify committer settings. This can be useful from within a Virtual Machine. Store the Committer.ini file on the host, and share this directory inside the Virtual Machine. This way multiple persons using copies of the same VM can each configure their own name for committing.

This file must have commitname and commitemail settings. If one is missing the entire file will be ignored.

# section \[GitNoob\]
In the project configuration the section GitNoob can contain the settings:

| Name | Type | Description |
| ---- | ---- | ----------- |
| commitname | Value | What value should the committer name have. E.g. "Mirco Babin". |
| commitemail | Value | What value should the committer email have. E.g. "mirco@..." |
| commitname-settings-clear-on-exit | Boolean | (Optional) when closing the working directory form, reset the git user.name and user.email to nothing. e.g. commitname-settings-clear-on-exit=true |

```
[gitnoob]
commitname=Mirco Babin
commitemail=mirco@...
;commitname-settings-clear-on-exit=true
```
