// Backend_device.cs (c) 2010 JV Software
//

#define BINDPARANOID

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using idLib.Math;
using idLib.Engine.Public;

using rtcw.Renderer.Effects;

namespace rtcw.Renderer.Backend
{
    //
    // Shade
    //
    static class Shade
    {
        private static BasicEffect default2DEffect;
        private static DualTextureEffect defaultEffect;
        private static idSkeletalEffect skeletalEffect;
        private static idRenderMatrix orthoMatrix;
        private static Vector3 pushedColor;
        private static float pushedAlpha;
        private static idRenderMatrix drawMatrix;

        private static BlendState blending = BlendState.AlphaBlend;
        private static cullType_t faceCulling = cullType_t.CT_FRONT_SIDED;

        // Dynamic vertex and index buffers for uploading vertex and index data.
        private static DynamicVertexBuffer dynVertexBuffer;
        private static DynamicIndexBuffer dynIndexBuffer;

        private static bool useSkeletalEffect = false;
        private static bool use2DEffect = false;

        //
        // Init
        //
        public static void Init()
        {
            defaultEffect = new DualTextureEffect(Globals.graphics3DDevice);
            default2DEffect = new BasicEffect(Globals.graphics3DDevice);
            skeletalEffect = new idSkeletalEffect();

            default2DEffect.TextureEnabled = true;

            //multiTextureEffect.Texture2 = (Texture2D)Globals.tr.whiteImage.GetDeviceHandle();

            orthoMatrix = new idRenderMatrix();
            orthoMatrix.Create2DOrthoMatrix(Engine.RenderSystem.GetViewportWidth(), Engine.RenderSystem.GetViewportHeight());

            dynVertexBuffer = new DynamicVertexBuffer(Globals.graphics3DDevice, idRenderGlobals.idDrawVertexDeclaration, 4, BufferUsage.None);
            dynIndexBuffer = new DynamicIndexBuffer(Globals.graphics3DDevice, IndexElementSize.SixteenBits, 6, BufferUsage.None);

            pushedColor = new Vector3(1, 1, 1);
            drawMatrix = new idRenderMatrix();
        }

        //
        // SetBoneMatrix
        //
        public static void SetBoneMatrix(Matrix[] bones)
        {
            useSkeletalEffect = true;
            use2DEffect = false;
            skeletalEffect.SetBoneMatrixes(bones);
        }

        //
        // Set2DOrthoMode
        //
        public static void Set2DOrthoMode()
        {
            use2DEffect = true;
            orthoMatrix.SetAsActiveMatrix(ref default2DEffect);
        }

        //
        // CreateTranslateRotateMatrix
        //
        public static void CreateTranslateRotateMatrix(idRenderEntityLocal refdef)
        {
            use2DEffect = false;
            drawMatrix.SetEntityMatrix(refdef);
            drawMatrix.SetAsActiveMatrix(ref defaultEffect);
            drawMatrix.SetAsActiveMatrix(ref skeletalEffect);
        }

        //
        // BindVertexIndexBuffer
        //
        public static void BindVertexIndexBuffer(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            use2DEffect = false;
            useSkeletalEffect = false;
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
            if (use2DEffect == true)
            {
                default2DEffect.DiffuseColor = pushedColor;
                default2DEffect.Alpha = pushedAlpha;
                default2DEffect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                defaultEffect.DiffuseColor = pushedColor;
                defaultEffect.Alpha = pushedAlpha;
                defaultEffect.CurrentTechnique.Passes[0].Apply();
            }
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
            if (use2DEffect == true)
            {
                default2DEffect.DiffuseColor = pushedColor;
                default2DEffect.Alpha = pushedAlpha;
                default2DEffect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                defaultEffect.DiffuseColor = pushedColor;
                defaultEffect.Alpha = pushedAlpha;
                defaultEffect.CurrentTechnique.Passes[0].Apply();
            }
            Globals.graphics3DDevice.DrawUserIndexedPrimitives<idDrawVertex>(PrimitiveType.TriangleList, Globals.tess.drawVerts, 0, Globals.tess.numVertexes, Globals.tess.indexes, 0, Globals.tess.numIndexes / 3, idRenderGlobals.idDrawVertexDeclaration);
#endif
        }

        //
        // DrawElements
        //
        public static void DrawSkinnedElements(int startVertex, int numVertexes, int startIndexes, int numIndexes, int offset)
        {
            skeletalEffect.Apply();
            Globals.graphics3DDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, startVertex + offset, 0, numVertexes, startIndexes, numIndexes / 3);
        }

        //
        // DrawElements
        //
        public static void DrawElements(int startVertex, int numVertexes, int startIndexes, int numIndexes, int offset)
        {
            if (useSkeletalEffect)
            {
                DrawSkinnedElements(startVertex, numVertexes, startIndexes, numIndexes, offset);
                return;
            }
            defaultEffect.Alpha = pushedAlpha;
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

            // If there isn't a world model don't set as the active matrix.
            if ((refdef.rdflags & idRenderType.RDF_NOWORLDMODEL) == 0)
            {
                drawMatrix.world = Matrix.Identity;
                drawMatrix.SetAsActiveMatrix(ref defaultEffect);
            }
        }

        //
        // BindImage
        //
        public static void BindImage(idImage image)
        {
            if (useSkeletalEffect)
            {
                skeletalEffect.Texture = (Texture2D)image.GetDeviceHandle();
                return;
            }
            else if (use2DEffect)
            {
                default2DEffect.Texture = (Texture2D)image.GetDeviceHandle();
                return;
            }
            defaultEffect.Texture = (Texture2D)image.GetDeviceHandle();
            defaultEffect.Texture2 = (Texture2D)Globals.tr.whiteImage.GetDeviceHandle();
        }

        //
        // BindMultiImage
        //
        public static void BindMultiImage(idImage image, idImage image2)
        {
#if BINDPARANOID
            if (image == null || image2 == null)
            {
                throw new Exception("Invalid bindmultiimage");
            }
#endif
            defaultEffect.Texture = (Texture2D)image.GetDeviceHandle();
            defaultEffect.Texture2 = (Texture2D)image2.GetDeviceHandle();

        }


        //
        // SetColor
        //
        public static void SetColor(float r, float g, float b, float a)
        {
            pushedColor.X = r;
            pushedColor.Y = g;
            pushedColor.Z = b;
            pushedAlpha = a;
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
