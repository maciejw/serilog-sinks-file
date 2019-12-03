using Xunit;
using System.Threading;

namespace Serilog.Sinks.File.Tests
{
    public class IOTests
    {
        [Fact]
        public void ShouldUseDefaultImplementations()
        {
            Assert.Equal(System.IO.File.Delete, IO.FileDelete);
            Assert.Equal(System.IO.Directory.GetFiles, IO.DirectoryGetFiles);
            Assert.Equal(System.IO.Directory.Exists, IO.DirectoryExists);
        }

        [Fact]
        public void ShouldResetImplementationOnCurrentCurrentThread()
        {
            void Thread1Work()
            {
                GetCustomImplementations(out var fileDelete, out var directoryGetFiles, out var directoryExists);
                IO.Reset(fileDelete, directoryGetFiles, directoryExists);
            }

            var testThread = new Thread(Thread1Work);

            testThread.Start();
            testThread.Join();

            ShouldUseDefaultImplementations();
        }

        [Fact]
        public void ShouldBeAbleToResetCustomImplementationAndRevertToDefaults()
        {
            try
            {
                GetCustomImplementations(out var fileDelete, out var directoryGetFiles, out var directoryExists);

                IO.Reset(fileDelete, directoryGetFiles, directoryExists);

                Assert.Equal(fileDelete, IO.FileDelete);
                Assert.Equal(directoryGetFiles, IO.DirectoryGetFiles);
                Assert.Equal(directoryExists, IO.DirectoryExists);
            }
            finally
            {
                IO.Reset();
                ShouldUseDefaultImplementations();
            }
        }

        private static void GetCustomImplementations(out FileDelete fileDelete, out DirectoryGetFiles directoryGetFiles, out DirectoryExists directoryExists)
        {
            fileDelete = (_) => { };
            directoryGetFiles = (_, __) => new string[0];
            directoryExists = (_) => false;
        }
    }
}
