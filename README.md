KeePass to RDP
==============

KeePass to RDP is an application that uses RDP entries from a KeePass
database to launch RDP sessions. This removes the hassle of having to enter
connection info and login credentials to hosts, especially handy in an
environment where you connect to various servers all day long.

It will aggregate all entries from a KeePass database which are for RDP
connections and present them in an easy to use interface. Simply select
the entry from the dropdown list and click on the **Launch Connection**
button. KeePassToRDP will launch Remote Desktop Connection and fill in
all of the authentication details automatically. It will also launch the
window full screen and on the second monitor if you have it.

![Default window](https://github.com/tetsuo13/KeePassToRdp/raw/master/media/launch.png)

Usage
-----

Requires a KeePass 2.x KDBX file. Only entries with the string "RDP" (without
quotes, case-insensitive) in either the title, notes, or tags will appear in
the list. If the entries fall within a group, the group will also appear in
the list. An example server list is shown below which includes multiple
KeePass groups, which are translated to unselected sections in the dropdown
list.

![Server dropdown list](https://github.com/tetsuo13/KeePassToRdp/raw/master/media/servers.png)

For reference, the database looks like this in KeePass.

![KeePass view](https://github.com/tetsuo13/KeePassToRdp/raw/master/media/keepass_group_view.png)

Why not just write a plugin for KeePass which does the same thing? This
program is intended to be standalone from KeePass, it just happens to use
its database for credentials. It was originally created out of the need
to connect easily to remote machines. The actual KeePass program played
no role in that.

Technical details
-----------------

KeePassToRDP generates an RDP file suitable for Remote Desktop Connection
(`mstsc.exe`) to use. This file is saved to the user's temporary folder
while KeePassToRDP initiates the process -- read the server login
credentials from KeePass, encrypt them to an RDP file, launch Remote
Desktop Connection pointing to the RDP file, and finally delete the RDP
file. There is a security concern where the file can be copied during the
small window where it exists.

The password saved to this file is encrypted using the Data Protection
API (DPAPI). Only the user which encrypted the password can decrypt it.
In other words, if an attack was made against the generated RDP file and
it was copied during its short-lived life, the attacker would have to use
your Windows credentials in order to decrypt the password.
