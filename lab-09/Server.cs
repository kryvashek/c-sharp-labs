namespace lab09 {
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public interface ILinkHandler<LinkHandlerSettings> : IDisposable {
        void Run( TcpClient tcpClient, ref LinkHandlerSettings settings );
    }

    public class Server< LinkHandler, LinkHandlerSettings > : TcpListener, IDisposable
        where LinkHandler : ILinkHandler< LinkHandlerSettings >, new()
    {
        LinkHandlerSettings _clientProcessorSettings;
        int _extraThreadsCountMax;
        int _extraThreadsCountMin;

        public Server( int portNumber, ref LinkHandlerSettings clientProcessorSettings, int extraThreads = 1 ) : base( IPAddress.Any, portNumber ) {
            _clientProcessorSettings = clientProcessorSettings;
            _extraThreadsCountMax = _extraThreadsCountMin = extraThreads;
            changeThreads( true );
            Start();
        }

        public void Run() {
            while( true ) // wait for next connected client and add its data to processing queue
                ThreadPool.QueueUserWorkItem( new WaitCallback( ProcessorThread ), AcceptTcpClient() );
        }

        public void Dispose() {
            Stop();
            changeThreads( false );
        }

        void changeThreads( bool atStart ) {
            int startWorkersCountMin, startWorkersCountMax, asyncIOTasks;

            // change (increase at start, decrease on dispose) worker threads counts
            ThreadPool.GetMaxThreads( out startWorkersCountMax, out asyncIOTasks );
            ThreadPool.GetMinThreads( out startWorkersCountMin, out asyncIOTasks );
            ThreadPool.SetMaxThreads( startWorkersCountMax + _extraThreadsCountMax, asyncIOTasks );
            ThreadPool.SetMinThreads( startWorkersCountMin + _extraThreadsCountMin, asyncIOTasks );

            if( atStart ) { // saving real change values to correctly decrease worker threads counts on dispose
                ThreadPool.GetMaxThreads( out _extraThreadsCountMax, out asyncIOTasks );
                _extraThreadsCountMax = startWorkersCountMax - _extraThreadsCountMax;
                ThreadPool.GetMinThreads( out _extraThreadsCountMin, out asyncIOTasks );
                _extraThreadsCountMin = startWorkersCountMin - _extraThreadsCountMin;
            }
        }

        void ProcessorThread( Object tcpLink ) {
            using( LinkHandler processor = new LinkHandler() ) // create new instanse of class to process client`s data
                processor.Run( tcpLink as TcpClient, ref _clientProcessorSettings ); // run processing upon got connection and specified settings

            ( tcpLink as TcpClient ).Close();
        }
    }
}
