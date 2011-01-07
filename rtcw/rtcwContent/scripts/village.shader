textures/village/villwin_c12a
{
	qer_editorimage textures/village/villwin_c12a.tga
	// surfaceparm nomarks
	// q3map_lightimage textures/village/villwin_c12.tga
	q3map_surfacelight 300
//	light 1


	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/village/villwin_c12a.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map textures/village/villwin_c12a.blend.tga
		blendfunc GL_ONE GL_ONE
	}
}

textures/village/villwin_c12m
{
	{
		map textures/village/villwin_c12m.tga
		blendFunc GL_ONE GL_ZERO
		rgbGen identity
	}
	{
		map textures/effects/vilfx2.tga
		blendFunc GL_ONE_MINUS_DST_ALPHA GL_ONE
		tcMod Scale 1.5 1.5  
		tcGen environment
	}
	{
		map $lightmap
		blendFunc GL_DST_COLOR GL_ONE_MINUS_DST_ALPHA
		rgbGen identity
	}
}

textures/village/vill2_inwin1
{
	qer_editorimage textures/village/villwin_c12m.tga
	{
		map textures/village/villwin_c12m.tga
		blendFunc GL_ONE GL_ZERO
		rgbGen identity
	}
	{
		map textures/effects/vilfx2.tga
		blendFunc GL_ONE_MINUS_DST_ALPHA GL_ONE
		tcMod Scale 1.5 1.5  
		tcGen environment
	}
	{
		map $lightmap
		blendFunc GL_DST_COLOR GL_ONE_MINUS_DST_ALPHA
		rgbGen identity
	}
}

textures/village/villwin_c15
{
	qer_editorimage textures/village/villwin_c15.tga
	surfaceparm nomarks
	// q3map_lightimage textures/village/villwin_c15.tga
	q3map_surfacelight 300
//	light 1


	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/village/villwin_c15.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map textures/village/villwin_c15.blend.tga
		blendfunc GL_ONE GL_ONE
	}
}

textures/village/villwin_c18
{
	qer_editorimage textures/village/villwin_c18.tga
	surfaceparm nomarks
	// q3map_lightimage textures/village/villwin_c17.tga
	q3map_surfacelight 200
//	light 1
	q3map_lightsubdivide 128

	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/village/villwin_c18.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map textures/village/villwin_c18.blend.tga
		blendfunc GL_ONE GL_ONE
	}
}

textures/village/villdoor_c06

{
	surfaceparm woodsteps
	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/village/villdoor_c06.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
}

////////////////
//village door//
////////////////

textures/village_door/villdoor_c03
{
	qer_editorimage textures/village_door/villdoor_c03.tga
	// surfaceparm nomarks
	// q3map_lightimage textures/village_door/villdoor_c03.tga
	q3map_surfacelight 300
//	light 1


	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/village_door/villdoor_c03.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map textures/village_door/villdoor_c03.blend.tga
		blendfunc GL_ONE GL_ONE
	}
}

textures/village_door/villdoor_c04
{
	qer_editorimage textures/village_door/villdoor_c04.tga
	// surfaceparm nomarks
	// q3map_lightimage textures/village_door/villdoor_c04.tga
	q3map_surfacelight 300
//	light 1


	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/village_door/villdoor_c04.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map textures/village_door/villdoor_c04.blend.tga
		blendfunc GL_ONE GL_ONE
	}
}

textures/village_door/villdoor_c05
{
	qer_editorimage textures/village_door/villdoor_c05.tga
	// surfaceparm nomarks
	// q3map_lightimage textures/village_door/villdoor_c05.tga
	q3map_surfacelight 300
//	light 1


	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/village_door/villdoor_c05.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map textures/village_door/villdoor_c05.blend.tga
		blendfunc GL_ONE GL_ONE
	}
}
