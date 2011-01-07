/////////////
//town wall//
/////////////

textures/town_wall/church_c01_shadow
{
	qer_editorimage town_wall/church_c01.tga
	{
		map textures/town_wall/church_c01.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
	{
		map $lightmap
		rgbGen identity
		tcMod scale 1 1
		tcMod turb 0 .1 0 .1
	}

}

textures/town_wall/store_c01

{
	surfaceparm woodsteps
	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/town_wall/store_c01.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
}
textures/town_wall/store_c02

{
	surfaceparm woodsteps
	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/town_wall/store_c02.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
}
textures/town_wall/store_c03_a

{
	surfaceparm woodsteps
	{
		map $lightmap
		rgbGen identity
	}
	{
		map textures/town_wall/store_c03_a.tga
		blendFunc GL_DST_COLOR GL_ZERO
		rgbGen identity
	}
}

