## Unity Log Copy
Copies and stores log files from %localappdata%low to the actual game folder (or Unity editor)

### Why?
Unity only stores two log files at a time and overwrites them on each launch, which can be troublesome when users run into bugs, since the logs may already be gone, and directing less experienced users to the folder can be a bother

### Usage
Either add this as a MonoBehaviour to the scene (in which case the logs will be copied when the app closes), or call CopyLogsToAppFolder.DoCopyLogs()

### Future
Considering automatically purging machine-related info and personal details like file paths from the logs

### Notes
Unity only creates log files when running built projects (not the editor), and doesn't update the project folder view until the editor has lost and regained focus. So the Logs folder may not show the latest files until you click on another window, and then return to Unity, which will trigger the refresh