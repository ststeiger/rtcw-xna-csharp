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

// Video.cs (c) 2010 JV Software
//

//#define USE_ROQ_MOVIES // This works but doesn't work on the phone due to the file size.

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using idLib.Engine.Public;
using Microsoft.Xna.Framework.Media;
using rtcw.Framework;

namespace rtcw.Renderer
{
    //
    // idVideoVQTable
    //
#if USE_ROQ_MOVIES
    unsafe class idVideoVQTable
    {
        ushort[] membuffer;
        GCHandle handle;

        //
        // idVideoVQTable
        //
        public idVideoVQTable( int memsize )
        {
            membuffer = new ushort[memsize];
            handle = GCHandle.Alloc(membuffer, GCHandleType.Pinned);
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            handle.Free();
            membuffer = null;
        }

        //
        // Indexed 
        //
        public ushort this[int index]
        {
            get
            {
                return membuffer[index];
            }
            set
            {
                membuffer[index] = value;
            }
        }

        //
        // Ptr
        //
        public ushort* Ptr
        {
            get
            {
                return (ushort *)handle.AddrOfPinnedObject().ToPointer();
            }
        }

        //
        // GetPointerFromLocation
        //
        public ushort* GetPointerFromLocation(int index)
        {
            fixed (ushort* buf = &membuffer[index])
            {
                return buf;
            }
        }
    }

    //
    // idVideoLocal
    //
    public unsafe class idVideoLocal : idVideo
    {
        //
        // idVideoLocal
        //
        public idVideoLocal( idFile f )
        {
            cinCache.iFile = f;
            cinCache.status = e_status.FMV_IDLE;
        }

        public override void Dispose()
        {
            Engine.fileSystem.CloseFile(ref cinCache.iFile);

            if (cinCache.sound != null)
            {
                cinCache.sound.Dipose();
                cinCache.sound = null;
            }

            Engine.imageManager.DestroyImage(ref blitImage);

            vq2.Dispose();
            vq4.Dispose();
            vq8.Dispose();
            cin.Dispose();
            cinCache.Dispose();
            cinCache = null;
            cin = null;

            Engine.common.ForceGCCollect();
        }

        public override e_status GetStatus()
        {
            return cinCache.status;
        }

        const int ROQ_BASE_POSITION = 0;

        const int MAXSIZE		=		8;
        const int MINSIZE		=		4;

        const int DEFAULT_CIN_WIDTH	 =  512;
        const int DEFAULT_CIN_HEIGHT =	512;

        const short ROQ_QUAD		 =	0x1000;
        const short ROQ_QUAD_INFO	 =	0x1001;
        const short ROQ_CODEBOOK	 =	0x1002;
        const short ROQ_QUAD_VQ		 =	0x1011;
        const short ROQ_QUAD_JPEG	 =	0x1012;
        const short ROQ_QUAD_HANG	 =	0x1013;
        const short ROQ_PACKET		 =	0x1030;
        const short ZA_SOUND_MONO	 =	0x1020;
        const short ZA_SOUND_STEREO	 =	0x1021;

        const int MAX_VIDEO_HANDLES	 = 16;

        /******************************************************************************
        *
        * Class:		trFMV
        *
        * Description:	RoQ/RnR manipulation routines
        *				not entirely complete for first run
        *
        ******************************************************************************/

        private	long[]				ROQ_YY_tab = new long[256];
        private	long[]				ROQ_UB_tab = new long[256];
        private	long[]				ROQ_UG_tab = new long[256];
        private	long[]				ROQ_VG_tab = new long[256];
        private	long[]				ROQ_VR_tab = new long[256];
        private idVideoVQTable      vq2 = new idVideoVQTable(256 * 16 * 4);
        private idVideoVQTable      vq4 = new idVideoVQTable(256 * 64 * 4);
        private idVideoVQTable      vq8 = new idVideoVQTable(256 * 256 * 4);


        private class cinematics_t {
            public void Dispose()
            {
                linbufmem = null;
                blitBuffer = null;
                sbufmemory = null;
                file = null;
                sqrTable = null;
                mcomp = null;
                qStatus = null;
                
            }

	        public byte[]				linbufmem = new byte[DEFAULT_CIN_WIDTH*DEFAULT_CIN_HEIGHT*4*2];
            public byte *linbuf
            {
                get
                {
                    fixed( byte *mem = &linbufmem[0])
                    {
                        return mem;
                    }
                }
            }

            public byte[] blitBuffer = new byte[32768 * 2];

            public short[] sbufmemory = new short[32768];
            public short* sbuf
            {
                get
                {
                    fixed (short* mem = &sbufmemory[0])
                    {
                        return mem;
                    }
                }
            }

	        public byte[]				file;
	        public short[]				sqrTable;

	        public uint[]		        mcomp = new uint[256];
	        public byte*[,]			   qStatus = new byte*[2,32768];

	        public long				oldXOff, oldYOff, oldysize, oldxsize;
        } ;

