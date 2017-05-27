using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGenericContext.Settings
{
    /// <summary>
    /// configuration object to handle request and storage of Retry Settings
    /// </summary>
    public class RetryConnectionSettings
    {
        /// <summary>
        /// GET:SET: the number of retry attempts allowed
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// GET:SET: The initial interval that will apply for the first retry.
        /// </summary>
        public int InitialInterval { get; set; }

        /// <summary>
        /// GET:SET: The incremental time value that will be used to calculate the progressive delay
        //  between retries.
        /// </summary>
        public int Increment { get; set; }
    }
}