using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using Fclp;

namespace lab10 {
    class MainClass {
        private static XmlReader CreateReader( ref object source ) {
            if( source is string )
                return XmlReader.Create( source as string );
            else if( source is TextReader )
                return XmlReader.Create( source as TextReader );
            else if( source is Stream )
                return XmlReader.Create( source as Stream );
            else if( source is XElement )
                return ( source as XElement ).CreateReader();
            else if( source is XmlReader )
                return ( source as XmlReader );
            else
                return null;
        }

        private delegate bool CheckerMethod( string name );

        private static CheckerMethod CreateChecker( ref object checkData ) {
            if( checkData is string )
                return ( checkData as string ).Equals;
            else if( checkData is ICollection< string > )
                return ( checkData as ICollection< string > ).Contains;
            else
                return null;
        }

        static IEnumerable< XmlReader > ReadEntries( object source, object checkData ) {
            using( XmlReader reader = CreateReader( ref source ) ) {
                CheckerMethod checker = CreateChecker( ref checkData );
                reader.MoveToContent();

                while( reader.Read() )
                    if( XmlNodeType.Element == reader.NodeType && checker( reader.Name ) )
                        yield return reader.ReadSubtree();
            }
        }

        static XElement ReadElement( XmlReader reader ) {
            reader.MoveToContent();
            return ( XElement.ReadFrom( reader ) as XElement );
        }

        public struct Statistics {
            public double  
            Distance,
            Time,
            MinSpeed,
            MaxSpeed,
            AvdSpeed;

            public void Init() {
                Distance = .0;
                Time = .0;
                MinSpeed = double.MaxValue;
                MaxSpeed = .0;
                AvdSpeed = .0;
            }

            override public string ToString() {
                StringBuilder text = new StringBuilder( "Track statistics:\n" );
                text.AppendLine( "Total distance: " + Distance.ToString( "N1" ) );
                text.AppendLine( "Total time: " + Time.ToString( "N2" ) );
                text.AppendLine( "Average velocity (by time): " + ( Distance / Time ).ToString( "N1" ) );
                text.AppendLine( "Average velocity (by distance): " + ( AvdSpeed / Distance ).ToString( "N1" ) );
                text.AppendLine( "Minimum velocity: " + MinSpeed.ToString( "N2" ) );
                text.AppendLine( "Maximum velocity: " + MaxSpeed.ToString( "N1" ) );
                return text.ToString();
            }
        }

        public static void ProcessPoint( GpxPoint current, ref GpxPoint previous, ref Statistics stats ) {
            if( null != previous ) {
                current.Previous = previous;
                stats.Distance += current.Distance;
                stats.Time += ( current.Moment - previous.Moment ).TotalSeconds;
                stats.AvdSpeed += current.Velocity * current.Distance;

                if( current.Velocity < stats.MinSpeed )
                    stats.MinSpeed = current.Velocity;

                if( current.Velocity > stats.MaxSpeed )
                    stats.MaxSpeed = current.Velocity;
            }

            previous = current;
            Console.WriteLine( current + ", distance " + current.Distance + ", velocity " + current.Velocity );
        }

        public static int Main( string[] args ) {
            double  hdopLimit = 8.0;
            string  trackFile = "";
            var     clp = new FluentCommandLineParser();

            clp.Setup< double >( 'h', "hdop" )
                .Callback( value => hdopLimit = value )
                .SetDefault( 8.0 );

            clp.Setup< string >( 't', "track" )
                .Callback( value => trackFile = value )
                .Required();

            if( clp.Parse( args ).HasErrors )
                Console.WriteLine( "Wrong arguments passed to the application." );
            else
                using( XmlReader reader = XmlReader.Create( trackFile ) ) {
                    GpxPoint    previous = null,
                                current;
                    Statistics stats = new Statistics();

                    stats.Init();

                    foreach( XmlReader trkReader in ReadEntries( reader, "trk" ) )
                        foreach( XmlReader trksegReader in ReadEntries( trkReader, "trkseg" ) )
                            foreach( XmlReader trkptReader in ReadEntries( trksegReader, "trkpt" ) ) {
                                current = new GpxPoint( ReadElement( trkptReader ), reader.NamespaceURI );

                                if( current.Hdop > hdopLimit )
                                    Console.WriteLine( current + ", hdop " + current.Hdop );
                                else
                                    ProcessPoint( current, ref previous, ref stats );
                            }

                    Console.WriteLine( stats );
                }

            return 0;
        }
    }
}