        public class cin_cache{
	        public string		fileName;
	        public int			CIN_WIDTH, CIN_HEIGHT;
	        public int			xpos, ypos, width, height;
	        public bool			looping, holdAtEnd, dirty, alterGameState, silent, shader;
	        public idFile		iFile;
	        public e_status		status;
	        public uint		    startTime;
	        public uint		    lastTime;
	        public long			tfps;
	        public long			RoQPlayed;
	        public long			ROQSize;
	        public uint 		RoQFrameSize;
	        public long			onQuad;
	        public long			numQuads;
	        public long			samplesPerLine;
	        public short	    roq_id;
	        public long			screenDelta;
            public int          numSoundFrames = 0;
            public idSound      sound;

            public delegate void PixelProcessFunc_t(int status, byte *data);
            public PixelProcessFunc_t VQ0;
            public PixelProcessFunc_t VQ1;
            public PixelProcessFunc_t VQNormal;
            public PixelProcessFunc_t VQBuffer;

	        public long			samplesPerPixel;				// defaults to 2
	        public byte*		gray;
	        public uint		    xsize, ysize, maxsize, minsize;

	        public bool			half, smootheddouble, inMemory;
	        public long			normalBuffer0;
	        public long			roq_flags;
	        public long			roqF0;
	        public long			roqF1;
	        public long[]		t = new long[2];
	        public long			roqFPS;
	        public int			playonwalls;
            public void Dispose()
            {
                bufmemory = null;
            }

	        public Color[]		bufmemory;
            public Color* buf
            {
                get
                {
                    fixed (Color* firstPixel = &bufmemory[0])
                    {
                        return firstPixel;
                    }
                }
                set
                {
                    Color* temp = (Color*)value;
                    for (int i = 0; i < bufmemory.Length; i++)
                    {
                        bufmemory[i] = temp[i];
                        if (bufmemory[i].A == 0)
                        {
                            Engine.common.ErrorFatal("Buf Memory Alpha invalid...\n");
                        }
                    }
                }
            }
	        public long			drawX, drawY;
            public bool isInit = false;
        };


        private cinematics_t	cin = new cinematics_t();
        private cin_cache		cinCache = new cin_cache();

        static class VQ2
        {
            public static  void TO4(ref uint *a, ref uint *b, ref uint *c, ref uint *d)
            {
                *c++ = a[0];	
	            *d++ = a[0];	
	            *d++ = a[0];	
	            *c++ = a[1];	
	            *d++ = a[1];	
	            *d++ = a[1];	
	            *c++ = b[0];	
	            *d++ = b[0];	
	            *d++ = b[0];	
	            *c++ = b[1];	
	            *d++ = b[1];	
	            *d++ = b[1];	
	            *d++ = a[0];	
	            *d++ = a[0];	
	            *d++ = a[1];	
	            *d++ = a[1];	
	            *d++ = b[0];	
	            *d++ = b[0];	
	            *d++ = b[1];	
	            *d++ = b[1];	
	            a += 2; b += 2;
            }
        }

#region soundlookup
        //-----------------------------------------------------------------------------
        // RllSetupTable
        //
        // Allocates and initializes the square table.
        //
        // Parameters:	None
        //
        // Returns:		Nothing
        //-----------------------------------------------------------------------------
        private void RllSetupTable()
        {
	        int z;

            cin.sqrTable = new short[256];

	        for (z=0;z<128;z++) {
		        cin.sqrTable[z] = (short)(z*z);
		        cin.sqrTable[z+128] = (short)(-cin.sqrTable[z]);
	        }
        }



        //-----------------------------------------------------------------------------
        // RllDecodeMonoToMono
        //
        // Decode mono source data into a mono buffer.
        //
        // Parameters:	from -> buffer holding encoded data
        //				to ->	buffer to hold decoded data
        //				size =	number of bytes of input (= # of shorts of output)
        //				signedOutput = 0 for unsigned output, non-zero for signed output
        //				flag = flags from asset header
        //
        // Returns:		Number of samples placed in output buffer
        //-----------------------------------------------------------------------------
        private long RllDecodeMonoToMono(char *from, short *to, uint size,char signedOutput ,ushort flag)
        {
	        uint z;
	        int prev;
	
	        if (signedOutput != 0)	
		        prev =  flag - 0x8000;
	        else 
		        prev = flag;

	        for (z=0;z<size;z++) {
		        prev = to[z] = (short)(prev + cin.sqrTable[from[z]]); 
	        }
	        return size;	//*sizeof(short));
        }


        //-----------------------------------------------------------------------------
        // RllDecodeMonoToStereo
        //
        // Decode mono source data into a stereo buffer. Output is 4 times the number
        // of bytes in the input.
        //
        // Parameters:	from -> buffer holding encoded data
        //				to ->	buffer to hold decoded data
        //				size =	number of bytes of input (= 1/4 # of bytes of output)
        //				signedOutput = 0 for unsigned output, non-zero for signed output
        //				flag = flags from asset header
        //
        // Returns:		Number of samples placed in output buffer
        //-----------------------------------------------------------------------------
        private long RllDecodeMonoToStereo(char *from, short *to,uint size,char signedOutput,ushort flag)
        {
	        uint z;
	        int prev;
	
	        if (signedOutput != 0)	
		        prev =  flag - 0x8000;
	        else 
		        prev = flag;

	        for (z = 0; z < size; z++) {
		        prev = (short)(prev + cin.sqrTable[from[z]]);
		        to[z*2+0] = to[z*2+1] = (short)(prev);
	        }
	
	        return size;	// * 2 * sizeof(short));
        }


