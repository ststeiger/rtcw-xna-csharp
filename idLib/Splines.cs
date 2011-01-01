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

using System.Collections.Generic;
using idLib.Math;

namespace idLib
{
    //
    // idSplineList
    //
    public class idSplineList {
        private string name;
        
        private List<idVector3> controlPoints = new List<idVector3>();
        private List<idVector3> splinePoints = new List<idVector3>();
        private List<double> splineTime = new List<double>();
        private idVector3 pathColor, segmentColor, controlColor, activeColor;
        private float granularity;
        private bool editMode;
        private bool dirty;
        private int activeSegment;
        private long baseTime;
        private long time;

        public idSplineList() {
	        clear();
        }

        public idSplineList( string p ) {
	        clear();
	        name = p;
        }

        private void clearControl() {
	        controlPoints.Clear();
        }

        void clearSpline() {
	        splinePoints.Clear();
        }

        public void parse( ref idParser parser )
        {
            string token = "";
	        //Com_MatchToken( text, "{" );
	        do {
                if (parser.ReachedEndOfBuffer == true)
                    break;

                token = parser.NextToken;

		        if ( token == null || token.Length <= 0 ) {
			        break;
		        }
		        if ( token == "}" ) {
			        break;
		        }

		        do {
			        // if token is not a brace, it is a key for a key/value pair
                    if (token == null || token.Length <= 0 || token == "(" || token == "}")
                    {
				        break;
			        }

                    parser.UngetToken();
                    string key = parser.GetNextTokenFromLine();

                    token = parser.NextToken;
			        if ( key == "granularity" ) {
				        granularity = float.Parse( token );
			        } else if ( key == "name" ) {
				        name = token;
			        }
                    token = parser.NextToken;

		        } while ( true );

		        if ( token == "}" ) {
			        break;
		        }

                parser.UngetToken();

		        // read the control point
		        idVector3 point = idVector3.vector_origin;
                parser.NextVector3(ref point);
		        addPoint( point );
	        } while ( true );

	        //Com_UngetToken();
	        //Com_MatchToken( text, "}" );
	        dirty = true;
        }

        public void addPoint(idVector3 p)
        {
            controlPoints.Add(p);
            dirty = true;
        }

        public float calcSpline( int step, float tension )
        {
            switch (step)
            {
                case 0: return ((float)System.Math.Pow(1 - tension, 3)) / 6;
                case 1: return (3 * (float)System.Math.Pow(tension, 3) - 6 * (float)System.Math.Pow(tension, 2) + 4) / 6;
                case 2: return (-3 * (float)System.Math.Pow(tension, 3) + 3 * (float)System.Math.Pow(tension, 2) + 3 * tension + 1) / 6;
                case 3: return (float)System.Math.Pow(tension, 3) / 6;
            }
            return 0.0f;
        }

        public void clear() {
	        clearControl();
	        clearSpline();
	        splineTime.Clear();
	        dirty = true;
	        activeSegment = 0;
	        granularity = 0.025f;
	        pathColor = new idVector3( 1.0f, 0.5f, 0.0f );
	        controlColor = new idVector3( 0.7f, 0.0f, 1.0f );
	        segmentColor = new idVector3( 0.0f, 0.0f, 1.0f );
	        activeColor = new idVector3( 1.0f, 0.0f, 0.0f );
        }


        public void initPosition( long startTime, long totalTime )
        {
            if (dirty)
            {
                buildSpline();
            }

            if (splinePoints.Count == 0)
            {
                return;
            }

            baseTime = startTime;
            time = totalTime;

            // calc distance to travel ( this will soon be broken into time segments )
            splineTime.Clear();
            splineTime.Add(startTime);
            double dist = totalDistance();
            double distSoFar = 0.0;
            idVector3 temp;
            int count = splinePoints.Count;
            //for(int i = 2; i < count - 1; i++) {
            for (int i = 1; i < count; i++)
            {
                temp = splinePoints[i - 1];
                temp -= splinePoints[i];
                distSoFar += temp.Length();
                double percent = distSoFar / dist;
                percent *= totalTime;
                splineTime.Add(percent + startTime);
            }
            activeSegment = 0;
        }

