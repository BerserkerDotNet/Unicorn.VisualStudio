# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 1.5

- Better support for Unicorn 3.1
- Configuration status messages
- Search in logs
- Copy log entry to clipboard

**BREAKING CHANGES**

In Unicorn 3.1 "/unicornRemote.aspx" was removed, now you can use "/unicorn.aspx". 
Please make sure to update your settings.

## 1.4

- Support for upcoming changes in Unicorn 3.1
- Progress status is now displayed in VS status bar

## 1.3 

- fixed issue with "Configurations dropdown doesn't update after fixing a broken connection" 
- fixed issue "Cannot connect using Deployment Token" 
- added settings window to change remote URL. 
- support for arbitrary multiple config selections (need to be enabled in settings)

## 1.2.1

- Update for menu icon

## 1.2 

- fixed warning prompting to update Unicorn.Remote.dll if using Unicorn 3.0 or above
- new official icon

## 1.1 

- using streaming instead of polling
- JSON.NET is removed from a dependencies list.