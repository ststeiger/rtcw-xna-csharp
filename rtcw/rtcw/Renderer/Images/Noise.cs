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

// Noise.cs (c) 2010 JV Software
//

using System;

namespace rtcw.Renderer.Images
{
    //
    // Noise
    //
    public static class Noise
    {
        const int NOISE_SIZE = 256;
        const int NOISE_MASK = ( NOISE_SIZE - 1 );
        const int RAND_MAX   = 0x7fff;

        private static int VAL( int a )
        {
            return s_noise_perm[ ( a ) & ( NOISE_MASK )];
        }
        private static int INDEX( int x, int y, int z, int t ) 
        {
            return VAL( x + VAL( y + VAL( z + VAL( t ) ) ) );
        }

        private static float[] s_noise_table = new float[NOISE_SIZE];
        private static int[] s_noise_perm = new int[NOISE_SIZE];

        public static float LERP( float a, float b,float w ) 
        {
            return ( a * ( 1.0f - w ) + b * w );
        }

        static float GetNoiseValue( int x, int y, int z, int t ) {
	        int index = INDEX( ( int ) x, ( int ) y, ( int ) z, ( int ) t );

	        return s_noise_table[index];
        }

        public static void Init() {
	        int i;
            Random random = new Random( 1001 );

	        for ( i = 0; i < NOISE_SIZE; i++ )
	        {
		        s_noise_table[i] = ( float ) ( ( ( random.Next() / ( float ) RAND_MAX ) * 2.0 - 1.0 ) );
		        s_noise_perm[i] = ( char )( random.Next() / ( float ) RAND_MAX * 255 );
	        }
        }

        static float[] front = new float[4];
        static float[] back = new float[4];
        static float[] value = new float[2];
        public static float NoiseGet4f( float x, float y, float z, float t ) {
	        int i;
	        int ix, iy, iz, it;
	        float fx, fy, fz, ft;

	        float fvalue, bvalue,finalvalue;

	        ix = ( int ) Math.Floor( x );
	        fx = x - ix;
	        iy = ( int ) Math.Floor( y );
	        fy = y - iy;
            iz = (int)Math.Floor(z);
	        fz = z - iz;
            it = (int)Math.Floor(t);
	        ft = t - it;

	        for ( i = 0; i < 2; i++ )
	        {
		        front[0] = GetNoiseValue( ix, iy, iz, it + i );
		        front[1] = GetNoiseValue( ix + 1, iy, iz, it + i );
		        front[2] = GetNoiseValue( ix, iy + 1, iz, it + i );
		        front[3] = GetNoiseValue( ix + 1, iy + 1, iz, it + i );

		        back[0] = GetNoiseValue( ix, iy, iz + 1, it + i );
		        back[1] = GetNoiseValue( ix + 1, iy, iz + 1, it + i );
		        back[2] = GetNoiseValue( ix, iy + 1, iz + 1, it + i );
		        back[3] = GetNoiseValue( ix + 1, iy + 1, iz + 1, it + i );

		        fvalue = LERP( LERP( front[0], front[1], fx ), LERP( front[2], front[3], fx ), fy );
		        bvalue = LERP( LERP( back[0], back[1], fx ), LERP( back[2], back[3], fx ), fy );

		        value[i] = LERP( fvalue, bvalue, fz );
	        }

	        finalvalue = LERP( value[0], value[1], ft );

	        return finalvalue;
        }

    }
}
