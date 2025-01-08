# Php Ini template file replacements

In the configured **phpIni** file, markers will be replaced before starting Apache or a dosprompt.

```
; Directory in which the loadable extensions (modules) reside.
; http://php.net/extension-dir
; extension_dir = "./"
; On windows:
extension_dir = "[PHP_EXTENSION_DIR_SLASH]"



; Directory where the temporary files should be placed.
; Defaults to the system default (see sys_get_temp_dir)
sys_temp_dir = "[PHP_TEMP_DIR_SLASH]"



; Log errors to specified file. PHP's default behavior is to leave this value
; empty.
; https://php.net/error-log
; Example:
error_log = "[PHP_ERROR_LOG_FILENAME_SLASH]"
```

## marker \[GIT_ROOT_DIR_SLASH\]
The marker "\[GIT_ROOT_DIR_SLASH\]" will be replaced with the configured **path** setting. Using unix-style '/' as directory seperator.

## marker \[GIT_ROOT_DIR\]
The marker "\[GIT_ROOT_DIR\]" will be replaced with the configured **path** setting. Using windows-style '\\' as directory seperator.



## marker \[PHP_EXTENSION_DIR_SLASH\]
The marker "\[PHP_EXTENSION_DIR_SLASH\]" will be replaced with the configured **phpPath** setting. Using unix-style '/' as directory seperator.

## marker \[PHP_EXTENSION_DIR\]
The marker "\[PHP_EXTENSION_DIR\]" will be replaced with the configured **phpPath** setting. Using windows-style '\\' as directory seperator.



## marker \[PHP_TEMP_DIR_SLASH\]
The marker "\[PHP_TEMP_DIR_SLASH\]" will be replaced with the configured **phpTempPath** setting. The project type (PhpLaravel) can override this setting. Using unix-style '/' as directory seperator.

## marker \[PHP_TEMP_DIR\]
The marker "\[PHP_TEMP_DIR\]" will be replaced with the configured **phpTempPath** setting. The project type (PhpLaravel) can override this setting. Using windows-style '\\' as directory seperator.



## marker \[PHP_LOG_DIR_SLASH\]
The marker "\[PHP_LOG_DIR_SLASH\]" will be replaced with the configured **phpLogPath** setting. The project type (PhpLaravel) can override this setting. Using unix-style '/' as directory seperator.

## marker \[PHP_LOG_DIR\]
The marker "\[PHP_LOG_DIR\]" will be replaced with the configured **phpLogPath** setting. The project type (PhpLaravel) can override this setting. Using windows-style '\\' as directory seperator.



## marker \[PHP_ERROR_LOG_FILENAME_SLASH\]
The marker "\[PHP_ERROR_LOG_FILENAME_SLASH\]" will be replaced with "php-errors.log" in the configured **phpLogPath** setting. The project type (PhpLaravel) can override this setting. Using unix-style '/' as directory seperator.

## marker \[PHP_ERROR_LOG_FILENAME\]
The marker "\[PHP_ERROR_LOG_FILENAME\]" will be replaced with "php-errors.log" in the configured **phpLogPath** setting. The project type (PhpLaravel) can override this setting. Using windows-style '\\' as directory seperator.



## Example for XDEBUG extension

```
[xdebug]
; Install php_xdebug.dll via the instructions at https://xdebug.org/wizard
;
; Uncomment the next line to enable the xdebug extension.
; zend_extension=xdebug

xdebug.log="[PHP_LOG_DIR_SLASH]/php-xdebug.log"
xdebug.output_dir="[PHP_LOG_DIR_SLASH]"

xdebug.trace_output_name=php-xdebug-trace.%u.%r
xdebug.start_with_request=yes
xdebug.mode=trace
xdebug.trace_format=0
xdebug.trace_options=0
xdebug.use_compression=false
```