        public idVector3 getPosition( long time )
        {
            idVector3 interpolatedPos;
        //	static long lastTime = -1; // TTimo unused

            int count = splineTime.Count;
	        if ( count == 0 ) {
                return idVector3.vector_origin;
	        }

        //	Com_Printf("Time: %d\n", t);

	        while ( activeSegment < count ) {
                if (splineTime[activeSegment] >= time)
                {
			        if ( activeSegment > 0 && activeSegment < count - 1 ) {
                        float timeHi = (float)splineTime[activeSegment + 1];
                        float timeLo = (float)splineTime[activeSegment - 1];
                        float percent = (timeHi - time) / (timeHi - timeLo);
				        // pick two bounding points
				        idVector3 v1 = splinePoints[activeSegment - 1];
                        idVector3 v2 = splinePoints[activeSegment + 1];
				        v2 *= ( 1.0f - percent );
				        v1 *= percent;
				        v2 += v1;
				        interpolatedPos = v2;
				        return interpolatedPos;
			        }
			        return splinePoints[activeSegment];
		        } else {
			        activeSegment++;
		        }
	        }
	        return splinePoints[count - 1];
        }

        private void buildSpline()
        {
            //int start = Sys_Milliseconds();
            clearSpline();
            for (int i = 3; i < controlPoints.Count; i++)
            {
                for (float tension = 0.0f; tension < 1.001f; tension += granularity)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        x += controlPoints[i - (3 - j)].X * calcSpline(j, tension);
                        y += controlPoints[i - (3 - j)].Y * calcSpline(j, tension);
                        z += controlPoints[i - (3 - j)].Z * calcSpline(j, tension);
                    }
                    splinePoints.Add(new idVector3(x, y, z));
                }
            }
            dirty = false;
        }

        public void setGranularity( float f ) {
	        granularity = f;
        }

        public float getGranularity() {
	        return granularity;
        }

        public int numPoints() {
	        return controlPoints.Count;
        }

        public idVector3 getPoint( int index ) {
	        return controlPoints[index];
        }

        public idVector3 getSegmentPoint( int index ) {
	        return splinePoints[index];
        }


        public void setSegmentTime( int index, int time ) {
	        splineTime[index] = time;
        }

        public int getSegmentTime( int index ) {
	        return (int)splineTime[index];
        }

        public void addSegmentTime( int index, int time ) {
	        splineTime[index] += time;
        }

        public float totalDistance()
        {
            // FIXME: save dist and return
            //
            if (controlPoints.Count == 0)
            {
                return 0.0f;
            }

            if (dirty)
            {
                buildSpline();
            }

            float dist = 0.0f;
            idVector3 temp;
            int count = splinePoints.Count;
            for (int i = 1; i < count; i++)
            {
                temp = splinePoints[i - 1];
                temp -= splinePoints[i];
                dist += temp.Length();
            }
            return dist;
        }

        public int getActiveSegment() {
	        return activeSegment;
        }

        public void setActiveSegment( int i ) {
	        //assert(i >= 0 && (splinePoints.Num() > 0 && i < splinePoints.Num()));
	        activeSegment = i;
        }

        public int numSegments() {
	        return splinePoints.Count;
        }

        public string getName() {
	        return name;
        }

        public void setName( string p ) {
	        name = p;
        }

        public bool validTime() {
	        if ( dirty ) {
		        buildSpline();
	        }
	        // gcc doesn't allow static casting away from bools
	        // why?  I've no idea...
            return (bool)(splineTime.Count > 0 && splineTime.Count == splinePoints.Count);
        }

        public void setTime( long t ) {
	        time = t;
        }

        public void setBaseTime( long t ) {
	        baseTime = t;
        }
    }
}



