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

        private static BlendState blending = BlendState.AlphaBlend;
        private static cullType_t faceCulling = cullType_t.CT_FRONT_SIDED;

        // Dynamic vertex and index buffers for uploading vertex and index data.
        private static DynamicVertexBuffer dynVertexBuffer;
        private static DynamicIndexBuffer dynIndexBuffer;

        //
        // Init
        //
        public static void Init()
        {
            defaultEffect = new BasicEffect(Globals.graphics3DDevice);
            defaultEffect.TextureEnabled = true;

            orthoMatrix = new idRenderMatrix();
            orthoMatrix.Create2DOrthoMatrix(Engine.RenderSystem.GetViewportWidth(), Engine.RenderSystem.GetViewportHeight());

            dynVertexBuffer = new DynamicVertexBuffer(Globals.graphics3DDevice, idRenderGlobals.idDrawVertexDeclaration, 4, BufferUsage.WriteOnly);
            dynIndexBuffer = new DynamicIndexBuffer(Globals.graphics3DDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);

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
        // ResetGraphicsBuffers
        //
        private static void ResetGraphicsBuffers()
        {
            Globals.graphics3DDevice.Indices = null;
            Globals.graphics3DDevice.SetVertexBuffer(null);
        }

        //
        // DrawPrimitives
        //
        public static void DrawPrimitives(int numVerts, int numIndexes, idDrawVertex[] verts, short[] indexes)
        {
#if false
            ResetGraphicsBuffers();
            dynVertexBuffer.SetData<idDrawVertex>(verts, 0, numVerts);
            dynIndexBuffer.SetData<short>(indexes, 0, numIndexes);

            Globals.graphics3DDevice.SetVertexBuffer(dynVertexBuffer);
            Globals.graphics3DDevice.Indices = dynIndexBuffer;

            DrawElements(0, numVerts, 0, numIndexes, 0);

#else
            defaultEffect.DiffuseColor = pushedColor;
            defaultEffect.CurrentTechnique.Passes[0].Apply();
            Globals.graphics3DDevice.DrawUserIndexedPrimitives<idDrawVertex>(PrimitiveType.TriangleList, verts, 0, numVerts, indexes, 0, numIndexes / 3, idRenderGlobals.idDrawVertexDeclaration);
#endif
        }

        //
        // DrawTess
        //
        public static void DrawTess()
        {
#if false
            ResetGraphicsBuffers();
            dynVertexBuffer.SetData<idDrawVertex>(Globals.tess.drawVerts, 0, Globals.tess.numVertexes, SetDataOptions.Discard);
            dynIndexBuffer.SetData<short>(Globals.tess.indexes, 0, Globals.tess.numIndexes, SetDataOptions.Discard);

            Globals.graphics3DDevice.SetVertexBuffer(dynVertexBuffer);
            Globals.graphics3DDevice.Indices = dynIndexBuffer;

            DrawElements(0, Globals.tess.numVertexes, 0, Globals.tess.numIndexes, 0);
#else
            defaultEffect.DiffuseColor = pushedColor;
            defaultEffect.CurrentTechnique.Passes[0].Apply();
            Globals.graphics3DDevice.DrawUserIndexedPrimitives<idDrawVertex>(PrimitiveType.TriangleList, Globals.tess.drawVerts, 0, Globals.tess.numVertexes, Globals.tess.indexes, 0, Globals.tess.numIndexes / 3, idRenderGlobals.idDrawVertexDeclaration);
#endif
        }

        //
        // DrawElements
        //
        public static void DrawElements(int startVertex, int numVertexes, int startIndexes, int numIndexes, int offset)
        {
            defaultEffect.DiffuseColor = pushedColor;
            defaultEffect.CurrentTechnique.Passes[0].Apply();
            Globals.graphics3DDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, startVertex + offset, 0, numVertexes, startIndexes, numIndexes / 3);
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
        // SetCullMode
        //
        public static void SetCullMode( cullType_t cullType )
        {
	        if ( faceCulling == cullType ) {
		        return;
	        }

	        faceCulling = cullType;

            if (cullType == cullType_t.CT_TWO_SIDED)
            {
                Globals.graphics3DDevice.RasterizerState = RasterizerState.CullNone;
	        } 
            else
	        {
                if (cullType == cullType_t.CT_BACK_SIDED)
                {
                    Globals.graphics3DDevice.RasterizerState = RasterizerState.CullClockwise;
		        } 
                else
		        {
                    Globals.graphics3DDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		        }
	        }
        }

        //
        // SetMaterialStageState
        //
        public static void SetMaterialStageState(shaderStage_t stage)
        {
            if (stage.useBlending == true)
            {
                if (blending != stage.blendState)
                {
                    Globals.graphics3DDevice.BlendState = stage.blendState;
                    blending = stage.blendState;
                }
            }
            else
            {
                if (blending != BlendState.AlphaBlend)
                {
                    Globals.graphics3DDevice.BlendState = BlendState.AlphaBlend;
                    blending = BlendState.AlphaBlend;
                }
            }
        }
    }
}
