using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;

namespace Es
{
    public abstract class ESManager
    {
        private static ElasticClient _client = null;

        static ESManager()
        {
        }

        public ElasticClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (new Object())
                    {
                        if (_client == null)
                        {
                            List<Uri> _esNodes = new List<Uri>();
                            _esNodes.Add(new Uri("http://localhost:9200/"));
                            var pool = new SniffingConnectionPool(_esNodes);
                            ConnectionSettings settings = null;
                            settings = new ConnectionSettings(pool).DefaultIndex("product");
                            _client = new ElasticClient(settings);
                        }
                    }
                }
                return _client;
            }
        }
    }
}
