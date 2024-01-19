# GitNoob changelog

## Version 1.21
* Add: cherry pick a commit.
* Fix: prevent Windows screensaver when merging.

## Version 1.20
* Add: show git reference log.
* Add: delete of undeletion tag.
* Add: toilet option to put current changes in a deleted branch, so they can also be undeleted.
* Add: start git gui and workspace upon rebase/merge conflicts.
* Fix: before rebase create a undelete tag.
* Fix: change branch in detached head mode.
* Add: touch-timestamp-of-commits-before-merge setting

## Version 1.19
* Fix: several hints and explanations.
* Add: dividers in choose project screen.
* Add: empty git recycle bin to git repair options.
* Add: stage all changes to git repair options.

## Version 1.18
* Fix: Laravel - composer dump-autoload issues with packages.php - see also https://github.com/laravel/framework/issues/45209

## Version 1.17
* Fix: after get latest try to ensure main branch exists.
* Fix: cloning should checkout the specified main branch.
* Fix: show an errormessage if configuration file setting "loadRootConfigurationFrom" specifies an invalid file.
* Fix: show an errormessage if there isn't any project defined in the configuration files.
* Fix: starting history.
* Add: configuration setting commitname-settings-via-filename
* Add: configuration setting commitname-settings-clear-on-exit

## Version 1.16
* Project migrated to Visual Studio 2019 Community Edition.
* Fix: show correct icon on Windows/11.
* Add: "Show history of one file" on current branch history button via right mouseclick.
* Add: "Show history of all branches / tags / remotes" on current branch history button via right mouseclick.
* Fix: set committer name & email.
* Fix: touch commit timestamps.
* Remove: git repair option "Show history of all branches / tags / remotes", it can be found via context menu on current branch history.
* Add: system menu option "Check for GitNoob updates".
* Add: GitNoobUpdater.exe for automated updating.

## Version 1.15
* Fix: start explorer multiple times.
* Fix: exiting the dosprompt, sometimes "exit" had to be entered twice.
* Add: "Run as administrator" on dosprompt button via right mouseclick.

## Version 1.14
* Fix: workspace-run-as-administrator works again.
* Fix: use LocalApplicationData path for intermediate files. As files in the temp path may be deleted by the Windows OS at unexpected moments, when GitNoob is still running.

## Version 1.13
* Fix: use Ngrok v3.

## Version 1.12
* Fix: undelete a deleted branch was wrongly determining the commitid.

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
