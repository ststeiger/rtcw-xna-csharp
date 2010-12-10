// ui_compiledcontent.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceCompiledContent
    //
    public class idUserInterfaceCompiledContent
    {
        idUserInterfaceCachedAssets assets = new idUserInterfaceCachedAssets();

        /*
        =================
        PC_Color_Parse
        =================
        */
        private idVector4 ParseColor(ref idUserInterfaceFile ui)
        {
            int i;
            float f;
            idVector4 c = new idVector4();

            for (i = 0; i < 4; i++)
            {
                c[i] = float.Parse(ui.Parser.GetNextTokenFromLineChecked());
            }
            return c;
        }

        //
        // ParseAsset
        //
        private void ParseAsset(ref idUserInterfaceFile ui)
        {
            if (ui.Parser.ReachedEndOfBuffer)
                throw new Exception("EOF at start of asset");

            if (ui.Parser.NextToken != "{")
                throw new Exception("UI asset expected open bracket");

            while (true)
            {
                string token;

                if (ui.Parser.ReachedEndOfBuffer)
                    throw new Exception("Unexpected EOF in asset expected closing bracket first");

                token = ui.Parser.NextToken;

                if (token == "}")
                {
                    break;
                }
                else if (token == "font")
                {
                    assets.textFont = ui.Parser.GetNextTokenFromLineChecked();
                    assets.textFontSize = int.Parse(ui.Parser.GetNextTokenFromLineChecked());
                }
                else if (token == "smallFont")
                {
                    assets.smallFont = ui.Parser.GetNextTokenFromLineChecked();
                    assets.smallFontSize = int.Parse(ui.Parser.GetNextTokenFromLineChecked());
                }
                else if (token == "bigFont")
                {
                    assets.bigFont = ui.Parser.GetNextTokenFromLineChecked();
                    assets.bigFontSize = int.Parse(ui.Parser.GetNextTokenFromLineChecked());
                }
                else if (token == "handwritingFont")
                {
                    assets.handwritingFont = ui.Parser.GetNextTokenFromLineChecked();
                    assets.handwritingFontSize = int.Parse(ui.Parser.GetNextTokenFromLineChecked());
                }
                else if (token == "gradientbar")
                {
                    assets.gradientBar = ui.Parser.GetNextTokenFromLineChecked();
                }
                else if ( token == "menuEnterSound" ) {
			        assets.menuEnterSound = ui.Parser.GetNextTokenFromLineChecked();
		        }

		        // exitMenuSound
		        else if ( token == "menuExitSound" ) {
                    assets.menuExitSound = ui.Parser.GetNextTokenFromLineChecked();
			        continue;
		        }

		        // itemFocusSound
		        else if ( token == "itemFocusSound" ) {
                    assets.itemFocusSound = ui.Parser.GetNextTokenFromLineChecked();
			        continue;
		        }

		        // menuBuzzSound
		        else if ( token == "menuBuzzSound" ) {
                    assets.menuBuzzSound = ui.Parser.GetNextTokenFromLineChecked();
			        continue;
		        }

		        else if ( token == "cursor" ) {
                    assets.cursor = ui.Parser.GetNextTokenFromLineChecked();
			        continue;
		        }

		        else if ( token == "fadeClamp" ) {
                    assets.fadeClamp = float.Parse(ui.Parser.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "fadeCycle" ) {
                    assets.fadeCycle = int.Parse(ui.Parser.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "fadeAmount" ) {
                    assets.fadeAmount = float.Parse(ui.Parser.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "shadowX" ) {
                    assets.shadowX = float.Parse(ui.Parser.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "shadowY" ) {
                    assets.shadowY = float.Parse(ui.Parser.GetNextTokenFromLineChecked());
			        continue;
		        }

                else if (token == "shadowColor")
                {
                    assets.shadowFadeClamp = ParseColor(ref ui)[3];
                    continue;
                }
                else
                {
                    throw new Exception("Invalid or unexpcted token in asset body " + token);
                }
            }
        }

        //
        // ParseMenu
        //
        private void ParseMenu(ref idUserInterfaceFile ui)
        {

        }

        //
        // idUserInterfaceCompiledContent
        //
        public idUserInterfaceCompiledContent(ref idUserInterfaceFile ui)
        {
            // Parse the user interface.
            while (true)
            {
                string token;

                if (ui.Parser.ReachedEndOfBuffer == true)
                {
                    throw new Exception("Expected closing bracket before EOF");
                }

                token = ui.Parser.NextToken;

                if (token == "}")
                {
                    break;
                }
                else if (token == "assetGlobalDef")
                {
                    ParseAsset(ref ui);
                }
                else if (token == "menudef")
                {
                    ParseMenu(ref ui);
                }
                else
                {
                    throw new Exception("Unexpected token " + token + " while parsing main body");
                }
            }
        }
    }
}
