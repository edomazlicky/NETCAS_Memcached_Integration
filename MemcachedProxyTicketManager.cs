/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;
using System.Web;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using DotNetCasClient.Logging;
using DotNetCasClient.State;

namespace Cas.Integration.Memcached
{
    ///<summary>
    /// This IProxyTicketManager implementation relies on the Memcached for ticket 
    /// storage.  This means tickets can be stored in a distributed caching environment and are
    /// available even in clustered and load balanced/round-robin configurations
    ///</summary>
    /// <author>Eric Domazlicky</author>
    public class MemcachedProxyTicketManager : IProxyTicketManager
    {
        private static readonly TimeSpan DefaultExpiration = new TimeSpan(0, 0, 3, 0); // 180 seconds
        private MemcachedClient client;

        private static readonly Logger log = new Logger(Category.Security);

        /// <summary>
        /// Retrieve standard memcached section from config file
        /// </summary>
        public void Initialize()
        {
            client = new MemcachedClient("enyim.com/memcached");
            log.Debug("Memcached client inited");
        }

        /// <summary>
        /// Removes expired PGTIOU-PGT from the ticket store
        /// </summary>
        public void RemoveExpiredMappings()
        {
            // No-op.  Memcached removes expired entries automatically.
        }

        /// <summary>
        /// Method to save the ProxyGrantingTicket to the backing storage facility.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <param name="proxyGrantingTicket">used as the value</param>
        public void InsertProxyGrantingTicketMapping(string proxyGrantingTicketIou, string proxyGrantingTicket)
        {
            log.Debug("Memcached Storing PGT-IOU:" + proxyGrantingTicketIou);
            client.Store(StoreMode.Set, proxyGrantingTicketIou, proxyGrantingTicket, DateTime.Now.Add(DefaultExpiration));             
        }

        /// <summary>
        /// Method to retrieve a ProxyGrantingTicket based on the
        /// ProxyGrantingTicketIou.  Implementations are not guaranteed to
        /// return the same result if retieve is called twice with the same 
        /// proxyGrantingTicketIou.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <returns>the ProxyGrantingTicket Id or null if it can't be found</returns>
        public string GetProxyGrantingTicket(string proxyGrantingTicketIou)
        {
            log.Debug("Memcached Getting PGT-IOU:" + proxyGrantingTicketIou);
            if (client.Get(proxyGrantingTicketIou) != null && client.Get(proxyGrantingTicketIou).ToString().Length > 0)
            {
                log.Debug("Memcached Found PGT-IOU:" + proxyGrantingTicketIou);
                return client.Get(proxyGrantingTicketIou).ToString();
            }
            log.Debug("Memcached Not found PGT-IOU:" + proxyGrantingTicketIou);
            return null;
        }
    }
}
