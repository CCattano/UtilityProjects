# Utility Projects

Misc projects made so I didn't have to keep doing something manually

### Toggle Ethernet

Toggles the Ethernet Network Adapter for Windows machines.

Created because my company's network blocks spotify, but the guest wifi hotspot doesn't.
  
  Got tired of having to click through several popup-menus/screens to shutoff the ethernet adapter
  to switch to Wifi so I could download something on Spotify via guest Wifi then switch back
  
  Console app just toggles the adapter to the opposite of its current state for me

### SFTP Client
Needed to change the name of and relocate like, 30-something files on a SFTP Server of mine once

Renaming alone would've been tedious, but to relocate in WinSCP you have to provide the full file path for the new dest

Would've been a lot of copypasta work

Figured there's probably a SFTPClient library similar to HttpClient out there, and there was.

Used it to programatically apply the changes I needed to the files on my server.
