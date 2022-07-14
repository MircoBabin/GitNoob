# Project configuration file xxx.ini

- In the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File") this file is referenced under the \[projects\] section.

# section \[GitNoob\]
In the project configuration the section GitNoob can contain the settings:

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | Path | Name of the project. E.g. "My marvelous project". |
| icon | Path | (Optional) Filename of icon to be used in the workingdirectory form. E.g. "marvelous.ico". |
| type | Value | (Optional) Project type. e.g. "Php" or "PhpLaravel7", "PhpLaravel8", "PhpLaravel9" |
| | | |
| apache | Value | (Optional) project apache settings name. E.g. "global" then there should also be a section \[apache-global\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| apachePath | Path | (Optional) Path to Apache binaries. %apachePath%\\bin should contain httpd.exe. e.g. "%binPath%\\apache". |
| apacheConf | Path | (Optional) Path to Apache conf template file. |
| apacheUseSsl | Boolean | (Optional) default false. Enable https, specify certificate via apacheSslCertificateKeyFile, apacheSslCertificateFile and apacheSslCertificateChainFile. e.g. apacheUseSsl=true |
| apacheSslCertificateKeyFile | Path | (Optional) path to Apache certificate key file (SSLCertificateKeyFile). |
| apacheSslCertificateFile | Path | (Optional) path to Apache certificate file (SSLCertificateFile). |
| apacheSslCertificateChainFile | Path | (Optional) path to Apache certificate chain file (SSLCertificateChainFile). |
| | | |
| php | Value | (Optional) project php settings name. E.g. "5.6.40" then there should also be a section \[php-5.6.40\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| phpPath | Path | (Optional) Path to Php binaries. %phpPath% should contain php.exe. e.g. "%binPath%\\php-5.6.40". |
| phpIni | Path | (Optional) Path to Php ini template file. |
| | | |
| ngrok | Value | (Optional) project ngrok settings name. E.g. "global" then there should also be a section \[ngrok-global\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| ngrokPath | Path | (Optional) Path to Ngrok binaries. %ngrokPath% should contain ngrok.exe. e.g. "%binPath%\\ngrok". |
| ngrokPort | Integer | (Optional) Port on localhost where the Ngrok dashboard is running. e.g. the default port "4040". |
| ngrokAgentConfigurationFile | Path | (Optional) Ngrok Agent Configuration File (ngrok.yml). Will be specified as commandline parameter "--config=ngrokAgentConfigurationFile". |
| ngrokAuthToken | String | (Optional) Ngrok authtoken to use. Will be set as environment variable NGROK_AUTHTOKEN. The authtoken can also be set in the Ngrok Agent Configuration File (ngrok.yml). |
| ngrokApiKey | String | (Optional) Ngrok apikey for https://api.ngrok.com. Will be set as environment variable NGROK_API_KEY. |
| | | |
| smtpserver | Value | (Optional) project smtpserver settings name. E.g. "papercut" then there should also be a section \[smtpserver-papercut\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| smtpServerExecutable | Path | (Optional) Smtp server executable. |
| | | |
| origin | Value | (Optional) project url to be used for "git clone". e.g. "https://github.com/MircoBabin/GitNoob.git". |
| mainbranch | Value | (Optional) main branch name. e.g. "master". |
| commitname | Value | (Optional) what value should the committer name have. E.g. "Mirco Babin". |
| commitemail | Value | (Optional) what value should the committer email have. E.g. "mirco@..." |
| | | |
| homepage | Value | (Optional) url to start for the homepage. e.g. "https://localhost:7777" or "https://localhost:%port%" |

```
[gitnoob]
name=My marvelous project
type=PhpLaravel7
icon=marvelous.ico

origin=https://github.com/MircoBabin/GitNoob.git

php=7.2.31
```

# sections \[working-\]
This section defines a working directory. 

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | Path | Name of the working directory. E.g. "Work In Progress". |
| path | Path | Git root directory. Does not have to exist, when unexisting a "git clone" will be performed. E.g. "%prjPath%\marvelous". |
| icon | Path | (Optional) Filename of icon to be used in the workingdirectory form. E.g. "marvelous.ico". |
| image | Path | (Optional) Filename of image to be used in the workingdirectory form. The max image size is 384px width by 96px height (ratio 4:1). Smaller images will be shown as is, bigger images will be scaled. E.g. "marvelous.png". |
| imagebackgroundcolor | Value | (Optional) a html color reference for the image background. E.g. "#000000" for black. |
| type | Value | (Optional) Project type. e.g. "Php" or "PhpLaravel7", "PhpLaravel8", "PhpLaravel9" |
| | | |
| apache | Value | (Optional) project apache settings name. E.g. "global" then there should also be a section \[apache-global\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| apachePath | Path | (Optional) Path to Apache binaries. %apachePath%\\bin should contain httpd.exe. e.g. "%binPath%\\apache". |
| apacheConf | Path | (Optional) Path to Apache conf template file. |
| apacheUseSsl | Boolean | (Optional) default false. Enable https, specify certificate via apacheSslCertificateKeyFile, apacheSslCertificateFile and apacheSslCertificateChainFile. e.g. apacheUseSsl=true |
| apacheSslCertificateKeyFile | Path | (Optional) path to Apache certificate key file (SSLCertificateKeyFile). |
| apacheSslCertificateFile | Path | (Optional) path to Apache certificate file (SSLCertificateFile). |
| apacheSslCertificateChainFile | Path | (Optional) path to Apache certificate chain file (SSLCertificateChainFile). |
| | | |
| php | Value | (Optional) project php settings name. E.g. "5.6.40" then there should also be a section \[php-5.6.40\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| phpPath | Path | (Optional) Path to Php binaries. %phpPath% should contain php.exe. e.g. "%binPath%\\php-5.6.40". |
| phpIni | Path | (Optional) Path to Php ini template file. |
| | | |
| ngrok | Value | (Optional) project ngrok settings name. E.g. "global" then there should also be a section \[ngrok-global\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| ngrokPath | Path | (Optional) Path to Ngrok binaries. %ngrokPath% should contain ngrok.exe. e.g. "%binPath%\\ngrok". |
| ngrokPort | Integer | (Optional) Port on localhost where the Ngrok dashboard is running. e.g. the default port "4040". |
| ngrokAgentConfigurationFile | Path | (Optional) Ngrok Agent Configuration File (ngrok.yml). Will be specified as commandline parameter "--config=ngrokAgentConfigurationFile". |
| ngrokAuthToken | String | (Optional) Ngrok authtoken to use. Will be set as environment variable NGROK_AUTHTOKEN. The authtoken can also be set in the Ngrok Agent Configuration File (ngrok.yml). |
| ngrokApiKey | String | (Optional) Ngrok apikey for https://api.ngrok.com. Will be set as environment variable NGROK_API_KEY. |
| | | |
| smtpserver | Value | (Optional) project smtpserver settings name. E.g. "papercut" then there should also be a section \[smtpserver-papercut\] in the [root configuration file - GitNoob.ini](RootConfigurationFile.md "Root Configuration File"). |
| smtpServerExecutable | Path | (Optional) Smtp server executable. |
| | | |
| origin | Value | (Optional) url to be used for "git clone". e.g. "https://github.com/MircoBabin/GitNoob.git". |
| mainbranch | Value | (Optional) main branch name. e.g. "master". |
| commitname | Value | (Optional) what value should the committer name have. E.g. "Mirco Babin". |
| commitemail | Value | (Optional) what value should the committer email have. E.g. "mirco@..." |
| | | |
| port| Integer | (Optional) port on localhost to use for the webserver. e.g. "7777". |
| webroot | Path | (Optional) path relative to **path of the working directory** containing the webroot. E.g. "public". |
| homepage | Value | (Optional) url to start for the homepage. When not filled in "http://localhost:%port%/" will be used. e.g. "https://localhost:7777" or "https://localhost:%port%" |
| | | |
| workspace | Path | (Optional) workspace filename of the IDE. The file is opened via shell execute, can be from any editor/ide. e.g. "%myDocuments%\\workspace.marvelous.code-workspace" or "%gitRoot%\\src\\GitNoob.sln". |
| workspace-run-as-administrator | Boolean | (Optional) start workspace with "run as administrator" rights. e.g. workspace-run-as-administrator=true |

```
[working-wip]
name=Work In Progress
path=%prjPath%\marvelous
mainbranch=master
;icon=
image=marvelous.png
imagebackgroundcolor=#000000

port=6001
webroot=
workspace=%myDocuments%\workspace.marvelous.code-workspace
```

# full example

```
[gitnoob]
name=My marvelous project
type=PhpLaravel7
icon=marvelous.ico

origin=https://github.com/MircoBabin/GitNoob.git

php=7.2.31

[working-wip]
name=Work In Progress
path=%prjPath%\marvelous
mainbranch=master
;icon=
image=marvelous.png
imagebackgroundcolor=#000000

port=6001
webroot=
workspace=%myDocuments%\workspace.marvelous.code-workspace

[working-release]
name=Release hotfixes
path=%prjPath%\marvelous.release
mainbranch=release
;icon=
image=marvelous-release.png
imagebackgroundcolor=#FF0000

port=7001
webroot=
workspace=%myDocuments%\workspace.marvelous.release.code-workspace
```