using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLooper.Utils;

namespace VideoLooper.Models {

    /// <summary>
    /// information for a playlist item
    /// </summary>
    public class PlaylistItem {

        /// <summary>
        /// the full file path of the item
        /// </summary>
        public String FullPath { get; set; }

        /// <summary>
        /// the file name of the item
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// the begin time for the loop
        /// </summary>
        public String BeginTimestamp { get; set; } = "00:00:00";

        /// <summary>
        /// the ending time for the loop
        /// </summary>
        public String EndTimestamp { get; set; } = "00:00:00";

        /// <summary>
        /// the jump to time
        /// </summary>
        public String JumpTimestamp { get; set; } = "00:00:00";

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="fullPath">the full file path of the item</param>
        public PlaylistItem(String fullPath) {
            FullPath = fullPath;
            FileName = VideoUtils.getFileName(fullPath);
        }
    }
}
