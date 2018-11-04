using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowzzBuzz {
    internal class Program {
        private static async Task Main(string[] args) {

            //FizzBuzz implementation using TPL-DataFlow (mutating version)

            var pipe = new TransformBlock<int, FizzBuzz>(x => new FizzBuzz(x));

            var fizz = new TransformBlock<FizzBuzz, FizzBuzz>(x => {
                x.IsFizz = x.Number % 3 == 0;
                return x;
            });

            var buzz = new TransformBlock<FizzBuzz, FizzBuzz>(x => {
                x.IsBuzz = x.Number % 5 == 0;
                return x;
            });

            var setText = new TransformBlock<FizzBuzz, FizzBuzz>(x => {
                if (!x.IsFizz && !x.IsBuzz) x.Text = x.Number.ToString();
                if (x.IsFizz) x.Text += "data";
                if (x.IsBuzz) x.Text += "flow";
                return x;
            });

            var output = new ActionBlock<FizzBuzz>(x => Console.WriteLine(x));

            var options = new DataflowLinkOptions() {
                PropagateCompletion = true
            };

            pipe.LinkTo(fizz, options);
            fizz.LinkTo(buzz, options);
            buzz.LinkTo(setText, options);
            setText.LinkTo(output, options);

            foreach (var i in Enumerable.Range(1, 100)) {
                await pipe.SendAsync(i);
            }

            pipe.Complete();

            await output.Completion;

            Console.WriteLine("Done");
            Console.ReadKey();

        }
    }

    internal class FizzBuzz {

        public FizzBuzz(int number) {
            Number = number;
        }
        public int Number { get; }

        public bool IsFizz { get; set; } = false;
        public bool IsBuzz { get; set; } = false;

        public bool IsNumber { get; set; } = true;

        public string Text { get; set; } = "";

        public override string ToString() => Text;
    }
}
