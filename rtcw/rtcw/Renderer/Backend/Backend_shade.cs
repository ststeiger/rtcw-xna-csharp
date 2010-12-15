// Backend_device.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib.Math;
using idLib.Engine.Public;

namespace rtcw.Renderer.Backend
{
    //
    // Shade
    //
    static class Shade
    {
        private static BasicEffect defaultEffect;
        private static idRenderMatrix orthoMatrix;
        private static Vector3 pushedColor;
        private static idRenderMatrix drawMatrix;

        //
        // Init
        //
        public static void Init()
        {
            defaultEffect = new BasicEffect(Globals.graphics3DDevice);
            defaultEffect.TextureEnabled = true;

            orthoMatrix = new idRenderMatrix();
            orthoMatrix.Create2DOrthoMatrix(Engine.RenderSystem.GetViewportWidth(), Engine.RenderSystem.GetViewportHeight());

            pushedColor = new Vector3(1, 1, 1);

            drawMatrix = new idRenderMatrix();
        }

        //
        // Set2DOrthoMode
        //
        public static void Set2DOrthoMode()
        {
            orthoMatrix.SetAsActiveMatrix(ref defaultEffect);
        }

        //
        // CreateTranslateRotateMatrix
        //
        public static void CreateTranslateRotateMatrix(idRenderEntityLocal refdef)
        {
            drawMatrix.SetEntityMatrix(refdef);
            drawMatrix.SetAsActiveMatrix(ref defaultEffect);
        }

        //
        // BindVertexIndexBuffer
        //
        public static void BindVertexIndexBuffer(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            Globals.graphics3DDevice.SetVertexBuffer(vertexBuffer);
            Globals.graphics3DDevice.Indices = indexBuffer;
        }

        //
        // DrawPrimitives
        //
        public static void DrawPrimitives(int numVerts, int numIndexes, idDrawVertex[] verts, short[] indexes)
        {
            
            defaultEffect.DiffuseColor = pushedColor;
            defaultEffect.CurrentTechnique.Passes[0].Apply();
            Globals.graphics3DDevice.DrawUserIndexedPrimitives<idDrawVertex>(PrimitiveType.TriangleList, verts, 0, numVerts, indexes, 0, numIndexes / 3, idDrawVertex.VertexDeclaration);
        }

        //
        // DrawElements
        //
        public static void DrawElements(int startVertex, int numVertexes, int startIndexes, int numIndexes, int offset)
        {
            defaultEffect.DiffuseColor = pushedColor;
            defaultEffect.CurrentTechnique.Passes[0].Apply();
            Globals.graphics3DDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, startVertex, offset, numVertexes, startIndexes, numIndexes / 3);
        }

        //
        // PushProjectionMatrix
        //
        public static void PushDrawMatrix(idRefdefLocal refdef)
        {
            drawMatrix.SetupProjection(refdef, 1000, 1);
            drawMatrix.CreateViewMatrix(refdef);
        }

        //
        // BindImage
        //
        public static void BindImage(idImage image)
        {
            defaultEffect.Texture = (Texture2D)image.GetDeviceHandle();
        }

        //
        // SetColor
        //
        public static void SetColor(float r, float g, float b, float a)
        {
            pushedColor.X = r;
            pushedColor.Y = g;
            pushedColor.Z = b;
        }

        //
        // DrawTess
        //
        public static void DrawTess()
        {
            defaultEffect.DiffuseColor = pushedColor;
            defaultEffect.CurrentTechnique.Passes[0].Apply();
            Globals.graphics3DDevice.DrawUserIndexedPrimitives<idDrawVertex>(PrimitiveType.TriangleList, Globals.tess.drawVerts, 0, Globals.tess.numVertexes, Globals.tess.indexes, 0, Globals.tess.numIndexes / 3, idDrawVertex.VertexDeclaration);
        }

        //
        // SetMaterialStageState
        //
        public static void SetMaterialStageState(shaderStage_t stage)
        {
            if (stage.useBlending == true)
            {
                Globals.graphics3DDevice.BlendState = stage.blendState;
            }
            else
            {
                Globals.graphics3DDevice.BlendState = BlendState.AlphaBlend;
            }
        }
    }
}
