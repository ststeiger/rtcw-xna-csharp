textures/training/trees_m01
{

 // invalid JPW FIXME SP merge   cull front
    {  
        map textures/training/trees_m01.tga
	  blendFunc GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA
//      alphaFunc GE128
        depthWrite
        rgbGen identity

    }
}

textures/training/window_m04
{
	qer_editorimage textures/training/window_m04.tga
	// surfaceparm nomarks
	q3map_lightimage textures/chateau/soft_blue.tga       // color for window
	q3map_surfacelight 300
	q3map_lightsubdivide 128
//	light 1


	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/training/window_m04.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map textures/chateau/window_m01.blend.tga
		blendfunc GL_ONE GL_ONE
	}
}