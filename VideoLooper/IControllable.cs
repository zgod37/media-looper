using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoLooper {

    /// <summary>
    /// interface for each window
    /// </summary>
    public interface IControllable {

        /// <summary>
        /// plays the media only if its not playing
        /// </summary>
        void StrictPlay();

        /// <summary>
        /// pauses the media only if its playing
        /// </summary>
        void StrictPause();

        /// <summary>
        /// stops the media
        /// </summary>
        void Stop();

        /// <summary>
        /// goto the next item in the playlist
        /// </summary>
        void Next();

        /// <summary>
        /// goto the previous item in the playlist
        /// </summary>
        void Prev();

        /// <summary>
        /// jump to the user-specified timestamp in the video
        /// </summary>
        void JumpTo();

    }
}
