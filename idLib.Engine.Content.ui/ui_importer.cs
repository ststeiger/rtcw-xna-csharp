// ui_importer.cs (c) 2010 JV Software
//

using System;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Content.Pipeline;
using idLib;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceFile
    // 
    public class idUserInterfaceFile
    {
        idDict constantdict;
        idParser parser;
        string uifolder;

        //
        // idUSerInterfaceFile
        //
        public idUserInterfaceFile(string filename)
        {
            // Alloc the parser from the stream.
            parser = new idParser(ReadFile(filename));

            constantdict = new idDict();
            uifolder = Path.GetDirectoryName(filename) + "\\";
        }

        //
        // ReadFile
        //
        private string ReadFile(string filename)
        {
            BinaryReader fstream;
            string uibuffer;

            // Load in the entire UI file for parsing.
            fstream = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            uibuffer = new string(fstream.ReadChars((int)fstream.BaseStream.Length));

            // Close the filestream.
            fstream.Close();

            return uibuffer;
        }

        private byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        //
        // ProcessIncludeFile
        //
        private void ProcessIncludeFile(string filename)
        {
            idParser parser = new idParser(ReadFile(filename));

            while (true)
            {
                string nextToken;

                nextToken = parser.NextToken;
                if (nextToken == null)
                {
                    break;
                }

                if (nextToken == "#define")
                {
                    string keyName = parser.NextToken;
                    string val = parser.NextToken;

                    if (val.Contains("0x"))
                    {
                        char[] t = new char[val.Length - 2];
                        val.CopyTo(2, t, 0, val.Length - 2);
                        int vali = int.Parse(new string(t), NumberStyles.AllowHexSpecifier);
                        val = "" + vali;
                    }

                    constantdict.AddKey(keyName, val);
                }
                else
                {
                    throw new Exception("Preprocess unexpected or invalid token " + nextToken);
                }
            }

            parser.Dispose();
        }

        //
        // PreprocessNextToken
        //
        private string PreprocessNextToken(bool nextToken, bool nextTokenFromLine, bool nextTokenFromLineChecked)
        {
            string token = null;
            if (nextToken)
            {
                token = parser.NextToken;
            }
            else if (nextTokenFromLine)
            {
                token = parser.GetNextTokenFromLine();
            }
            else if (nextTokenFromLineChecked)
            {
                token = parser.GetNextTokenFromLineChecked();
            }

            if (token == null)
            {
                return "";
            }
            else if (token == "#include")
            {
                token = parser.GetNextTokenFromLineChecked();
                token = new string(token.ToCharArray(), 3, token.Length - 3);
                
                // Process the include file.
                ProcessIncludeFile(uifolder + token);

                // Parse the next token.
                return PreprocessNextToken(nextToken, nextTokenFromLine, nextTokenFromLineChecked);
            }

            string keyToken = constantdict.FindKey(token);
            if (keyToken != null)
            {
                return keyToken;
            }


            return token;
        }

        public string GetNextBracketedSection()
        {
            return parser.NextBracketSection;
        }

        public string NextToken
        {
            get
            {
                return PreprocessNextToken(true, false, false);
            }
        }

        public string GetNextTokenFromLine()
        {
            return PreprocessNextToken(false, true, false);
        }

        public bool ReachedEndOfBuffer
        {
            get
            {
                return parser.ReachedEndOfBuffer;
            }
        }

        public string GetNextTokenFromLineChecked()
        {
            return PreprocessNextToken(false, false, true);
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            parser.Dispose();
        }
    }

    //
    // idUserInterfaceImporter
    //
    [ContentImporter(".menu", DisplayName = "RTCW UserInterface Importer", DefaultProcessor = "RTCW UserInterface Processor")]
    public class idUserInterfaceImporter : ContentImporter<idUserInterfaceFile>
    {
        public override idUserInterfaceFile Import(string filename, ContentImporterContext context)
        {
            return new idUserInterfaceFile(filename);
        }
    }
}
