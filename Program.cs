using System;
using System.Linq;

namespace Travelling
{
    class Program
    {
        static void Main(string[] args)
        {
            var ids = Distances.Matrix.Keys
                .Select(k => ushort.Parse(k.Substring(0, 1)))
                .Distinct()
                .ToArray();

            var singleAgents = ids
                .Select(id => new Agent(id))
                .ToArray();

            var agents = singleAgents
                .Select(a => a.With(singleAgents.Where(sa => sa.Id != a.Id).ToArray()))
                .OrderBy(a => a.Id)
                .ToArray();

            var startingAgent = agents.First();

            var message = new Message(startingAgent.Id);

            startingAgent.Recieve(message);

            Console.ReadLine();

            startingAgent
                .GetReturned()
                .ToList()
                .ForEach(m => Console.WriteLine(m.Path + " -> " + m.MileAge));

            Console.ReadLine();
        }
    }
}