        //-----------------------------------------------------------------------------
        // RllDecodeStereoToStereo
        //
        // Decode stereo source data into a stereo buffer.
        //
        // Parameters:	from -> buffer holding encoded data
        //				to ->	buffer to hold decoded data
        //				size =	number of bytes of input (= 1/2 # of bytes of output)
        //				signedOutput = 0 for unsigned output, non-zero for signed output
        //				flag = flags from asset header
        //
        // Returns:		Number of samples placed in output buffer
        //-----------------------------------------------------------------------------
        private long RllDecodeStereoToStereo(byte *from,short *to,uint size,bool signedOutput, ushort flag)
        {
	        uint z;
            byte* zz = from;
	        int	prevL, prevR;

	        if (signedOutput) {
		        prevL = (flag & 0xff00) - 0x8000;
		        prevR = ((flag & 0x00ff) << 8) - 0x8000;
	        } else {
		        prevL = flag & 0xff00;
		        prevR = (flag & 0x00ff) << 8;
	        }

	        for (z=0;z<size;z+=2) {
                        prevL = (short)(prevL + cin.sqrTable[*zz++]); 
                        prevR = (short)(prevR + cin.sqrTable[*zz++]);
                        to[z+0] = (short)(prevL);
                        to[z+1] = (short)(prevR);
	        }
	
	        return (size>>1);	//*sizeof(short));
        }


