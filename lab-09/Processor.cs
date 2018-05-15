namespace lab09 {
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Net.Sockets;
    using System.Collections.Generic;
    using PeterO.Numbers;

    public struct ProcessorRunSettings {
        public int BufferSize; // size of buffer being used to read input stream
        public int ExpressionLengthLimit; // maximum size of expression being processed
    }

    public class Processor : ILinkHandler< ProcessorRunSettings > {
        enum ExpAtomType {
            None, Numeric, Operation
        };

        enum OperationType {
            None, Addition, Substraction, Multiplication, Division
        };

        class ExpAtom {
            ExpAtomType _type;
            EDecimal _value;
            OperationType _operation;

            public ExpAtom( string source, ExpAtomType type ) {
                _type = type;
                _value = EDecimal.NaN;
                _operation = OperationType.None;

                if( ExpAtomType.Numeric == type ) {
                    try {
                        _value = EDecimal.FromString( source );
                    } catch {
                        _type = ExpAtomType.None;
                    }
                } else if( 1 != source.Length )
                    _type = ExpAtomType.None;
                else
                    switch( source[ 0 ] ) {
                        case '+': _operation = OperationType.Addition; break;
                        case '-': _operation = OperationType.Substraction; break;
                        case '/': _operation = OperationType.Division; break;
                        case '*': _operation = OperationType.Multiplication; break;
                        default: _operation = OperationType.None; _type = ExpAtomType.None; break;
                    }
            }

            public ExpAtom( EDecimal value ) {
                _type = ExpAtomType.Numeric;
                _value = value;
                _operation = OperationType.None;

            }

            public OperationType Operation {
                get { return _operation; }
            }

            public EDecimal Value {
                get { return _value; }
            }

            public ExpAtomType AtomType {
                get { return _type; }
            }
        }

        TcpClient _tcpClient;
        StringBuilder _reqStringBuilder; // builder to create request string
        ProcessorRunSettings _settings; // settings being used during one processor run
        Regex _regExpNumber; // to read numbers from the input
        Regex _regExpOperation; // to read operations from the input
        byte[] _reqBuffer; // buffer for reading client`s request from socket
        int _reqPartSize; // to count bytes got from client in one iteration
        int _expLength; // length of the request part containing one full expression candidate
        EContext _context; // to set precision and rounding

        public Processor() {
            _regExpNumber = new Regex( @"^\s*([\+\-]?\d*[\.,]?\d+)\s*" );
            _regExpOperation = new Regex( @"^\s*([\+\*\/\-])\s*" );
            _context = new EContext( 50, ERounding.HalfUp, 1, 10000, false );
        }

        bool readInputStream() {
            _reqPartSize = _tcpClient.GetStream().Read( _reqBuffer, 0, _settings.BufferSize );
            return 0 < _reqPartSize;
        }

        void appendInputString() {
            _reqStringBuilder.Append( Encoding.UTF8.GetString( _reqBuffer, 0, _reqPartSize ) );
        }

        void trimInputString() {
            int trimCount = 0;

            while( '\n' == _reqStringBuilder[ trimCount ] )
                trimCount++;

            _reqStringBuilder.Remove( 0, trimCount );
        }

        int firstExpCandidateEnd() {
            return _reqStringBuilder.ToString().IndexOf( "\n", StringComparison.InvariantCulture );
        }

        bool inputHasFirstExpCandidate() {
            return 0 < _expLength;
        }

        bool firstExpCandidateIsTooLong() {
            return _settings.ExpressionLengthLimit <= _reqStringBuilder.Length;
        }

        string getFirstExpCandidate() {
            char[] expChars = new char[ _expLength ];

            _reqStringBuilder.CopyTo( 0, expChars, 0, _expLength );
            _reqStringBuilder.Remove( 0, _expLength + 1 ); // including '\n' ending
            _expLength = -1;

            return new string( expChars );
        }

        bool parseExpCandidate( ref string expCandidate, IList<ExpAtom> sequence ) {
            int position = 0;
            bool numberNow = true; // what to read from string: number or operation
            Match match;
            ExpAtom expAtom;

            Console.WriteLine( "Parsing expression candidate '{0}'", expCandidate );

            do {
                Console.WriteLine( "Retrieving first {1} from '{0}'", expCandidate.Substring( position ), ( numberNow ? "numeric" : "operation" ) );
                match = ( numberNow ? _regExpNumber : _regExpOperation ).Match( expCandidate.Substring( position ) );

                if( !match.Success ) {
                    Console.WriteLine( "Matching failed" );
                    break;
                }

                Console.WriteLine( "Matched substring '{0}'", match.Value );

                if( 2 < match.Groups.Count || 1 != match.Groups[ 1 ].Captures.Count ) {
                    Console.WriteLine( "Wrong match groups or captures count" );
                    break;
                }

                Console.WriteLine( "Captured substring '{0}'", match.Groups[ 1 ].Captures[ 0 ].Value );

                expAtom = new ExpAtom( match.Groups[ 1 ].Captures[ 0 ].Value, ( numberNow ? ExpAtomType.Numeric : ExpAtomType.Operation ) );

                if( ExpAtomType.None == expAtom.AtomType ) {
                    Console.WriteLine( "Convertation error" );
                    break;
                }
                    
                sequence.Add( expAtom );
                position += match.Length;
                numberNow ^= true;
            } while( position < expCandidate.Length );

            return ( !numberNow && position == expCandidate.Length );
        }

        EDecimal performCalculation( EDecimal value1, OperationType operation, EDecimal value2 ) {
            Console.WriteLine( "Performing operation {0}{1}{2}", value1.ToPlainString(), operationLetter( operation ), value2.ToPlainString() );
            switch( operation ) {
                case OperationType.Addition: return value1.Add( value2, _context );
                case OperationType.Substraction: return value1.Subtract( value2, _context );
                case OperationType.Division: return value1.Divide( value2, _context );
                case OperationType.Multiplication: return value1.Multiply( value2, _context );
                default: return EDecimal.NaN;
            }
        }

        ExpAtom simplifyTriple( ExpAtom value1, ExpAtom operation, ExpAtom value2 ) {
            return new ExpAtom( performCalculation( value1.Value, operation.Operation, value2.Value ) );
        }

        char operationLetter( OperationType operation ) {
            switch( operation ) {
                case OperationType.Addition: return '+';
                case OperationType.Substraction: return '-';
                case OperationType.Multiplication: return '*';
                case OperationType.Division: return '/';
                default: return '?';
            }
        }

        void printSequence( IList<ExpAtom> sequence ) {
            var builder = new StringBuilder();

            foreach( ExpAtom atom in sequence )
                if( ExpAtomType.Numeric == atom.AtomType )
                    builder.Append( atom.Value.ToPlainString() );
                else if( ExpAtomType.Operation == atom.AtomType )
                    builder.Append( operationLetter( atom.Operation ) );
                else
                    builder.Append( '?' );

            Console.WriteLine( builder );
        }

        void simplifySequenceOperations( IList<ExpAtom> sequence, OperationType operation ) {
            if( OperationType.None != operation )
                for( int index = 0; index < sequence.Count - 2; )
                    if( sequence[ index + 1 ].Operation == operation ) { // if numeric - operation type is None
                        sequence[ index ] = simplifyTriple( sequence[ index ], sequence[ index + 1 ], sequence[ index + 2 ] );
                        sequence.RemoveAt( index + 2 );
                        sequence.RemoveAt( index + 1 );
                        printSequence( sequence );
                    } else
                        index++;
        }

        void simplifySequence( IList<ExpAtom> sequence ) {
            printSequence( sequence );
            simplifySequenceOperations( sequence, OperationType.Division );
            simplifySequenceOperations( sequence, OperationType.Multiplication );
            simplifySequenceOperations( sequence, OperationType.Substraction );
            simplifySequenceOperations( sequence, OperationType.Addition );
        }

        void processFirstExpCandidate() {
            IList<ExpAtom> sequence = new List<ExpAtom>();
            string expCandidate = getFirstExpCandidate();
            string result = "INVALID\n";

            if( parseExpCandidate( ref expCandidate, sequence ) ) {
                simplifySequence( sequence );

                if( 1 == sequence.Count && ExpAtomType.Numeric == sequence[ 0 ].AtomType )
                    result = sequence[ 0 ].Value.ToPlainString() + "\n";
                else
                    Console.WriteLine( "Calculation error" );
            } else
                Console.WriteLine( "Parsing error" );

            byte[] resultBuffer = Encoding.UTF8.GetBytes( result );
            _tcpClient.GetStream().Write( resultBuffer, 0, resultBuffer.Length );
        }

        public void Run( TcpClient tcpClient, ref ProcessorRunSettings settings ) {
            _tcpClient = tcpClient;
            _settings = settings;

            if( _settings.BufferSize > _settings.ExpressionLengthLimit )
                _settings.BufferSize = _settings.ExpressionLengthLimit;

            _reqBuffer = new byte[ _settings.BufferSize ];
            _reqStringBuilder = new StringBuilder();

            while( _tcpClient.Connected && readInputStream() ) {
                appendInputString();
                _expLength = firstExpCandidateEnd();

                while( inputHasFirstExpCandidate() ) {
                    processFirstExpCandidate();
                    _expLength = firstExpCandidateEnd();
                }

                if( firstExpCandidateIsTooLong() )
                    break;
            }

            _reqBuffer = null;
            _reqStringBuilder.Clear();
            _reqStringBuilder = null;
        }

        public void Dispose() {
            if( null != _reqStringBuilder ) {
                _reqStringBuilder.Clear();
                _reqStringBuilder = null;
            }
        }
    }
}
