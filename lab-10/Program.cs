using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;

namespace lab10 {
    class MainClass {
        static XElement ReadElement( XmlReader reader ) {
            reader.MoveToContent();
            return ( XElement.ReadFrom( reader ) as XElement );
        }

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

                using( XmlReader reader = XmlReader.Create( parsedArgs.InTrackFile ) ) {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    Statistics stats = new Statistics();

                    settings.Indent = true;
                    settings.CloseOutput = true;
                    settings.Encoding = UTF8Encoding.UTF8;
                    settings.WriteEndDocumentOnClose = true;
                    stats.Init();

                    using( XmlWriter writer = XmlWriter.Create( parsedArgs.OutTrackFile, settings ) ) {
                        GpxPoint previous = null, current;

                        writer.WriteStartDocument();

                        foreach( XmlReader trkReader in ProcessXml.ReadEntries( reader, "trk" ) ) {
                            XmlWriter trkWriter = XmlWriter.Create( writer );

                            trkWriter.WriteStartElement( "trk" );

                            foreach( XmlReader trksegReader in ProcessXml.ReadEntries( trkReader, "trkseg" ) ) {
                                XmlWriter trksegWriter = XmlWriter.Create( trkWriter );

                                trksegWriter.WriteStartElement( "trkseg" );

                                foreach( XmlReader trkptReader in ProcessXml.ReadEntries( trksegReader, "trkpt" ) ) {
                                    current = new GpxPoint( ReadElement( trkptReader ), reader.NamespaceURI );

                                    if( current.Hdop > parsedArgs.HdopLimit )
                                        Console.WriteLine( current + ", hdop " + current.Hdop );
                                    else
                                        ProcessPoint( current, ref previous, ref stats );

                                    current.Entry.WriteTo( trksegWriter );
                                }

                                trksegWriter.WriteEndElement();
                            }
                            
                            trkWriter.WriteEndElement();
                        }

                        writer.Close();
                    }

                    Console.WriteLine( stats );
                }
            } else
                Console.WriteLine( "Wrong arguments passed to the application." );

            return 0;
        }
    }
}
