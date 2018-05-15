
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;

namespace MathServer {




}

namespace Program {
    using lab09;

    public class Program {

        public static void Main( string[] args ) {
            // some defaults, in production version they can be read from configuration file
            const int clientProcessorsExtraCount = 1;
            const int portNumber = 9876;
            var processorRunSettings = new ProcessorRunSettings { BufferSize = 256, ExpressionLengthLimit = 4096 };

            // create and run server
            using( var server = new Server< Processor, ProcessorRunSettings >( portNumber, ref processorRunSettings, clientProcessorsExtraCount ) )
                server.Run();
        }
    }
}