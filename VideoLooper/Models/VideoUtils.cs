using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper.Utils {
    /// <summary>
    /// collection of helper methods for controlling the player
    /// </summary>
    public static class VideoUtils {

        /// <summary>
        /// normalize given timestamp to hh:mm:ss
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static String NormalizeTimestamp(String timestamp) {
            String hh = "";
            if (timestamp.Length < 6) {
                hh += "00:";
            }
            return hh + timestamp;
        }

        /// <summary>
        /// converts a normalized timestamp to seconds
        /// </summary>
        /// <param name="timestamp">video timestamp in hh:mm:ss format</param>
        /// <returns>number of seconds, or -1 if error occured</returns>
        public static int ConvertTimestampToSeconds(String normalizedTimestamp) {

            String[] breakup = normalizedTimestamp.Split(':');

            if (breakup.Length == 3) {
                try {
                    int hh = Int32.Parse(breakup[0]) * 60 * 60;
                    int mm = Int32.Parse(breakup[1]) * 60;
                    int ss = Int32.Parse(breakup[2]);

                    return hh + mm + ss;
                } catch {}
            }

            return -1;
        }

        /// <summary>
        /// get the file name for the item
        /// </summary>
        /// <param name="fullPath">the full path of the item</param>
        /// <returns></returns>
        public static String getFileName(String fullPath) {

            if (String.IsNullOrEmpty(fullPath)) {
                return String.Empty;
            }

            int lastIndex = fullPath.LastIndexOf('\\');

            if (lastIndex <= 0) {
                return fullPath;
            }
            return fullPath.Substring(lastIndex + 1);
        }


        /// <summary>
        /// perform in-place shuffle of list using fischer-yates algo
        /// </summary>
        /// <param name="list">the list to be shuffled</param>
        public static void shuffleInPlace<T>(ObservableCollection<T> list) {

            int j = list.Count - 1;
            Random rng = new Random();
            while (j > 0) {

                var temp = list[j];
                int randomIndex = rng.Next(j);
                list[j] = list[randomIndex];
                list[randomIndex] = temp;
                j--;
            }
        }
    }
}
