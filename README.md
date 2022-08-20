# KeeLocker
KeePass2 plugin to open BitLocker volumes when you open your KeePass database.

Should also work for USB sticks or portable drives.

## The feature

This plugin adds the following tab to entries :

![Select Drive by MountPoint or GUID, and whether you want the drive to open automatically](https://github.com/Gugli/KeeLocker/raw/main/KeeLockerSettings.png)

## How to use

If you have a static drive (let's say a HDD bound to "D:") encrypted with bitlocker, you should :

  -  create a new entry, choose a title you want
  -  put the password for the drive in the entry usual password field
  -  Select "D:" as drive mountpoint in the KeeLocker tab

You can the either tick the checkbox "Unlock volume on opening" that will do so when the Database is opened (but that requires to close then open the DB). Or you can click "Unlock Volume Now" to test it right now.

## When should I use of *Drive GUID* ?

The *Drive GUID* feature is for more complex scenarios.

For example : if you use the same DB on multiple machines, along with a USB stick cyphered using BitLocker. 
The issue is that on the first machine, you USB stick mounts as "D:", but on another one, it mounts as "E:". 

In such cases, the "Drive Mountpoint" is not super useful, so instead of using it, you can use the "GUID", that should work in such scenarios.
