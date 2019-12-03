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
using System.Threading;

namespace Serilog.Sinks.File
{
    delegate void FileDelete(string path);
    delegate string[] DirectoryGetFiles(string logFileDirectory, string directorySearchPattern);
    delegate bool DirectoryExists(string logFileDirectory);

    static class IO
    {
        private static FileDelete DefaultFileDelete => System.IO.File.Delete;
        private static DirectoryGetFiles DefaultDirectoryGetFiles => Directory.GetFiles;
        private static DirectoryExists DefaultDirectoryExists => Directory.Exists;

        [ThreadStatic] private static FileDelete _testFileDelete;
        [ThreadStatic] private static DirectoryGetFiles _testDirectoryGetFiles;
        [ThreadStatic] private static DirectoryExists _testDirectoryExists;

        private static FileDelete _fileDelete;
        private static DirectoryGetFiles _directoryGetFiles;
        private static DirectoryExists _directoryExists;

        public static FileDelete FileDelete { get => _fileDelete ?? _testFileDelete ?? DefaultFileDelete; }
        public static DirectoryGetFiles DirectoryGetFiles { get => _directoryGetFiles ?? _testDirectoryGetFiles ?? DefaultDirectoryGetFiles; }
        public static DirectoryExists DirectoryExists { get => _directoryExists ?? _testDirectoryExists ?? DefaultDirectoryExists; }


        static IO()
        {
            Reset();
        }

        /// <summary>
        /// Set IO operation to specific implementation
        /// </summary>
        /// <remarks>
        /// Test implemetation is stored in <see cref="ThreadStaticAttribute"/> field and default implementation is set to null.
        /// Passing null as an argument resets vlaue to defaults.
        /// </remarks>
        /// <param name="fileDelete"></param>
        /// <param name="directoryGetFiles"></param>
        /// <param name="directoryExists"></param>
        public static void Reset(FileDelete fileDelete = null, DirectoryGetFiles directoryGetFiles = null, DirectoryExists directoryExists = null)
        {
            SetField(ref _fileDelete, ref _testFileDelete, fileDelete ?? DefaultFileDelete, fileDelete != null);
            SetField(ref _directoryGetFiles, ref _testDirectoryGetFiles, directoryGetFiles ?? DefaultDirectoryGetFiles, directoryGetFiles != null);
            SetField(ref _directoryExists, ref _testDirectoryExists, directoryExists ?? DefaultDirectoryExists, directoryExists != null);
        }

        private static void SetField<T>(ref T field, ref T testField, T value, bool setTestInplementation = false) where T : class
        {
            if (setTestInplementation)
            {
                field = null;
                testField = value;
            }
            else
            {
                field = value;
                testField = null;
            }
        }
    }
}
