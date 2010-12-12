/*
===========================================================================

Return to Castle Wolfenstein XNA Managed C# Port
Copyright (c) 2010 JV Software
Copyright (C) 1999-2010 id Software LLC, a ZeniMax Media company. 

This file is part of the Return to Castle Wolfenstein XNA Managed C# Port GPL Source Code.  

RTCW C# Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

RTCW C# Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RTCW C# Source Code.  If not, see <www.gnu.org/licenses/>.

In addition, the RTCW SP Source Code is also subject to certain additional terms. 
You should have received a copy of these additional terms immediately following the terms 
and conditions of the GNU General Public License which accompanied the RTCW C# Source Code.  
If not, please request a copy in writing from id Software at the address below.

If you have questions concerning this license or the applicable additional terms, you may contact in writing 
id Software LLC, c/o ZeniMax Media Inc., Suite 120, Rockville, Maryland 20850 USA.

===========================================================================
*/

// parser.cs (c) 2010 id Software
//

using System;

using idLib.Engine.Public;
using idLib.Math;

namespace idLib
{
    //
    // idParser
    //
    public class idParser
    {
        string buffer;
        int parsePosition = 0;
        int lastParsePosition = 0;
        string token = "";
        bool inQuotedString = false;
        bool inLongComment = false;
        bool lineBreakFound = false;
        bool lastStrInQuote = false;

        //
        // idParser
        //
        public idParser(idFile file)
        {
            buffer = new string(file.ReadChars(file.Length()));
        }

        public idParser(string parsebuffer)
        {
            buffer = parsebuffer;
        }

        //
        // UngetToken
        //
        public void UngetToken()
        {
            parsePosition = lastParsePosition;
        }

        //
        // ReachedEndOfBuffer
        //
        public bool ReachedEndOfBuffer
        {
            get
            {
                if (parsePosition >= buffer.Length)
                {
                    return true;
                }

                return false;
            }
        }

        //
        // NextToken
        //
        public string NextToken
        {
            get
            {
                return GetNextToken();
            }
        }

        //
        // CurrentToken
        //
        public string CurrentToken
        {
            get
            {
                return token;
            }
        }

        //
        // NextBracketSection
        //
        public string NextBracketSection
        {
            get
            {
                return ParseBracketSection();
            }
        }

        public void NextInt2(ref int[] i)
        {
            if (NextToken != "(")
            {
                throw new Exception("Expected open brace");
            }

            i[0] = NextInt;
            i[1] = NextInt;

            if (NextToken != ")")
            {
                throw new Exception("Expected closing brace");
            }
        }

        public void NextVector3(ref idVector3 v)
        {
            if (NextToken != "(")
            {
                throw new Exception("Expected open brace");
            }

            v.X = NextFloat;
            v.Y = NextFloat;
            v.Z = NextFloat;

            if (NextToken != ")")
            {
                throw new Exception("Expected closing brace");
            }
        }

        //
        // NextInt
        //
        public int NextInt
        {
            get
            {
                return Int32.Parse(NextToken);
            }
        }

        //
        // NextShort
        //
        public short NextShort
        {
            get
            {
                return Int16.Parse(NextToken);
            }
        }

        //
        // NextLong
        //
        public long NextLong
        {
            get
            {
                return Int64.Parse(NextToken);
            }
        }

        //
        // NextFloat
        //
        public float NextFloat
        {
            get
            {
                return Single.Parse(NextToken);
            }
        }

        public void ExpectNextToken(string s)
        {
            if (NextToken != s)
            {
                throw new Exception("Expected token " + s);
            }
        }

        //
        // ParseBracketSection
        //
        private string ParseBracketSection()
        {
            string token;
            bool inInternalBracketSection = false;

            if (NextToken != "{")
            {
                UngetToken();
                throw new Exception("ParseBracketSection: Expected open bracket but found " + NextToken + " Following Token " + NextToken);
            }

            token = "";

            while (true)
            {
                char c;

                if (ReachedEndOfBuffer == true)
                {
                    throw new Exception("ParseBracketSection: Expected closing bracket");
                }

                // Check for comments in the bracket section.

                c = GetNextUncheckedChar();
                if (c == '/')
                {
                    c = GetNextUncheckedChar();
                    if (c == '/')
                    {
                        ParseRestOfLine();
                    }
                    else
                    {
                        parsePosition-=2;
                    }
                    c = GetNextUncheckedChar();
                }

                if (c == '{')
                {
                    inInternalBracketSection = true;
                }

                if (c == '}')
                {
                    if (inInternalBracketSection == false)
                    {
                        break;
                    }
                    else
                    {
                        inInternalBracketSection = false;
                    }
                }

                token += c;
            }

            return token;
        }

