using System;
using Fclp;

namespace lab10 {
    public class ParsedArgs {
        private string
        _inTrackFile,
        _outTrackFile;

        private double
        _hdopLimit;

        private bool 
        _ready;

        public string InTrackFile { get { return _inTrackFile; } }
        public string OutTrackFile { get { return _outTrackFile; } }
        public double HdopLimit { get { return _hdopLimit; } }
        public bool Ready { get { return _ready; } }

        public ParsedArgs( string[] args ) {
            var clp = new FluentCommandLineParser();

            clp.Setup< string >( 'i', "input-track" )
                .Callback( value => _inTrackFile = value )
                .Required();

            clp.Setup< string >( 'o', "output-track" )
                .Callback( value => _outTrackFile = value )
                .Required();

            clp.Setup< double >( 'h', "hdop" )
                .Callback( value => _hdopLimit = value )
                .SetDefault( 8.0 );

            _ready = !clp.Parse( args ).HasErrors;
        }

        override public string ToString() {
            return "Input track " + InTrackFile + ",\nOutput track " + OutTrackFile + ",\nHDOP limit " + HdopLimit;
        }
    }
}

