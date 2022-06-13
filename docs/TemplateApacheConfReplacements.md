# Apache Conf template file replacements

In the configured **apacheConf** file, markers will be replaced before starting Apache.

```
[APACHE_LOADMODULE_PHP]
PHPIniDir "[PHP_INIFILE_SLASH]"

Define SRVROOT "[APACHE_SRVROOT_SLASH]"
ServerRoot "${SRVROOT}"
PidFile "[APACHE_PIDFILE_SLASH]"

Listen [APACHE_PORT]

ServerName localhost:[APACHE_PORT]

ErrorLog "[APACHE_ERRORLOG]"

CustomLog "[APACHE_CUSTOMLOG]" common

<VirtualHost *:[APACHE_PORT]>
    [APACHE_VIRTUALHOST_SSL]
    
    ServerName localhost
    ServerAlias [COMPUTERNAME]
    DocumentRoot "[WEBROOT]"
    <Directory "[WEBROOT_SLASH]/">
        AllowOverride All
        Options FollowSymLinks Indexes 
#       Require local
    </Directory>
</VirtualHost>
```

## marker \[GIT_ROOT_DIR\]
The marker "\[GIT_ROOT_DIR\]" will be replaced with the configured **path** setting. Using windows-style '\\' as directory seperator.

## marker \[GIT_ROOT_DIR_SLASH\]
The marker "\[GIT_ROOT_DIR_SLASH\]" will be replaced with the configured **path** setting. Using unix-style '/' as directory seperator.



## marker \[APACHE_PORT\]
The marker "\[APACHE_PORT\]" will be replaced with the configured **port** setting.

## marker \[APACHE_SRVROOT_SLASH\]
The marker "\[APACHE_SRVROOT_SLASH\]" will be replaced with the configured **apachePath** setting. Using unix-style '/' as directory seperator.

## marker \[APACHE_PIDFILE_SLASH\]
The marker "\[APACHE_PIDFILE_SLASH\]" will be replaced with the pid filename. Using unix-style '/' as directory seperator.

## marker \[APACHE_ERRORLOG\]
The marker "\[APACHE_ERRORLOG\]" will be replaced with the errorlog filename. Using unix-style '/' as directory seperator.

## marker \[APACHE_CUSTOMLOG\]
The marker "\[APACHE_CUSTOMLOG\]" will be replaced with the customlog filename. Using unix-style '/' as directory seperator.

## marker \[APACHE_VIRTUALHOST_SSL\]
The marker "\[APACHE_VIRTUALHOST_SSL\]" if configured setting **apacheUseSsl=true** will be replaced with:

```
SSLEngine on
SSLCertificateKeyFile "[APACHE_VIRTUALHOST_SSL_CERTIFICATEKEYFILE_SLASH]"
SSLCertificateFile "[APACHE_VIRTUALHOST_SSL_CERTIFICATEFILE_SLASH]"
SSLCertificateChainFile "[APACHE_VIRTUALHOST_SSL_CERTIFICATECHAINFILE_SLASH]"
```

If configured setting **apacheUseSsl=false** then the marker will be replaced with an empty string.

## marker \[APACHE_VIRTUALHOST_SSL_ONOFF]
The marker "\[APACHE_VIRTUALHOST_SSL_ONOFF\]" will be replaced with:

- **on**, if if configured setting **apacheUseSsl=true**
- **off**, if if configured setting **apacheUseSsl=false**

## marker \[APACHE_VIRTUALHOST_SSL_CERTIFICATEKEYFILE_SLASH\]
The marker "\[APACHE_VIRTUALHOST_SSL_CERTIFICATEKEYFILE_SLASH\]" will be replaced with the configured **apacheSslCertificateKeyFile** setting. Using unix-style '/' as directory seperator.

## marker \[APACHE_VIRTUALHOST_SSL_CERTIFICATEFILE_SLASH\]
The marker "\[APACHE_VIRTUALHOST_SSL_CERTIFICATEFILE_SLASH\]" will be replaced with the configured **apacheSslCertificateFile** setting. Using unix-style '/' as directory seperator.

## marker \[APACHE_VIRTUALHOST_SSL_CERTIFICATECHAINFILE_SLASH\]
The marker "\[APACHE_VIRTUALHOST_SSL_CERTIFICATECHAINFILE_SLASH\]" will be replaced with the configured **apacheSslCertificateChainFile** setting. Using unix-style '/' as directory seperator.


## marker \[PROJECTNAME\]
The marker "\[PROJECTNAME\]" will be replaced with the project-workingdirectory name.



## marker \[WEBROOT\]
The marker "\[WEBROOT\]" will be replaced with the configured **webroot** setting. Using windows-style '\\' as directory seperator.

## marker \[WEBROOT_SLASH\]
The marker "\[WEBROOT_SLASH\]" will be replaced with the configured **webroot** setting. Using unix-style '/' as directory seperator.



## marker \[PHP_PATH_SLASH\]
The marker "\[PHP_PATH_SLASH\]" will be replaced with configured **phpPath** setting. Using unix-style '/' as directory seperator.

## marker \[PHP_INIFILE_SLASH\]
The marker "\[PHP_INIFILE_SLASH\]" will be replaced with replaced **phpIni** template setting. Using unix-style '/' as directory seperator.

## marker \[APACHE_LOADMODULE_PHP\]
The marker "\[APACHE_LOADMODULE_PHP\]" will be replaced with: LoadModule php?_module "php?apache_2.4.dll". Taking into account the different Php 5, 7 and 8 syntax for this statement.

## marker \[COMPUTERNAME\]
The marker "\[COMPUTERNAME\]" will be replaced with the computername configured in Windows.