        //
        // GetNextUncheckedChar
        //
        private char GetNextUncheckedChar()
        {
            return buffer[parsePosition++];
        }

        //
        // GetNextChar
        //
        private bool GetNextChar(ref char c)
        {
            if( parsePosition >= buffer.Length )
            {
                c = '\0';
                return false;
            }

            lineBreakFound = false;

            c = GetNextUncheckedChar();

            if (inQuotedString == false)
            {
                if (c == ' ' || c == '\n' || c == '\b' || c == '\t' || c == '\r')
                {
                    // If the token size isn't 0 than we have a full token, if not we hit a space or a continous
                    // blank space, if thats the case continue to call get nextchar 
                    if( token.Length > 0 )
                    {
                        if (c == '\n' || c == '\r')
                        {
                            lineBreakFound = true;
                        }

                        // Check trailing spaces to see if there is a new line following it.
                        int currentPosition = parsePosition;
                        while (c == ' ' || c == '\t')
                        {
                            if (ReachedEndOfBuffer == true)
                            {
                                break;
                            }
                            c = GetNextUncheckedChar();

                            if (c == '\n' || c == '\r')
                            {
                                lineBreakFound = true;
                                break;
                            }
                        }

                        if (lineBreakFound == false)
                        {
                            parsePosition = currentPosition;
                        }

                        return false;
                    }
                    else
                    {
                        return GetNextChar( ref c );
                    }
                }

                if (c == '"')
                {
                    inQuotedString = true;
                    lastStrInQuote = true;
                    return GetNextChar(ref c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotedString = false;
                    return false;
                }
            }


            return true;
        }

        //
        // ParseRestOfLine
        //
        public void ParseRestOfLine()
        {
            token = "";
            for (; parsePosition < buffer.Length; parsePosition++)
            {
                if (buffer[parsePosition] == '\n' || buffer[parsePosition] == '\b' || buffer[parsePosition] == '\r')
                {
                    break;
                }
                token += buffer[parsePosition];
            }
        }

        //
        // GetNextTokenFromLine
        //
        public string GetNextTokenFromLine()
        {
            if (lineBreakFound == true)
            {
                return null;
            }

            return GetNextToken();
        }

        //
        // GetNextTokenFromLine
        //
        public string GetNextTokenFromLineChecked()
        {
            if (lineBreakFound == true)
            {
                throw new Exception("Expected token on line");
            }

            return GetNextToken();
        }

        //
        // GetNextToken
        //
        private string GetNextToken()
        {
            char c = '\0';

            token = "";
            lastStrInQuote = false;

            // If were passed the length of the buffer, this means we have reached the end of the stream.
            if (parsePosition >= buffer.Length)
            {
                if (inLongComment)
                {
                    throw new Exception("Long comment without closing token");
                }
                return null;
            }

            // Store the position were we are at before we found the next token.
            lastParsePosition = parsePosition;

            // Create a valid token
            while (GetNextChar(ref c))
            {
                token += c;

                if (token == "//")
                {
                    ParseRestOfLine();
                    token = "";
                }
            }

            if (token == "/*")
            {
                inLongComment = true;
                return GetNextToken();
            }

            if (token == "*/")
            {
                inLongComment = false;
                return GetNextToken();
            }

            if (inLongComment == true)
            {
                return GetNextToken();
            }

            // If this token is null than we have probley reached EOF.
            if (token.Length <= 0)
            {
               // if (ReachedEndOfBuffer == false && ReachedEndOfBuffer == false && lastStrInQuote == false)
              //  {
               //     return GetNextToken();
               // }
                return null;
            }

            return token;
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            buffer = null;
        }
    }
}
