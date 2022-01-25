# Php Ini template file replacements

In the configured **phpIni** file, markers will be replaced before starting Apache or a dosprompt.

```
; Directory in which the loadable extensions (modules) reside.
; http://php.net/extension-dir
; extension_dir = "./"
; On windows:
extension_dir = "[PHP_EXTENSION_DIR_SLASH]"
```

## marker \[PHP_EXTENSION_DIR_SLASH\]
The marker "\[PHP_EXTENSION_DIR_SLASH\]" will be replaced with the configured **phpPath** setting. Using unix-style '/' as directory seperator.

## marker \[PHP_EXTENSION_DIR\]
The marker "\[PHP_EXTENSION_DIR\]" will be replaced with the configured **phpPath** setting. Using windows-style '\\' as directory seperator.
