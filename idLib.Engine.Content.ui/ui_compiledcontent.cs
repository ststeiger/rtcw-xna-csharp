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
        public idUserInterfaceCachedAssets assets = new idUserInterfaceCachedAssets();
        public idUserInterfaceMenuDef menudef = new idUserInterfaceMenuDef();

        //
        // ParseAsset
        //
        private void ParseAsset(ref idUserInterfaceFile ui)
        {
            if (ui.ReachedEndOfBuffer)
                throw new Exception("EOF at start of asset");

            if (ui.NextToken != "{")
                throw new Exception("UI asset expected open bracket");

            while (true)
            {
                string token;

                if (ui.ReachedEndOfBuffer)
                    throw new Exception("Unexpected EOF in asset expected closing bracket first");

                token = ui.NextToken.ToLower();

                if (token == "}")
                {
                    break;
                }
                else if (token == "font")
                {
                    assets.textFont = ui.GetNextTokenFromLineChecked();
                    assets.textFontSize = int.Parse(ui.GetNextTokenFromLineChecked());
                }
                else if (token == "smallfont")
                {
                    assets.smallFont = ui.GetNextTokenFromLineChecked();
                    assets.smallFontSize = int.Parse(ui.GetNextTokenFromLineChecked());
                }
                else if (token == "bigfont")
                {
                    assets.bigFont = ui.GetNextTokenFromLineChecked();
                    assets.bigFontSize = int.Parse(ui.GetNextTokenFromLineChecked());
                }
                else if (token == "handwritingfont")
                {
                    assets.handwritingFont = ui.GetNextTokenFromLineChecked();
                    assets.handwritingFontSize = int.Parse(ui.GetNextTokenFromLineChecked());
                }
                else if (token == "gradientbar")
                {
                    assets.gradientBar = ui.GetNextTokenFromLineChecked();
                }
                else if ( token == "menuentersound" ) {
			        assets.menuEnterSound = ui.GetNextTokenFromLineChecked();
		        }

		        // exitMenuSound
		        else if ( token == "menuexitsound" ) {
                    assets.menuExitSound = ui.GetNextTokenFromLineChecked();
			        continue;
		        }

		        // itemFocusSound
		        else if ( token == "itemfocussound" ) {
                    assets.itemFocusSound = ui.GetNextTokenFromLineChecked();
			        continue;
		        }

		        // menuBuzzSound
		        else if ( token == "menubuzzsound" ) {
                    assets.menuBuzzSound = ui.GetNextTokenFromLineChecked();
			        continue;
		        }

		        else if ( token == "cursor" ) {
                    assets.cursor = ui.GetNextTokenFromLineChecked();
			        continue;
		        }

		        else if ( token == "fadeclamp" ) {
                    assets.fadeClamp = float.Parse(ui.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "fadecycle" ) {
                    assets.fadeCycle = int.Parse(ui.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "fadeamount" ) {
                    assets.fadeAmount = float.Parse(ui.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "shadowx" ) {
                    assets.shadowX = float.Parse(ui.GetNextTokenFromLineChecked());
			        continue;
		        }

		        else if ( token == "shadowy" ) {
                    assets.shadowY = float.Parse(ui.GetNextTokenFromLineChecked());
			        continue;
		        }

                else if (token == "shadowcolor")
                {
                    assets.shadowFadeClamp = MenuParser.ParseColor(ref ui)[3];
                    continue;
                }
                else
                {
                    throw new Exception("Invalid or unexpcted token in asset body " + token);
                }
            }
        }

        

        //
        // idUserInterfaceCompiledContent
        //
        public idUserInterfaceCompiledContent(ref idUserInterfaceFile ui)
        {
            if (ui.NextToken != "{")
            {
                throw new Exception("UI Expected open bracket in main body");
            }
            // Parse the user interface.
            while (true)
            {
                string token;

                if (ui.ReachedEndOfBuffer == true)
                {
                    throw new Exception("Expected closing bracket before EOF");
                }

                token = ui.NextToken;
                if (token.Length <= 0)
                {
                    continue;
                }

                if (token == "}")
                {
                    break;
                }
                else if (token == "assetGlobalDef")
                {
                    ParseAsset(ref ui);
                }
                else if (token == "menuDef" || token == "menudef")
                {
                    MenuParser.ParseMenu(ref menudef, ref assets, ref ui);
                }
                else
                {
                    throw new Exception("Unexpected token " + token + " while parsing main body");
                }
            }
        }
    }
}
