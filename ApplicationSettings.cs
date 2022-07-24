using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server
{

    public interface IApplicationSettings
    {
        string STORAGE_ACCOUNT_NAME { get; } 
        string STORAGE_ACCOUNT_KEY { get; }
        string TABLE_URL_FORMAT { get; }
    }

    public class ApplicationSettings : IApplicationSettings
    {   
        public string STORAGE_ACCOUNT_NAME {get; private set;}
        public string STORAGE_ACCOUNT_KEY {get; private set;}
        public string TABLE_URL_FORMAT {get; private set;}

        public ApplicationSettings(){
            STORAGE_ACCOUNT_KEY = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_KEY");
            STORAGE_ACCOUNT_NAME = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME");
            TABLE_URL_FORMAT = "https://{0}.table.core.windows.net/{1}";
        }

 
    }
}