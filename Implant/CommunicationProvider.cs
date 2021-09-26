using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Interface for communication. Right now only HTTP implemented but I need to make DNS/WebSocket/Other?
namespace Test23
{
 
    interface CommunicationProvider
    {

        public void keyExchangeSetup();

        public string getConfig();

        public string getNextTask();

        public void updateTaskResult();

        public string sendEncryptedRequest(string body, string endpoint);
    }
}
