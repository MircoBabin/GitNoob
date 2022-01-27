# Root configuration file GitNoob.ini

- In the My Documents folder a directory GitNoob should be manually created.
- Here the GitNoob.ini root configuration file must be manually created.

e.g. "c:\\Users\\Mirco\\Documents\\GitNoob\\GitNoob.ini".

Path values in this root configuration file are relative to the GitNoob.ini file. E.g. a relative path like "MyProject.ini" would result in "c:\\Users\\Mirco\\Documents\\GitNoob\\MyProject.ini".

## Alternative location
An alternative the root configuration filename can also be passed on the commandline as the first argument. 

e.g. c:\\Utils\\GitNoob\\GitNoob.exe "c:\\Projects\\GitNoob\\GitNoob.ini".

## Redirection
Also a redirection can be provided in the root configuration file. This prevents from having to use a commandline parameter.

The My Documents\\GitNoob\\GitNoob.ini contains the entry "loadRootConfigurationFrom" pointing to the alternative location. 

```
[gitnoob]
loadRootConfigurationFrom=c:\Projects\GitNoob\GitNoob.ini
```


# section \[GitNoob\]
In the root configuration the section GitNoob can contain the settings:

| Name | Type | Description |
| ---- | ---- | ----------- |
| prjPath | Path | (Optional) path to the projects root folder. Can be referenced in other configuration settings via %prjPath%. E.g. "c:\\Projects\\Webpage". |
| binPath | Path | (Optional) path to the binaries root folder. Can be referenced in other configuration settings via %binPath%. E.g. "c:\\Projects\\bin". |
| commitname | Value | (Optional) what value should the committer name have. E.g. "Mirco Babin". |
| commitemail | Value | (Optional) what value should the committer email have. E.g. "mirco@..." |
| apache | Value | (Optional) global apache settings name. E.g. "global" then there should also be a section \[apache-global\]. |
| php | Value | (Optional) global php settings name. E.g. "5.6.40" then there should also be a section \[php-5.6.40\]. |
| ngrok | Value | (Optional) global ngrok settings name. E.g. "global" then there should also be a section \[ngrok-global\]. |

```
[GitNoob]
prjPath=c:\Projects\Webpage
binPath=c:\Projects\bin

commitname=Mirco babin
commitemail=mirco@...

apache=global
php=5.6.40
ngrok=global
```

# section \[projecttypes\]
This section is optional and is for extending GitNoob with additional projecttypes. Each class that implements GitNoob.Config.IProjectType from GitNoob.Config.dll is automatically recognized, the name for the project type will be the lowercase classname. See the project GitNoob.ProjectTypes inside this repository for the primary projecttypes. The "assembly" key may be repeated multiple times.

| Name | Type | Description |
| ---- | ---- | ----------- |
| assembly | Path | Name of an assembly. e.g. "MyProjectTypes.dll" would be loaded from the directory containing GitNoob.exe. |

```
[projecttypes]
assembly=MyProjectTypes.dll
assembly=c:\MyCompany\MyCompanyProjectTypes.dll
```

# section \[projects\]
This section defines the project ini files. The "project" key may be repeated multiple times.

| Name | Type | Description |
| ---- | ---- | ----------- |
| project | Path | Name of an project ini file. e.g. "MyProject.ini". |

```
[projects]
project=MyProject.ini
project=c:\MyCompany\MyCompanyProject.ini
```

# sections starting with \[apache-\]
A section starting with "apache-" defines Apache settings. E.g. "\[apache-global\]" defines an apache section named "global".

| Name | Type | Description |
| ---- | ---- | ----------- |
| apachePath | Path | Path to Apache binaries. %apachePath%\\bin should contain httpd.exe. e.g. "%binPath%\\apache". |
| apacheConf | Path | Path to Apache conf template file. |

```
[apache-global]
apachePath=%binPath%\apache
apacheConf=gitnoob.httpd.template.conf
```

# sections starting with \[php-\]
A section starting with "php-" defines Php settings. E.g. "\[php-5.6.40\]" defines an php section named "5.6.40".

| Name | Type | Description |
| ---- | ---- | ----------- |
| phpPath | Path | Path to Php binaries. %phpPath% should contain php.exe. e.g. "%binPath%\\php-5.6.40". |
| phpIni | Path | Path to Php ini template file. |

```
[php-5.6.40]
phpPath=%binPath%\php-5.6.40
phpIni=%binPath%\php-5.6.40\php.ini

[php-7.2.31]
phpPath=%binPath%\php-7.2.31
phpIni=%binPath%\php-7.2.31\php.ini
```

# sections starting with \[ngrok-\]
A section starting with "ngrok-" defines Ngrok settings. E.g. "\[ngrok-global\]" defines an ngrok section named "global".

| Name | Type | Description |
| ---- | ---- | ----------- |
| ngrokPath | Path | Path to Ngrok binaries. %ngrokPath% should contain ngrok.exe. e.g. "%binPath%\\ngrok". |
| ngrokPort | Integer | Port on localhost where the Ngrok dashboard is running. e.g. the default port "4040" |

```
[ngrok-global]
ngrokPath=%binPath%\ngrok
ngrokPort=4040
```

# full example of GitNoob.ini

```
;
; Lines starting with a semicolon are comments
;

[gitnoob]
prjPath=c:\projects\Webpage
binPath=c:\projects\bin

commitname=Mirco
commitemail=mirco@...

apache=global
php=5.6.40
ngrok=global

[projecttypes]
assembly=MyProjectTypes.dll
assembly=c:\MyCompany\MyCompanyProjectTypes.dll

[projects]
project=MyProject.ini
project=c:\MyCompany\MyCompanyProject.ini

[apache-global]
apachePath=%binPath%\apache
apacheConf=gitnoob.httpd.template.conf

[php-5.6.40]
phpPath=%binPath%\php-5.6.40
phpIni=%binPath%\php-5.6.40\php.ini

[php-7.2.31]
phpPath=%binPath%\php-7.2.31
phpIni=%binPath%\php-7.2.31\php.ini

[ngrok-global]
ngrokPath=%binPath%\ngrok
ngrokPort=4040
```