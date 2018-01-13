using System;
using System.Xml;
using System.Text;

namespace lab10 {
    public class PartsRunner {
        private object _checkData;
        private string _selfName;
        private ProcessMethodExtended _subMethod;

        public PartsRunner( string selfName, object checkData, ProcessMethodExtended subMethod ) {
            _checkData = checkData;
            _selfName = selfName;
            _subMethod = subMethod;
        }

        public void Run( XmlReader reader, object extraData ) {
            XmlWriter writer = ( extraData as XmlWriter );
            writer.WriteStartElement( _selfName );
            ProcessXml.ProcessEntries( reader, _checkData, _subMethod, writer );
            writer.WriteEndElement();
        }
    }

    class MainClass {
        struct Statistics {
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

        private static void ProcessPoint( GpxPoint current, ref GpxPoint previous, ref Statistics stats ) {
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
            var parsedArgs = new ParsedArgs( args );

            if( parsedArgs.Ready ) {
                Console.WriteLine( parsedArgs );

                using( XmlReader fileReader = XmlReader.Create( parsedArgs.InTrackFile ) ) {
                    XmlWriterSettings   settings = new XmlWriterSettings();

                    settings.Indent = true;
                    settings.CloseOutput = true;
                    settings.Encoding = UTF8Encoding.UTF8;
                    settings.WriteEndDocumentOnClose = true;
                    settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

                    using( XmlWriter fileWriter = XmlWriter.Create( parsedArgs.OutTrackFile, settings ) ) {
                        Statistics  stats = new Statistics();
                        GpxPoint    previous = null;

                        stats.Init();

                        ProcessMethodExtended dealPoint = ( XmlReader entry, object extraData ) => {
                            GpxPoint current = new GpxPoint( ProcessXml.ReadElement( entry ), fileReader.NamespaceURI );

                            if( current.Hdop > parsedArgs.HdopLimit )
                                Console.WriteLine( current + ", hdop " + current.Hdop );
                            else
                                ProcessPoint( current, ref previous, ref stats );

                            current.Entry.WriteTo( extraData as XmlWriter );
                        };

                        var segment = new PartsRunner( "trkseg", "trkpt", dealPoint );
                        var track = new PartsRunner( "trk", "trkseg", segment.Run );

                        fileWriter.WriteStartDocument();
                        fileReader.MoveToContent();
                        fileWriter.WriteStartElement( "gpx", fileReader.NamespaceURI );
                        // xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd">
                        fileWriter.WriteAttributeString( "version", "1.1" );
                        ProcessXml.ProcessEntries( fileReader, "trk", track.Run, fileWriter );
                        fileWriter.WriteEndElement();
                        fileWriter.Close();
                        Console.WriteLine( stats );
                    }
                }
            } else
                Console.WriteLine( "Wrong arguments passed to the application." );

            return 0;
        }
    }
}
