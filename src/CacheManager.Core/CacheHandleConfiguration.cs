﻿using System;
using static CacheManager.Core.Utility.Guard;

namespace CacheManager.Core
{
    /// <summary>
    /// Defines all settings the cache handle should respect.
    /// </summary>
    public sealed class CacheHandleConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHandleConfiguration"/> class.
        /// </summary>
        /// <param name="handleName">Name of the handle.</param>
        /// <exception cref="System.ArgumentNullException">If handleName is null.</exception>
        public CacheHandleConfiguration(string handleName)
        {
            NotNullOrWhiteSpace(handleName, nameof(handleName));

            this.HandleName = handleName;
        }

        /// <summary>
        /// Gets a value indicating whether performance counters should be enabled or not.
        /// <para>
        /// If enabled, and the initialization of performance counters doesn't work, for example
        /// because of security reasons. The counters will get disabled silently.
        /// </para>
        /// </summary>
        /// <value><c>true</c> if performance counters should be enable; otherwise, <c>false</c>.</value>
        public bool EnablePerformanceCounters { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether statistics should be enabled.
        /// </summary>
        /// <value><c>true</c> if statistics should be enabled; otherwise, <c>false</c>.</value>
        public bool EnableStatistics { get; internal set; }

        /// <summary>
        /// Gets the expiration mode.
        /// </summary>
        /// <value>The expiration mode.</value>
        public ExpirationMode ExpirationMode { get; internal set; }

        /// <summary>
        /// Gets the expiration timeout.
        /// </summary>
        /// <value>The expiration timeout.</value>
        public TimeSpan ExpirationTimeout { get; internal set; }

        /// <summary>
        /// Gets the name for the cache handle which is also the identifier of the configuration.
        /// </summary>
        /// <value>The name of the handle.</value>
        public string HandleName { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is back plate source.
        /// <para>
        /// Only one cache handle inside one cache manager can be back plate source. Usually this is
        /// a distributed cache. It might not make any sense to define an in process cache as back
        /// plate source.
        /// </para>
        /// <para>If no back plate is configured for the cache, this setting will have no effect.</para>
        /// </summary>
        /// <value><c>true</c> if this instance should be back plate source; otherwise, <c>false</c>.</value>
        public bool IsBackPlateSource { get; internal set; }

        /// <summary>
        /// Gets or sets the type of the handle.
        /// </summary>
        /// <value>The type of the handle.</value>
        internal Type HandleType { get; set; }
    }
}