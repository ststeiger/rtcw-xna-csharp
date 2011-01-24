/*
===========================================================================
Copyright (C) 1999-2005 Id Software, Inc.

This file is part of Quake III Arena source code.

Quake III Arena source code is free software; you can redistribute it
and/or modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the License,
or (at your option) any later version.

Quake III Arena source code is distributed in the hope that it will be
useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Foobar; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
===========================================================================
*/

// jv - completely took this out.

#include <windows.h>
#include <GL/gl.h>
#include <GL/glu.h>

#include "qbsp.h"

// can't use the glvertex3fv functions, because the vec3_t fields
// could be either floats or doubles, depending on DOUBLEVEC_T

qboolean	drawflag;
vec3_t	draw_mins, draw_maxs;


#define	WIN_SIZE	512

void InitWindow (void)
{
   
}

void Draw_ClearWindow (void)
{

}

void Draw_SetRed (void)
{
	
}

void Draw_SetGrey (void)
{
	
}

void Draw_SetBlack (void)
{
	
}

void DrawWinding (winding_t *w)
{
	
}

void DrawAuxWinding (winding_t *w)
{
	
}

//============================================================

#define	GLSERV_PORT	25001

qboolean	wins_init;
int			draw_socket;

void GLS_BeginScene (void)
{
	
}

void GLS_Winding (winding_t *w, int code)
{
	
}

void GLS_EndScene (void)
{
	
}
