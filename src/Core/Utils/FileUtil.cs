using System.IO;
using System;

namespace Nekres.FailScreens.Core {
    internal static class FileUtil {
        /// <summary>
        /// Checks if a file is currently locked.
        /// </summary>
        /// <remarks>
        /// Suffers from thread race condition.
        /// </remarks>
        /// <param name="uri">The filename.</param>
        /// <returns><see langword="True"/> if file is locked or does not exist. Otherwise <see langword="false"/>.</returns>
        public static bool IsFileLocked(string uri) {
            FileStream stream = null;
            try {
                stream = File.Open(uri, FileMode.Open, FileAccess.Read, FileShare.None);
                // ERROR_SHARING_VIOLATION
            } catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32) {
                return true;
                // ERROR_LOCK_VIOLATION
            } catch (IOException e) when ((e.HResult & 0x0000FFFF) == 33) {
                return true;
            } catch (Exception) {
                return false;
            } finally {
                stream?.Dispose();
            }
            return false;
        }
    }
}
