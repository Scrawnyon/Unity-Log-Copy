using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// Copies log files from %localappdata%low to the actual game folder (or Unity editor).
/// Either add this as a MonoBehaviour to the scene (in which case the logs will be copied when the app closes),
/// or call CopyLogsToAppFolder.DoCopyLogs()
/// </summary>
public class CopyLogsToAppFolder : MonoBehaviour
{
    static string LogFolderPath => Application.persistentDataPath;
    static string TargetFolderPath => Path.Combine(Application.dataPath, TARGET_FOLDER_NAME);

    const string TARGET_FOLDER_NAME = "Logs";
    const string LOG_EXTENSION = ".log";
    const string METAFILE_EXTENSION = ".meta";
    const string FILENAME_PREFIX = "Log_";
    const string DATETIME_FORMAT = "yyyy-MM-dd_HH-mm-s";
    const int MAX_LOG_FILES_STORED = 50;

    void OnApplicationQuit() => DoCopyLogs();

    public static void DoCopyLogs()
    {
        EnsureFolderStructure();

        // Generate hash of datestrings that already exist in the target folder
        string _targetFolderPath = TargetFolderPath;
        List<string> _transferedLogFiles = GetLogFilesInFolder(_targetFolderPath); // Log files we already have
        int _numFilesStored = _transferedLogFiles.Count;
        HashSet<string> _existingWriteTimeStrings = new HashSet<string>(_transferedLogFiles.ConvertAll(x => PathToLastWriteTimeString(x)));

        // Get paths to logs created by Unity
        List<string> _logFiles = GetLogFilesInFolder(LogFolderPath);
        for (int i = 0; i < _logFiles.Count; i++)
        {
            // Convert the file's last write time to our custom DateTime string
            DateTime _lastWriteTime = File.GetLastWriteTime(_logFiles[i]);
            string _lastWriteTimeString = LastWriteTimeToFileName(_lastWriteTime);

            // If this file has already been copied, keep moving
            if (_existingWriteTimeStrings.Contains(_lastWriteTimeString))
                continue;

            // Create new file to target folder
            string _newFileName = LastWriteTimeToFileName(_lastWriteTime);
            string _newFilePath = Path.Combine(_targetFolderPath, _newFileName + LOG_EXTENSION);
            string _newLogContents = File.ReadAllText(_logFiles[i]);
            File.WriteAllText(_newFilePath, _newLogContents);

            _numFilesStored++;
        }

        // If we're storing too many files, remove leftovers
        if (_numFilesStored > MAX_LOG_FILES_STORED)
        {
            // Get stored log files and sort them by file name reversed (which denotes last write date)
            List<string> _storedLogFiles = GetLogFilesInFolder(_targetFolderPath);
            _storedLogFiles.Sort();
            _storedLogFiles.Reverse();
            for (int i = MAX_LOG_FILES_STORED; i < _storedLogFiles.Count; i++)
            {
                File.Delete(_storedLogFiles[i]);

                // If we're deleting files from Unity editor, also delete the meta files to avoid the warning in console
                string _metaFile = _storedLogFiles[i] + METAFILE_EXTENSION;
                if (File.Exists(_metaFile))
                    File.Delete(_metaFile);
            }
        }
    }

    /// <summary>
    /// Convert DateTime to a file name that can be parsed later if needed. Does not include a file extension
    /// </summary>
    static string LastWriteTimeToFileName(DateTime _lastWriteTime) => FILENAME_PREFIX + LastWriteTimeToDateString(_lastWriteTime);

    /// <summary>
    /// Convert DateTime to a specific datetime format, which is used to create file names
    /// </summary>
    static string LastWriteTimeToDateString(DateTime _lastWriteTime) => _lastWriteTime.ToString(DATETIME_FORMAT, CultureInfo.InvariantCulture);

    /// <summary>
    /// Convert the path of a log file in the target folder (that's already been renamed), and return only
    /// the part of the file name that denotes last write time
    /// </summary>
    static string PathToLastWriteTimeString(string _path)
    {
        string _fileName = Path.GetFileNameWithoutExtension(_path);
        if (_fileName.Length < 4)
        {
            Debug.LogError("Invalid path to DateTime conversion: " + _path);
            return "";
        }

        int _prefixLength = FILENAME_PREFIX.Length;
        return _fileName.Substring(_prefixLength);
    }

    static List<string> GetLogFilesInFolder(string _folderPath)
    {
        if (Directory.Exists(_folderPath) == false)
            return new List<string>();

        List<string> _files = new List<string>(Directory.GetFiles(_folderPath));
        _files.RemoveAll(x => Path.GetExtension(x) != LOG_EXTENSION);

        return _files;
    }

    /// <summary>
    /// Make sure the target folder exists. If not, create it
    /// </summary>
    static void EnsureFolderStructure()
    {
        if (Directory.Exists(TargetFolderPath) == false)
            Directory.CreateDirectory(TargetFolderPath);
    }
}