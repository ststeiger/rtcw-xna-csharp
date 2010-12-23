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

// SkeletalEffect.fx (c) 2010 JV Software
//

#define DECLARE_TEXTURE(Name, index) \
    Texture2D<float4> Name : register(t##index); \
    sampler Name##Sampler : register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name##Sampler, texCoord)

DECLARE_TEXTURE(Texture, 0);

#define MAX_BONES 75

// ---------------------------------------
float4x3 Bones[MAX_BONES];
float4x4 ModelViewProjectionMatrix;
// ---------------------------------------

//
// VertexShaderInput
//
struct VertexShaderInput
{
    float3 offset0 : POSITION0;
	float3 offset1 : POSITION1;
	float3 offset2 : POSITION2;
	float3 offset3 : POSITION3;

	float2 st : TEXCOORD0;
	float4 blendindexes : BLENDINDICES0;
	float4 weights : BLENDWEIGHT0;
};

//
// VertexShaderOutput
//
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 st : TEXCOORD0;
};

//
// LocalAddScaledMatrixTransformVectorTranslate
//
float4 LocalAddScaledMatrixTransformVectorTranslate( float3 weightoffset, float weight, float4x3 mat ) 
{
	float4 pos;
	pos[ 0 ] = weight * ( weightoffset[ 0 ] * mat[ 0 ][ 0 ] + weightoffset[ 1 ] * mat[ 0 ][ 1 ] + weightoffset[ 2 ] * mat[ 0 ][ 2 ] + mat[ 3 ][ 0 ] );
	pos[ 1 ] = weight * ( weightoffset[ 0 ] * mat[ 1 ][ 0 ] + weightoffset[ 1 ] * mat[ 1 ][ 1 ] + weightoffset[ 2 ] * mat[ 1 ][ 2 ] + mat[ 3 ][ 1 ] );
	pos[ 2 ] = weight * ( weightoffset[ 0 ] * mat[ 2 ][ 0 ] + weightoffset[ 1 ] * mat[ 2 ][ 1 ] + weightoffset[ 2 ] * mat[ 2 ][ 2 ] + mat[ 3 ][ 2 ] );
	pos[ 3 ] = 0;
	return pos;
}

//
// VertexShaderFunction
//
VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = LocalAddScaledMatrixTransformVectorTranslate(input.offset0, input.weights[0], Bones[input.blendindexes[0]]);
	output.Position += LocalAddScaledMatrixTransformVectorTranslate(input.offset1, input.weights[1], Bones[input.blendindexes[1]]);
	output.Position += LocalAddScaledMatrixTransformVectorTranslate(input.offset2, input.weights[2], Bones[input.blendindexes[2]]);
	output.Position += LocalAddScaledMatrixTransformVectorTranslate(input.offset3, input.weights[3], Bones[input.blendindexes[3]]);
	output.Position[3] = 1.0f;

	output.Position = mul(output.Position, ModelViewProjectionMatrix);
	output.st = input.st;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = SAMPLE_TEXTURE(Texture, input.st);

	return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
