using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Syntaq.Falcon.Test.Base.Url
{
    class FakeConfiguration : IConfiguration
    {
        string this[string key] { get { return "default"; } set { } }
        string IConfiguration.this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
       
        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }
}
