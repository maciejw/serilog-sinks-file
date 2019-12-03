// Copyright 2013-2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;

namespace Serilog.Sinks.File
{
    delegate void FileDelete(string path);
    delegate string[] DirectoryGetFiles(string logFileDirectory, string directorySearchPattern);
    delegate bool DirectoryExists(string logFileDirectory);

    static class IO
    {
        [ThreadStatic]
        private static FileDelete _fileDelete;
        [ThreadStatic]
        private static DirectoryGetFiles _directoryGetFiles;
        [ThreadStatic]
        private static DirectoryExists _directoryExists;

        public static FileDelete FileDelete => _fileDelete;

        public static DirectoryGetFiles DirectoryGetFiles => _directoryGetFiles;

        public static DirectoryExists DirectoryExists => _directoryExists;

        static IO()
        {
            SetIO();
        }

        public static void SetIO(FileDelete fileDelete = null, DirectoryGetFiles directoryGetFiles = null, DirectoryExists directoryExists = null)
        {
            _fileDelete = fileDelete ?? System.IO.File.Delete;
            _directoryGetFiles = directoryGetFiles ?? Directory.GetFiles;
            _directoryExists = directoryExists ?? Directory.Exists;
        }
    }
}
