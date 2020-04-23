using System;
using System.Collections.Generic;
using System.Text;

namespace qdx.Samples.FileCreator.Diagnostics
{
    public enum EventCodes
    {
        // Errors
        /// <summary>
        /// Cannot create a negative number of files
        /// </summary>
        Khopesh,
        /// <summary>
        /// Cannot create files in a directory that does not exist
        /// </summary>
        Shotel,

        // Informational
        /// <summary>
        /// A file was created
        /// </summary>
        Ballista,
    }
}
