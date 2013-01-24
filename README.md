KeePass to RDP
==============

KeePass to RDP is an application that uses RDP entries from a KeePass
database to launch RDP sessions. This removes the hassle of having to enter
connection info and login credentials to hosts, especially handy in an
environment where you connect to various servers all day long.

![Default window](https://github.com/tetsuo13/KeePassToRdp/raw/master/media/launch.png)

Download
--------

Stable release - latest version:

* [1.0.4767.41767](http://andreinicholson.com/project/keepasstordp/KeePassToRdp-1.0.4767.41767.zip)

Previous releases:

* [1.0.4760.42330](http://andreinicholson.com/project/keepasstordp/KeePassToRdp-1.0.4760.42330.zip)

Usage
-----

Requires a KeePass 2.x KDBX file. Only entries with the string "RDP" (without
quotes, case-insensitive) in either the title, notes, or tags will appear in
the list. If the entries fall within a group, the group will also appear in
the list.

The RDP session is launched by Remote Desktop Connection (`mstsc.exe`).
KeePassToRdp will configure the session to launch full screen on the second
monitor, if found; otherwise it defaults to the primary monitor.

Changelog
---------

### 1.0.4767.41767

- Fixed issue with first entry not under a group.
- Fixed issue after opening additional databases where entries were appended
  to the list instead of reset.

### 1.0.4760.42330

- Initial release.
