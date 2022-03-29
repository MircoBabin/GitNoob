# Php Ini template file replacements

In the configured **phpIni** file, markers will be replaced before starting Apache or a dosprompt.

```
; Directory in which the loadable extensions (modules) reside.
; http://php.net/extension-dir
; extension_dir = "./"
; On windows:
extension_dir = "[PHP_EXTENSION_DIR_SLASH]"
```

## marker \[GIT_ROOT_DIR_SLASH\]
The marker "\[GIT_ROOT_DIR_SLASH\]" will be replaced with the configured **path** setting. Using unix-style '/' as directory seperator.

## marker \[GIT_ROOT_DIR\]
The marker "\[GIT_ROOT_DIR\]" will be replaced with the configured **path** setting. Using windows-style '\\' as directory seperator.



## marker \[PHP_EXTENSION_DIR_SLASH\]
The marker "\[PHP_EXTENSION_DIR_SLASH\]" will be replaced with the configured **phpPath** setting. Using unix-style '/' as directory seperator.

## marker \[PHP_EXTENSION_DIR\]
The marker "\[PHP_EXTENSION_DIR\]" will be replaced with the configured **phpPath** setting. Using windows-style '\\' as directory seperator.



## Example for XDEBUG extension

```
[xdebug]
; Install php_xdebug.dll via the instructions at https://xdebug.org/wizard
;
; Uncomment the next line to enable the xdebug extension.
; zend_extension=xdebug

xdebug.log="[GIT_ROOT_DIR_SLASH]/storage/logs/php-xdebug.log"
xdebug.output_dir="[GIT_ROOT_DIR_SLASH]/storage/logs"

xdebug.trace_output_name=php-xdebug-trace.%u.%r
xdebug.start_with_request=yes
xdebug.mode=trace
xdebug.trace_format=0
xdebug.trace_options=0
xdebug.use_compression=false
```