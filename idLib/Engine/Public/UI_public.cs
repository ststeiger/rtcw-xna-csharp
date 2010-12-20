// UI_public.cs (c) 2010 JV Software
//

using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using idLib.Engine.Content.ui;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Public
{
    //
    // idUserInterfaceContentReader
    //
    public class idUserInterfaceContentReader : ContentTypeReader<idUserInterface>
    {
        //
        // Read
        //
        protected override idUserInterface Read(ContentReader input, idUserInterface existingInstance)
        {
            idUserInterfaceCachedAssets assets = new idUserInterfaceCachedAssets();
            

            BinaryReader reader = (BinaryReader)input;
            assets.ReadBinaryFile(ref reader);

            int numguis = reader.ReadInt32();
            for (int i = 0; i < numguis; i++)
            {
                idUserInterfaceMenuDef menu = new idUserInterfaceMenuDef();
                menu.ReadBinaryFile(ref reader);
                Engine.ui.LoadUIFromMemory(assets, menu);
            }

            return null;
        }
    }

    //
    // idUserInterface
    //
    public abstract class idUserInterface
    {
        public abstract string GetName();
        public abstract void Draw();
        public abstract void HandleMouseEvent(int x, int y);
        public abstract void HandleKeyEvent(keyNum key, bool down);
        public abstract void HorizontalPercentBar(float x, float y, float width, float height, float percent);
        public abstract void SetItemVisible(string name, bool visible);
    }

    //
    // idUserInterfaceManager
    //
    public abstract class idUserInterfaceManager
    {
        // Init the user interface manager.
        public abstract void Init();
        public abstract idUserInterface FindUserInterface(string uiname);
        public abstract idUserInterface LoadUIFromMemory(idUserInterfaceCachedAssets assets, idUserInterfaceMenuDef menu);
    }
}
