using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

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

        public static void Main( string[] args ) {
            using( XmlReader reader = XmlReader.Create( "/home/kryvashek/SampleTrack.gpx" ) ) {
                foreach( XmlReader trkReader in ReadEntries( reader, "trk" ) )
                    foreach( XmlReader trksegReader in ReadEntries( trkReader, "trkseg" ) )
                        foreach( XmlReader trkptReader in ReadEntries( trksegReader, "trkpt" ) )
                            Console.WriteLine( new GpxPoint( ReadElement( trkptReader ), reader.NamespaceURI ) );
            }
        }
    }
}
