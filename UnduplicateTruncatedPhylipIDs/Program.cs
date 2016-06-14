using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnduplicateTruncatedPhylipIDs
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Missing filename.");
                return;
            }

            // load sequences from file
            var sequences = new List<Bio.ISequence>();
            try
            {
                var file = System.IO.File.OpenRead(Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + args[0]);
                var parser = new Bio.IO.FastA.FastAParser();
                foreach (var seq in parser.Parse(file))
                    sequences.Add(seq);
            }
            catch
            {
                Console.WriteLine("Error reading file.");
                return;
            }

            // count the number of occurrences of each sequence ID
            var counts = new Dictionary<string, int>();
            foreach (var seq in sequences)
            {
                if (counts.ContainsKey(seq.ID.Remove(10)))
                    counts[seq.ID.Remove(10)]++;
                else
                    counts.Add(seq.ID.Remove(10), 1);
            }

            // extract all sequences with duplicate IDs
            var dupIdSeqs = new List<Bio.ISequence>();
            foreach (var seq in sequences)
            {
                if (counts[seq.ID.Remove(10)] > 1)
                    dupIdSeqs.Add(seq);
            }

            // adjust ID of duplicate ID sequences to be unique
            foreach (var dupseq in dupIdSeqs)
                dupseq.ID = dupseq.ID.Remove(10).Substring(3) + "_" + counts[dupseq.ID.Remove(10)]--;

            // save sequences to file
            try
            {
                using (var file = System.IO.File.Create(Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + args[0] + ".dups.fas"))
                {
                    var formatter = new Bio.IO.FastA.FastAFormatter();
                    formatter.Format(file, dupIdSeqs);
                }
            }
            catch
            {
                Console.WriteLine("Error writing file.");
                return;
            }
        }
    }
}
