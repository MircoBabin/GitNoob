# Configuration

GitNoob has 3 configuration layers. Each layer can override the settings of the previous layer.

1) Global
2) Project
3) Working directory within a project


The final result for a working directory are the settings from Global possibly overridden by the Project settings and finally maybe overridden by the Working directory settings. The following settings must exist for a working directory:

| Name |  Description |
| ---- |  ----------- |
| path | Git root directory. Does not have to exist, when unexisting a "git clone" will be performed. |
| origin | Url to be used for "git clone". |
| mainbranch | Main branch name. |

For a Php or PhpLaravel7, PhpLaravel8, PhpLaravel9 working directory, the following settings also must exist:

| Name |  Description |
| ---- |  ----------- |
| type | "Php" or "PhpLaravel7", "PhpLaravel8", "PhpLaravel9". |
| phpPath | Path to Php binaries. |
| phpIni | Path to Php ini template file. |


## Configuration chapters

- [Root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File")
- [Project and working directory configuration File - xxx.ini](ProjectConfigurationFile.md "Project Configuration File")
- [Apache Conf template replacements](TemplateApacheConfReplacements.md "Apache Conf Replacements")
- [Php Ini template replacements](TemplatePhpIniReplacements.md "Php Ini Replacements")
