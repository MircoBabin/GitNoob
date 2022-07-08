# GitNoob changelog

## Version 1.11
* Fix: starting Ngrok when apache is configured to use SSL (https).
* Add: delete branch
* Add: repair option: undelete a deleted branch.
* Add: repair option: set/touch the author and commit timestamps of all unmerged commits on the current branch.

## Version 1.10
* Fix: open working directory failed when Fiddler was not installed.

## Version 1.9
* Add: configuration for apache SSL certificate (https connections).
* Add: copy exception details to Windows clipboard when opening working directory fails.
* Add: check executable blocked status upon startup.

## Version 1.8
* Add: hints on buttons.
* Add: show history of current branch.
* Add: git repair options button.
* Add: repair option: show history of all branches.
* Add: repair option: unpack last commit on current branch.
* Fix: check for GitNoob Temporary Commits before merging.
* renamed release distribution to GitNoob-x.x.zip
* for automatic installation scripts https://github.com/MircoBabin/GitNoob/releases/latest/download/release.download.zip.url-location is a textfile and will contain an url to the latest release zip file

## Version 1.7
* Fix: show, don't hide, Apache running window.

## Version 1.6
* Fix: configure php for start workspace.
* Fix: configure global composer path (%appdata%\Composer\vendor\bin) for start workspace & start dosprompt.

## Version 1.5
* Add: workspace-run-as-administrator setting.
* Add: rename current branch.
* Add: show Apache path & version when starting Apache.
* Fix: setting committer name & email.

## Version 1.4
* Add: [GIT_ROOT_DIR] & [GIT_ROOT_DIR_SLASH] to template files.
* Add: PhpLaravel8, PhpLaravel9 projecttypes.
* Add: title for dos prompt window.
* Fix: php.ini & apache.conf files refresh without exiting GitNoob.
* Fix: non existing directory exception.
* Fix: non existing directory asking to repair committername.

## Version 1.3

* Add: check committername after choosing a project.
* Add: check remote for main branch after choosing a project.
* Fix: get latest with current branch is main branch should automatically fast forward.
* Fix: refresh status more often.

## Version 1.2

* Fix: create new branch on never checked out main branch.

## Version 1.1

* Fix: display the correct browser icon.
* Add: (optionally) start smtp server.
