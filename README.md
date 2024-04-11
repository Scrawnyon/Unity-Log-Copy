## Unity Log Copy
Copies and stores log files from %localappdata%low to the actual game folder (or Unity editor).

Either add this as a MonoBehaviour to the scene (in which case the logs will be copied when the app closes), or call CopyLogsToAppFolder.DoCopyLogs()

### Why?
Unity only stores two log files at a time, and overwrites them on each launch, which can be troublesome when users run into bugs since the logs may already be gone, and directing less experienced users to the folder can be a bother

### Future
Considering automatically purging machine-related info and personal details like file paths from the logs