        //-----------------------------------------------------------------------------
        // RllDecodeStereoToMono
        //
        // Decode stereo source data into a mono buffer.
        //
        // Parameters:	from -> buffer holding encoded data
        //				to ->	buffer to hold decoded data
        //				size =	number of bytes of input (= # of bytes of output)
        //				signedOutput = 0 for unsigned output, non-zero for signed output
        //				flag = flags from asset header
        //
        // Returns:		Number of samples placed in output buffer
        //-----------------------------------------------------------------------------
        private long RllDecodeStereoToMono(char *from,short *to,uint size,char signedOutput, ushort flag)
        {
	        uint z;
	        int prevL,prevR;
	
	        if (signedOutput != 0) {
		        prevL = (flag & 0xff00) - 0x8000;
		        prevR = ((flag & 0x00ff) << 8) -0x8000;
	        } else {
		        prevL = flag & 0xff00;
		        prevR = (flag & 0x00ff) << 8;
	        }

	        for (z=0;z<size;z+=1) {
		        prevL= prevL + cin.sqrTable[from[z*2]];
		        prevR = prevR + cin.sqrTable[from[z*2+1]];
		        to[z] = (short)((prevL + prevR)/2);
	        }

	        return size;
        }
#endregion
        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void move8_32( byte *src, byte *dst, int spl )
        {
	        double *dsrc;
            double *ddst;
	        int dspl;

	        dsrc = (double *)src;
	        ddst = (double *)dst;
	        dspl = spl>>3;

	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void move4_32( byte *src, byte *dst, int spl  )
        {
	        double *dsrc;
            double *ddst;
	        int dspl;

	        dsrc = (double *)src;
	        ddst = (double *)dst;
	        dspl = spl>>3;

	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
	        dsrc += dspl; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void blit8_32( byte *src, byte *dst, int spl  )
        {
	        double *dsrc;
            double *ddst;
	        int dspl;

	        dsrc = (double *)src;
	        ddst = (double *)dst;
	        dspl = spl>>3;

	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += 4; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += 4; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += 4; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += 4; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += 4; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += 4; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
	        dsrc += 4; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1]; ddst[2] = dsrc[2]; ddst[3] = dsrc[3];
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/
        private void blit4_32( byte *src, byte *dst, int spl  )
        {
	        double *dsrc;
            double *ddst;
	        int dspl;

	        dsrc = (double *)src;
	        ddst = (double *)dst;
	        dspl = spl>>3;

	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
	        dsrc += 2; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
	        dsrc += 2; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
	        dsrc += 2; ddst += dspl;
	        ddst[0] = dsrc[0]; ddst[1] = dsrc[1];
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void blit2_32( byte *src, byte *dst, int spl  )
        {
	        double *dsrc;
            double *ddst;
	        int dspl;

	        dsrc = (double *)src;
	        ddst = (double *)dst;
	        dspl = spl>>3;

	        ddst[0] = dsrc[0];
	        ddst[dspl] = dsrc[1];
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void blitVQQuad32fs(int status, byte* data)
        {
        ushort	newd, celdata, code;
        uint	index, i;
        int		spl;

	        newd	= 0;
	        celdata = 0;
	        index	= 0;
	
                spl = (int)cinCache.samplesPerLine;
        
	        do {
		        if (newd == 0) { 
			        newd = 7;
			        celdata = (ushort)(data[0] + data[1]*256);
			        data += 2;
		        } else {
			        newd--;
		        }

		        code = (ushort)(celdata&0xc000); 
		        celdata <<= 2;
		
		        switch (code) {
			        case	0x8000:													// vq code
                        blit8_32((byte *)vq8.GetPointerFromLocation((*data)*128), cin.qStatus[status, index], spl);
				        data++;
				        index += 5;
				        break;
			        case	0xc000:													// drop
				        index++;													// skip 8x8
				        for(i=0;i<4;i++) {
					        if (newd == 0) { 
						        newd = 7;
						        celdata = (ushort)(data[0] + data[1]*256);
						        data += 2;
					        } else {
						        newd--;
					        }
						
					        code = (ushort)(celdata&0xc000); celdata <<= 2; 

					        switch (code) {											// code in top two bits of code
						        case	0x8000:										// 4x4 vq code
                                    blit4_32((byte *)vq4.GetPointerFromLocation((*data)*32), cin.qStatus[status, index], spl);
							        data++;
							        break;
						        case	0xc000:										// 2x2 vq code
                                    blit2_32((byte*)vq2.GetPointerFromLocation((*data) * 8), cin.qStatus[status, index], spl);
							        data++;
                                    blit2_32((byte*)vq2.GetPointerFromLocation((*data) * 8), cin.qStatus[status, index] + 8, spl);
                                    data++;
                                    blit2_32((byte*)vq2.GetPointerFromLocation((*data) * 8), cin.qStatus[status, index] + spl * 2, spl);
                                    data++;
                                    blit2_32((byte*)vq2.GetPointerFromLocation((*data) * 8), cin.qStatus[status, index] + spl * 2 + 8, spl);
                                    data++;
                                    
							        break;
						        case	0x4000:										// motion compensation
                                    move4_32(cin.qStatus[status, index] + cin.mcomp[(*data)], cin.qStatus[status, index], spl);
							        data++;
							        break;
					        }
					        index++;
				        }
				        break;
			        case	0x4000:													// motion compensation
                        move8_32(cin.qStatus[status, index] + cin.mcomp[(*data)], cin.qStatus[status, index], spl);
				        data++;
				        index += 5;
				        break;
			        case	0x0000:
				        index += 5;
				        break;
		        }
            } while (cin.qStatus[status, index] != null);
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void ROQ_GenYUVTables()
        {
	        float t_ub,t_vr,t_ug,t_vg;
	        int i;

	        t_ub = (1.77200f/2.0f) * (float)(1<<6) + 0.5f;
	        t_vr = (1.40200f/2.0f) * (float)(1<<6) + 0.5f;
	        t_ug = (0.34414f/2.0f) * (float)(1<<6) + 0.5f;
	        t_vg = (0.71414f/2.0f) * (float)(1<<6) + 0.5f;
	        for(i=0;i<256;i++) {
		        float x = (float)(2 * i - 255);
	
		        ROQ_UB_tab[i] = (long)( ( t_ub * x) + (1<<5));
		        ROQ_VR_tab[i] = (long)( ( t_vr * x) + (1<<5));
		        ROQ_UG_tab[i] = (long)( (-t_ug * x)		 );
		        ROQ_VG_tab[i] = (long)( (-t_vg * x) + (1<<5));
		        ROQ_YY_tab[i] = (long)( (i << 6) | (i >> 2) );
	        }
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/
        private uint yuv_to_rgb24( long y, long u, long v )
        { 
	        long r,g,b,YY = (long)(ROQ_YY_tab[(y)]);
            

	        r = (YY + ROQ_VR_tab[v]) >> 6;
	        g = (YY + ROQ_UG_tab[u] + ROQ_VG_tab[v]) >> 6;
	        b = (YY + ROQ_UB_tab[u]) >> 6;
	
	        if (r<0) r = 0; if (g<0) g = 0; if (b<0) b = 0;
	        if (r > 255) r = 255; if (g > 255) g = 255; if (b > 255) b = 255;

	        return (uint)((r)|(g<<8)|(b<<16)|(255<<24));
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void decodeCodeBook( byte *input, ushort roq_flags )
        {
	        long	i, j, two, four;
	        long	y0,y1,y2,y3,cr,cb;
	        uint *iaptr;
            uint *ibptr;
            uint *icptr;
            uint *idptr;

	        if (roq_flags == 0) {
		        two = four = 256;
	        } else {
		        two  = roq_flags>>8;
		        if (two == 0) two = 256;
		        four = roq_flags&0xff;
	        }

	        four *= 2;

            ibptr = (uint*)vq2.Ptr;
            for (i = 0; i < two; i++)
            {
                y0 = (long)*input++;
                y1 = (long)*input++;
                y2 = (long)*input++;
                y3 = (long)*input++;
                cr = (long)*input++;
                cb = (long)*input++;
                *ibptr++ = yuv_to_rgb24(y0, cr, cb);
                *ibptr++ = yuv_to_rgb24(y1, cr, cb);
                *ibptr++ = yuv_to_rgb24(y2, cr, cb);
                *ibptr++ = yuv_to_rgb24(y3, cr, cb);
            }

            icptr = (uint*)vq4.Ptr;
            idptr = (uint*)vq8.Ptr;

            for (i = 0; i < four; i++)
            {
                iaptr = (uint*)vq2.Ptr + (*input++) * 4;
                ibptr = (uint*)vq2.Ptr + (*input++) * 4;
                for (j = 0; j < 2; j++)
                    VQ2.TO4(ref iaptr, ref ibptr, ref icptr, ref idptr);
            }
				      
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void recurseQuad( long startX, long startY, long quadSize, long xOff, long yOff )
        {
	        byte *scroff;
	        long bigx, bigy, lowx, lowy, useY;
	        long offset;

	        offset = cinCache.screenDelta;
	
	        lowx = lowy = 0;
	        bigx = cinCache.xsize;
	        bigy = cinCache.ysize;

	        if (bigx > cinCache.CIN_WIDTH) bigx = cinCache.CIN_WIDTH;
	        if (bigy > cinCache.CIN_HEIGHT) bigy = cinCache.CIN_HEIGHT;

	        if ( (startX >= lowx) && (startX+quadSize) <= (bigx) && (startY+quadSize) <= (bigy) && (startY >= lowy) && quadSize <= MAXSIZE) {
		        useY = startY;
		        scroff = cin.linbuf + (useY+((cinCache.CIN_HEIGHT-bigy)>>1)+yOff)*(cinCache.samplesPerLine) + (((startX+xOff))*cinCache.samplesPerPixel);

		        cin.qStatus[0,cinCache.onQuad  ] = scroff;
		        cin.qStatus[1,cinCache.onQuad++] = scroff+offset;
	        }

	        if ( quadSize != MINSIZE ) {
		        quadSize >>= 1;
		        recurseQuad( startX,		  startY		  , quadSize, xOff, yOff );
		        recurseQuad( startX+quadSize, startY		  , quadSize, xOff, yOff );
		        recurseQuad( startX,		  startY+quadSize , quadSize, xOff, yOff );
		        recurseQuad( startX+quadSize, startY+quadSize , quadSize, xOff, yOff );
	        }
        }


        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

       private void setupQuad( long xOff, long yOff )
        {
	        long numQuadCels, i,x,y;
	        byte *temp;

	        if (xOff == cin.oldXOff && yOff == cin.oldYOff && cinCache.ysize == cin.oldysize && cinCache.xsize == cin.oldxsize) {
		        return;
	        }

	        cin.oldXOff = xOff;
	        cin.oldYOff = yOff;
	        cin.oldysize = cinCache.ysize;
	        cin.oldxsize = cinCache.xsize;

	        numQuadCels  = (cinCache.CIN_WIDTH*cinCache.CIN_HEIGHT) / (16);
	        numQuadCels += numQuadCels/4 + numQuadCels/16;
	        numQuadCels += 64;							  // for overflow

	        numQuadCels  = (cinCache.xsize*cinCache.ysize) / (16);
	        numQuadCels += numQuadCels/4;
	        numQuadCels += 64;							  // for overflow

	        cinCache.onQuad = 0;

	        for(y=0;y<(long)cinCache.ysize;y+=16) 
		        for(x=0;x<(long)cinCache.xsize;x+=16) 
			        recurseQuad( x, y, 16, xOff, yOff );

	        temp = null;

	        for(i=(numQuadCels-64);i<numQuadCels;i++) {
		        cin.qStatus[0,i] = temp;			  // eoq
		        cin.qStatus[1,i] = temp;			  // eoq
	        }
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void readQuadInfo( byte *qData )
        {
	        cinCache.xsize    = (uint)(qData[0]+qData[1]*256);
	        cinCache.ysize    = (uint)(qData[2]+qData[3]*256);
	        cinCache.maxsize  = (uint)(qData[4]+qData[5]*256);
	        cinCache.minsize  = (uint)(qData[6]+qData[7]*256);
	
	        cinCache.CIN_HEIGHT = (int)cinCache.ysize;
	        cinCache.CIN_WIDTH  = (int)cinCache.xsize;

	        cinCache.samplesPerLine = cinCache.CIN_WIDTH*cinCache.samplesPerPixel;
	        cinCache.screenDelta = cinCache.CIN_HEIGHT*cinCache.samplesPerLine;

	        cinCache.half = false;
	        cinCache.smootheddouble = false;
	
	        cinCache.VQ0 = cinCache.VQNormal;
	        cinCache.VQ1 = cinCache.VQBuffer;

	        cinCache.t[0] = (0 - (uint)cin.linbuf)+(uint)cin.linbuf+cinCache.screenDelta;
	        cinCache.t[1] = (0 - ((uint)cin.linbuf + cinCache.screenDelta))+(uint)cin.linbuf;


            cinCache.drawX = cinCache.CIN_WIDTH;
            cinCache.drawY = cinCache.CIN_HEIGHT;
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void RoQPrepMcomp( long xoff, long yoff ) 
        {
	        long i, j, x, y, temp, temp2;

	        i=(uint)cinCache.samplesPerLine; j=cinCache.samplesPerPixel;
	        if ( cinCache.xsize == (cinCache.ysize*4) && !cinCache.half ) { j = j+j; i = i+i; }
	
	        for(y=0;y<16;y++) {
		        temp2 = (y+yoff-8)*i;
		        for(x=0;x<16;x++) {
			        temp = (x+xoff-8)*j;
			        cin.mcomp[(x*16)+y] = (uint)(cinCache.normalBuffer0-(temp2+temp));
		        }
	        }
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void initRoQ() 
        {
	        cinCache.VQNormal = blitVQQuad32fs;
	        cinCache.VQBuffer = blitVQQuad32fs;
	        cinCache.samplesPerPixel = 4;
	        ROQ_GenYUVTables();
	        RllSetupTable();
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/
        /*
        static byte* RoQFetchInterlaced( byte *source ) {
	        int x, *src, *dst;

	        if (currentHandle < 0) return NULL;

	        src = (int *)source;
	        dst = (int *)cinTable[currentHandle].buf2;

	        for(x=0;x<256*256;x++) {
		        *dst = *src;
		        dst++; src += 2;
	        }
	        return cinTable[currentHandle].buf2;
        }
        */
        private void RoQReset() {
            cinCache.iFile.Seek(idFileSeekOrigin.FS_SEEK_SET, ROQ_BASE_POSITION);
	        
	        cin.file = cinCache.iFile.ReadBytes( 16 );
	        RoQ_init();
	        cinCache.status = e_status.FMV_LOOPED;
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/
        private bool skipSoundFrame = false;
        private void RoQInterrupt()
        {
            int		ssize;
            bool redump = false;

            cin.file = cinCache.iFile.ReadBytes( (int)(cinCache.RoQFrameSize+8) );
            if (cin.file == null)
            {
                cinCache.status = e_status.FMV_EOF;
                return;
            }

	        fixed(byte *framedatafixed = &cin.file[0])
            {
                byte *framedata = framedatafixed;

                //
                // new frame is ready
                //
                
	                switch(cinCache.roq_id) 
	                {
		                case	ROQ_QUAD_VQ:
			                if ((cinCache.numQuads&1) != 0) {
				                cinCache.normalBuffer0 = cinCache.t[1];
				                RoQPrepMcomp( cinCache.roqF0, cinCache.roqF1 );
                                cinCache.VQ1(1, framedata);
                                byte *buffer = cin.linbuf + cinCache.screenDelta;
                                cinCache.buf = (Color*)(buffer);
			                } else {
				                cinCache.normalBuffer0 = cinCache.t[0];
				                RoQPrepMcomp( cinCache.roqF0, cinCache.roqF1 );
                                cinCache.VQ0(0, framedata);
				                cinCache.buf = 	(Color *)(cin.linbuf);
			                }
			                if (cinCache.numQuads == 0) {		// first frame
                                Array.Copy(cin.linbufmem, 0, cin.linbufmem, (int)cinCache.screenDelta, (int)(cinCache.samplesPerLine * cinCache.ysize));
				                //Com_Memcpy(cin.linbuf+cinTable[currentHandle].screenDelta, cin.linbuf, cinTable[currentHandle].samplesPerLine*cinTable[currentHandle].ysize);
			                }
			                cinCache.numQuads++;
			                cinCache.dirty = true;
			                break;
		                case	ROQ_CODEBOOK:
			                decodeCodeBook( framedata, (ushort)cinCache.roq_flags );
			                break;
		                case	ZA_SOUND_MONO:
                            if (cinCache.sound == null)
                            {
                                cinCache.sound = Engine.soundManager.CreateStreamingSound(22050, 1);
                            }
                            break;
		                case	ZA_SOUND_STEREO:
                            if (cinCache.sound == null)
                            {
                                cinCache.sound = Engine.soundManager.CreateStreamingSound(22050, 2);
                                cinCache.sound.Play();
                            }

                            ssize = (int)RllDecodeStereoToStereo(framedata, cin.sbuf, cinCache.RoQFrameSize, false, (ushort)cinCache.roq_flags);

                            for (int i = 0, a = 0; i < ssize; i++, a += 4)
                            {
                                byte[] temp = System.BitConverter.GetBytes(cin.sbufmemory[i * 2]);
                                cin.blitBuffer[a + 0] = temp[0];
                                cin.blitBuffer[a + 1] = temp[1];

                                temp = System.BitConverter.GetBytes(cin.sbufmemory[i * 2 + 1]);
                                cin.blitBuffer[a + 2] = temp[0];
                                cin.blitBuffer[a + 3] = temp[1];
                            }
                            cinCache.sound.BlitSoundData(cin.blitBuffer, 0, ssize * 4);
                            
                            break;
		                case	ROQ_QUAD_INFO:
			                if (cinCache.numQuads == -1) {
				                readQuadInfo( framedata );
				                setupQuad( 0, 0 );
				                // we need to use CL_ScaledMilliseconds because of the smp mode calls from the renderer
                                cinCache.startTime = cinCache.lastTime = (uint)ScaledMilliseconds();
			                }
			                if (cinCache.numQuads != 1) cinCache.numQuads = 0;
			                break;
		                case	ROQ_PACKET:
			                cinCache.inMemory = cinCache.roq_flags != 0;
			                cinCache.RoQFrameSize = 0;           // for header
			                break;
		                case	ROQ_QUAD_HANG:
			                cinCache.RoQFrameSize = 0;
			                break;
		                case	ROQ_QUAD_JPEG:
			                break;
		                default:
			                cinCache.status = e_status.FMV_EOF;
			                break;
	                }	
                //
                // read in next frame data
                //
	                if ( cinCache.RoQPlayed >= cinCache.ROQSize ) { 
		                if (cinCache.holdAtEnd==false) {
			                if (cinCache.looping) {
				                RoQReset();
			                } else {
				                cinCache.status = e_status.FMV_EOF;
			                }
		                } else {
			                cinCache.status = e_status.FMV_IDLE;
		                }
		                return; 
	                }
	
	                framedata		 += cinCache.RoQFrameSize;
	                cinCache.roq_id		  = (short)(framedata[0] + framedata[1]*256);
	                cinCache.RoQFrameSize = (uint)(framedata[2] + framedata[3]*256 + framedata[4]*65536);
	                cinCache.roq_flags	  = framedata[6] + framedata[7]*256;
	                cinCache.roqF0		  = (sbyte)framedata[7];
                    cinCache.roqF1        = (sbyte)framedata[6];

	                if (cinCache.RoQFrameSize>65536||cinCache.roq_id==0x1084) {
		                Engine.common.DPrintf("roq_size>65536||roq_id==0x1084\n");
		                cinCache.status = e_status.FMV_EOF;
		                if (cinCache.looping) {
			                RoQReset();
		                }
		                return;
	                }
	                //if (cinCache.inMemory && (cinCache.status != e_status.FMV_EOF)) { cinCache.inMemory = false; framedata += 8; goto redump; }
                    
                //
                // one more frame hits the dust
                //
                //	assert(cinTable[currentHandle].RoQFrameSize <= 65536);
                //	r = Sys_StreamedRead( cin.file, cinTable[currentHandle].RoQFrameSize+8, 1, cinTable[currentHandle].iFile );
	                cinCache.RoQPlayed	+= cinCache.RoQFrameSize+8;

            }
        }

        /******************************************************************************
        *
        * Function:		
        *
        * Description:	
        *
        ******************************************************************************/

        private void RoQ_init()
        {
	        // we need to use CL_ScaledMilliseconds because of the smp mode calls from the renderer
            cinCache.startTime = cinCache.lastTime = (uint)ScaledMilliseconds();

	        cinCache.RoQPlayed = 24;

        /*	get frame rate */	
	        cinCache.roqFPS	 = cin.file[ 6] + cin.file[ 7]*256;
	
	        if (cinCache.roqFPS == 0) cinCache.roqFPS = 30;

	        cinCache.numQuads = -1;

	        cinCache.roq_id		    = (short)(cin.file[ 8] + cin.file[ 9]*256);
	        cinCache.RoQFrameSize	= (uint)(cin.file[10] + cin.file[11]*256 + cin.file[12]*65536);
	        cinCache.roq_flags	    = cin.file[14] + cin.file[15]*256;

	        if (cinCache.RoQFrameSize > 65536 || cinCache.RoQFrameSize == 0) { 
		        return;
	        }

        }

        /*
        ==================
        SCR_StopCinematic
        ==================
        */
        public override void StopCinematic() {
	        if (cinCache.status == e_status.FMV_EOF) 
            {
                return;
            }
	        
	        Engine.common.DPrintf("trFMV::stop(), closing %s\n", cinCache.fileName);

	        if (cinCache.buf == null) {
		        return;
	        }

	        cinCache.status = e_status.FMV_EOF;
	        //RoQShutdown();
        }

        //
        // ScaledMilliseconds
        //
        private int ScaledMilliseconds()
        {
            return (int)(Engine.common.ScaledMilliseconds() * idCommonLocal.com_timescale.GetValueFloat());
        }

        /*
        ==================
        SCR_RunCinematic

        Fetch and decompress the pending frame
        ==================
        */
        public void  RunCinematic()
        {
                // bk001204 - init
	        int	start = 0;
	        int     thisTime = 0;

            if (cinCache.status == e_status.FMV_EOF)
                return;

	        // we need to use CL_ScaledMilliseconds because of the smp mode calls from the renderer
            thisTime = ScaledMilliseconds();
	      //  if (cinTable[currentHandle].shader && (abs(thisTime - cinTable[currentHandle].lastTime))>100) {
		   //     cinTable[currentHandle].startTime += thisTime - cinTable[currentHandle].lastTime;
	      //  }
	        // we need to use CL_ScaledMilliseconds because of the smp mode calls from the renderer
	        cinCache.tfps = ((((ScaledMilliseconds()) - cinCache.startTime)*3)/100);

	        start = (int)cinCache.startTime;
	     //   while(  (cinCache.tfps != cinCache.numQuads)
		 //         && (cinCache.status == e_status.FMV_PLAY) ) 
	      //  {
		        RoQInterrupt();
               // if (cinCache.numSoundFrames == 0)
               //     RoQInterrupt();

		    //    if (start != cinCache.startTime) {
			 //       // we need to use CL_ScaledMilliseconds because of the smp mode calls from the renderer
             //       cinCache.tfps = ((((ScaledMilliseconds())
			//				          - cinCache.startTime)*3)/100);
			  //      start = (int)cinCache.startTime;
		     //   }
	       // }

            cinCache.lastTime = (uint)thisTime;

	      //  if (cinTable[currentHandle].status == FMV_LOOPED) {
		 //       cinTable[currentHandle].status = FMV_PLAY;
	     //   }

	    //    if (cinTable[currentHandle].status == FMV_EOF) {
	    //      if (cinTable[currentHandle].looping) {
		//        RoQReset();
	     //     } else {
		  //      RoQShutdown();
	      //    }
	      //  }
        }

        /*
        ==================
        InitCinematic

        ==================
        */
        public int InitCinematic() {
	        ushort RoQID;

	        Engine.common.DPrintf("SCR_PlayCinematic\n");

	        cinCache.ROQSize = cinCache.iFile.Length();

	        if (cinCache.ROQSize<=0) {
		        Engine.common.DPrintf("play(%s), ROQSize<=0\n");
		        return -1;
	        }

	        cinCache.CIN_HEIGHT = DEFAULT_CIN_HEIGHT;
	        cinCache.CIN_WIDTH  =  DEFAULT_CIN_WIDTH;
	        cinCache.holdAtEnd = false; //(systemBits & CIN_hold) != 0;
	        cinCache.alterGameState = false; //(systemBits & CIN_system) != 0;
	        cinCache.playonwalls = 1;
	        cinCache.silent = false; //(systemBits & CIN_silent) != 0;
	        cinCache.shader = false; //(systemBits & CIN_shader) != 0;

	        initRoQ();

            cinCache.iFile.Seek(idFileSeekOrigin.FS_SEEK_SET, ROQ_BASE_POSITION);
            cin.file = cinCache.iFile.ReadBytes(16);
	        
	        RoQID = (ushort)((ushort)(cin.file[0]) + (ushort)(cin.file[1])*256);
	        if (RoQID == 0x1084)
	        {
		        RoQ_init();
        //		FS_Read (cin.file, cinTable[currentHandle].RoQFrameSize+8, cinTable[currentHandle].iFile);
		        // let the background thread start reading ahead
		        cinCache.status = e_status.FMV_PLAY;
		        Engine.common.DPrintf("trFMV::play(), playing...\n");


                return 1;
	        }
	        Engine.common.Warning("trFMV::play(), invalid RoQ ID\n");
	        return -1;
        }

        public override void SetExtents(int x, int y, int w, int h)
        {
	        if (cinCache.status == e_status.FMV_EOF) return;
	        cinCache.xpos = x;
	        cinCache.ypos = y;
	        cinCache.width = w;
	        cinCache.height = h;
	        cinCache.dirty = true;
        }

        public override void SetLooping( bool loop) {
	        if (cinCache.status == e_status.FMV_EOF) return;
	        cinCache.looping = loop;
        }

        //
        // DrawCinematic
        //
        private idImage blitImage;
        public override void DrawCinematic()
        {
            if (cinCache.isInit == false)
            {
                InitCinematic();
                cinCache.isInit = true;

                RunCinematic();

                cinCache.bufmemory = new Color[cinCache.CIN_WIDTH * cinCache.CIN_HEIGHT];
                blitImage = Engine.imageManager.CreateImage("*cinematic", cinCache.bufmemory, (int)cinCache.CIN_WIDTH, (int)cinCache.CIN_HEIGHT, false, false, SamplerState.AnisotropicWrap);
                Engine.RenderSystem.DrawStrechPic(cinCache.xpos, cinCache.ypos, (int)cinCache.width, (int)cinCache.height, blitImage);

                
                return;
            }

            RunCinematic();

            blitImage.BlitImageData(ref cinCache.bufmemory);
            Engine.RenderSystem.DrawStrechPic(cinCache.xpos, cinCache.ypos, (int)cinCache.width, (int)cinCache.height, blitImage);

            return;
        }
    }
#else
    //
    // idVideo
    //
    public class idVideoLocal : idVideo
    {
        Video videoHandle;
        VideoPlayer videoplayer;
        bool isInit = false;
        idImageLocal blitImage;
        int cin_x, cin_y, cin_w, cin_h;

        public idVideoLocal(string filepath)
        {
            videoHandle = Engine.fileSystem.ReadContent<Video>(filepath);
            videoplayer = new VideoPlayer();
        }

        public override void SetLooping(bool loop)
        {

        }

        public override void SetExtents(int x, int y, int w, int h)
        {
            cin_x = x;
            cin_y = y;
            cin_w = w;
            cin_h = h;
        }

        public override void DrawCinematic()
        {
            videoplayer.Play(videoHandle);

            if (isInit == false)
            {
                blitImage = new idImageLocal();
                isInit = true;
            }

            blitImage.BlitD3DHandle(videoplayer.GetTexture());
            Engine.RenderSystem.DrawStrechPic(cin_x, cin_y, cin_w, cin_h, blitImage);
        }

        public override void Dispose()
        {
            if (videoplayer != null)
            {
                videoplayer.Dispose();
                videoplayer = null;
                videoHandle = null;
            }
        }
        
        public override void StopCinematic()
        {
            Dispose();
        }

        public override e_status GetStatus()
        {
            if (isInit == false)
            {
                // Advance one frame.
                DrawCinematic();
            }
            if (videoplayer == null)
            {
                return e_status.FMV_EOF;
            }

            if (videoplayer.State == MediaState.Playing)
            {
                return e_status.FMV_PLAY;
            }

            return e_status.FMV_EOF;
        }
    }
#endif
}
