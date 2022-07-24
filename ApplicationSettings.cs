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
            //TODO: Load from host.settings.json and get settings deployed to function app.
            STORAGE_ACCOUNT_KEY = "crtflSMKFnKWIhJglDTq3e/HUCvezkUFD/zyYiAwoNY5PH8XgsF0c9Wvogfec1c55NXMLmi2HCPo+AStMHQyHg==";
            STORAGE_ACCOUNT_NAME = "fntemps";
            TABLE_URL_FORMAT = "https://{0}.table.core.windows.net/{1}";
        }

 
    }
}