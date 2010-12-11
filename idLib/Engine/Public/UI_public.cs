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
            idUserInterfaceMenuDef menu = new idUserInterfaceMenuDef();

            BinaryReader reader = (BinaryReader)input;
            assets.ReadBinaryFile(ref reader);
            menu.ReadBinaryFile(ref reader);

            return Engine.ui.LoadUIFromMemory(assets, menu);
        }
    }

    //
    // idUserInterface
    //
    public abstract class idUserInterface
    {
        public abstract string GetName();
        public abstract void Draw();
    }

    //
    // idUserInterfaceManager
    //
    public abstract class idUserInterfaceManager
    {
        // Init the user interface manager.
        public abstract void Init();
        public abstract idUserInterface LoadUIFromMemory(idUserInterfaceCachedAssets assets, idUserInterfaceMenuDef menu);
    }
}
