using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace xnacontentbuilder
{
    //
    // idXnaContentImportContext
    //
    public class idXnaContentImportContext : ContentImporterContext
    {
        idXnaContentImportLogger logger = new idXnaContentImportLogger();

        //
        // IntermediateDirectory
        //
        public override string IntermediateDirectory { 
            get { 
                return string.Empty; 
            } 
        }

        //
        // OutputDirectory
        //
        public override string OutputDirectory { 
            get {
                return Program.BASEDIR;
            } 
        }

        //
        // Logger
        //
        public override ContentBuildLogger Logger { 
            get { 
                return logger; 
            } 
        }
        
        //
        // AddDependency
        //
        public override void AddDependency(string filename) { 
        
        }
    }

    //
    // idXnaContentImportLogger
    //
    class idXnaContentImportLogger : ContentBuildLogger
    {
        public override void LogMessage(string message, params object[] messageArgs)
        {
            Program.Print(message);
        }

        public override void LogImportantMessage(string message, params object[] messageArgs)
        {
            Program.PrintColor(message, ConsoleColor.Cyan);
        }

        public override void LogWarning(string helpLink, ContentIdentity contentIdentity, string message, params object[] messageArgs)
        {
            Program.Warning(message);
        }
    }
}
