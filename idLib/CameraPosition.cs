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

using System;
using System.Collections.Generic;
using idLib.Math;

namespace idLib
{
    public class idPointListInterface {
        public idPointListInterface() {
	        
        }

        public virtual int numPoints() {
	        return 0;
        }

        public virtual void addPoint( float x, float y, float z ) {}
        public virtual void addPoint( idVector3 p ) {}
        public virtual void removePoint( int index ) {}
        public virtual idVector3 getPoint(int index) { return idVector3.vector_origin; }
    }

    // can either be a look at or origin position for a camera
    //
    public class idCameraPosition : idPointListInterface {
        internal static string[] positionStr = new string[(int)positionType.POSITION_COUNT];
        internal long startTime;
        internal long time;
        internal positionType type;
        internal string name;
        internal bool editMode;
        internal List<idVelocity> velocities = new List<idVelocity>();
        internal float baseVelocity;

        public virtual void clearVelocities() {
	        velocities.Clear();
        }

        public virtual void clear() {
	        editMode = false;
	        clearVelocities();
        }

        public idCameraPosition( string p ) {
	        name = p;
        }

        public idCameraPosition() {
	        time = 0;
	        name = "position";
        }

        public idCameraPosition( long t ) {
	        time = t;
        }

        // this can be done with RTTI syntax but i like the derived classes setting a type
        // makes serialization a bit easier to see
        //
        public enum positionType {
	        FIXED = 0x00,
	        INTERPOLATED,
	        SPLINE,
	        POSITION_COUNT
        };


        public virtual void start( long t ) {
	        startTime = t;
        }

        public long getTime() {
	        return time;
        }

        public virtual void setTime( long t ) {
	        time = t;
        }

        public float getBaseVelocity() {
	        return baseVelocity;
        }

        public float getVelocity( long t ) {
	        long check = t - startTime;
	        for ( int i = 0; i < velocities.Count; i++ ) {
		        if ( check >= velocities[i].startTime && check <= velocities[i].startTime + velocities[i].time ) {
			        return velocities[i].speed;
		        }
	        }
	        return baseVelocity;
        }

        public void addVelocity( long start, long duration, float speed ) {
	        velocities.Add( new idVelocity( start, duration, speed ) );
        }

        public virtual idVector3 getPosition( long t ) {
	        return idVector3.vector_origin;
        }

        public virtual void parse( ref idParser parser ) {}

        public virtual bool parseToken( string key, ref idParser parser )
        {
            string token = parser.NextToken;
	        if ( key == "time" ) {
		        time = long.Parse( token );
		        return true;
	        } else if ( key == "type" ) {
		        type = (positionType)( int.Parse( token ) );
		        return true;
	        } else if ( key == "velocity" ) {
		        long t = long.Parse( token );
		        token = parser.NextToken;
		        long d = long.Parse( token );
		        token = parser.NextToken;
		        float s = float.Parse( token );
		        addVelocity( t, d, s );
		        return true;
	        } else if ( key == "baseVelocity" ) {
		        baseVelocity = float.Parse( token );
		        return true;
	        } else if ( key == "name" ) {
		        name = token;
		        return true;
	        } else if ( key == "time") {
		        time = int.Parse( token );
		        return true;
	        }
	        parser.UngetToken();
	        return false;
        }


        public string getName() {
	        return name;
        }

        public void setName( string p ) {
	        name = p;
        }

        public string typeStr() {
	        return positionStr[(int)( type )];
        }

        public void calcVelocity( float distance ) {
	        if ( time != 0 ) {   //DAJ BUGFIX
		        float secs = (float)time / 1000;
		        baseVelocity = distance / secs;
	        }
        }
    };

    //
    // idFixedPosition
    //
    public class idFixedPosition : idCameraPosition {
        idVector3 pos;

        public void init() {
	        pos = idVector3.vector_origin;
	        type = positionType.FIXED;
        }

        public idFixedPosition() : base() {
	        init();
        }

        public idFixedPosition( idVector3 p ) : base() {
	        init();
	        pos = p;
        }

        public override void addPoint( idVector3 v ) {
	        pos = v;
        }

        public override void addPoint(float x, float y, float z)
        {
	        pos.X = x;
            pos.Y = y;
            pos.Z = z;
        }

        public override idVector3 getPosition(long t)
        {
	        return pos;
        }

        public override void parse(ref idParser parser)
        {
            string token;

            parser.ExpectNextToken("{");

	        do {
                if(parser.ReachedEndOfBuffer == true)
                    break;

		        token = parser.NextToken;

		        if ( token == null || token.Length <= 0 ) {
			        break;
		        }
		        if ( token == "}" ) {
			        break;
		        }

		        // here we may have to jump over brush epairs ( only used in editor )
		        do {
			        // if token is not a brace, it is a key for a key/value pair
			        if ( token == null || token.Length <= 0 || token == "(" || token == "}" ) {
				        break;
			        }

                    parser.UngetToken();
			        
			        string key = parser.GetNextTokenFromLine();

			        if ( key == "pos" ) {
				        parser.NextVector3( ref pos );
			        } else {
				        base.parseToken( key, ref parser );
			        }
			        token = parser.NextToken;

		        } while ( true );

		        if ( token == "}" ) {
			        break;
		        }

	        } while ( true );

            parser.UngetToken();
	        parser.ExpectNextToken("}");
        }

        public override int numPoints()
        {
	        return 1;
        }

        public override idVector3 getPoint(int index)
        {
	        if ( index != 0 ) {
		        throw new Exception("index != 0");
	        }
	        return pos;
        }
    };

    public class idInterpolatedPosition : idCameraPosition {
        internal bool first;
        internal idVector3 startPos;
        internal idVector3 endPos;
        internal long lastTime;
        internal float distSoFar;

        private void init() {
	        type = positionType.INTERPOLATED;
	        first = true;
	        startPos = idVector3.vector_origin;
	        endPos= idVector3.vector_origin;
        }

        public idInterpolatedPosition() : base() {
	        init();
        }

        public idInterpolatedPosition( idVector3 start, idVector3 end, long time ) : base( time ) {
	        init();
	        startPos = start;
	        endPos = end;
        }

        public override idVector3 getPosition(long t)
        {
	        float percent = 0.0f;
	        float velocity = getVelocity( t );
	        float timePassed = t - lastTime;
	        lastTime = t;

	        // convert to seconds
	        timePassed /= 1000;

	        float distToTravel = timePassed * velocity;

	        idVector3 temp = startPos;
	        temp -= endPos;
	        float distance = temp.Length();

	        distSoFar += distToTravel;

	        // TTimo
	        // show_bug.cgi?id=409
	        // avoid NaN on fixed cameras
	        if ( distance != 0.0 ) {   //DAJ added to protect DBZ
		        percent = (float)( distSoFar ) / distance;
	        }

	        if ( percent > 1.0 ) {
		        percent = 1.0f;
	        } else if ( percent < 0.0 ) {
		        percent = 0.0f;
	        }

	        // the following line does a straigt calc on percentage of time
	        // float percent = (float)(startTime + time - t) / time;

	        idVector3 v1 = startPos;
	        idVector3 v2 = endPos;
	        v1 *= ( 1.0f - percent );
	        v2 *= percent;
	        v1 += v2;
	        return v1;
        }

        public override void parse(ref idParser parser)
        {
            string token;

            parser.ExpectNextToken( "{" );
	        do {
		         if(parser.ReachedEndOfBuffer == true)
                    break;

		        token = parser.NextToken;

		        if ( token == null || token.Length <= 0 ) {
			        break;
		        }
		        if ( token == "}" ) {
			        break;
		        }

		        // here we may have to jump over brush epairs ( only used in editor )
		        do {
			        // if token is not a brace, it is a key for a key/value pair
			        if ( token == null || token.Length <= 0 || token == "(" || token == "}" ) {
				        break;
			        }

			      //  parser.UngetToken();
                    string key = token; //parser.GetNextTokenFromLine();

			        if ( key == "startPos") {
                        parser.NextVector3(ref startPos);
				        
			        } else if ( key == "endPos" ) {
				        parser.NextVector3(ref endPos);
			        } else {
				        base.parseToken( key, ref parser );
			        }
			        token = parser.NextToken;

		        } while ( true );

		        if ( token == "}" ) {
			        break;
		        }

	        } while ( true );

	        parser.UngetToken();
	        parser.ExpectNextToken("}");
        }

        public override int numPoints()
        {
	        return 2;
        }

        public override idVector3 getPoint(int index)
        {
	        if ( index == 0 ) {
		        return startPos;
	        }
	        return endPos;
        }

        public override void addPoint(float x, float y, float z)
        {
	        if ( first ) {
                startPos.X = x;
                startPos.Y = y;
                startPos.Z = z;
		        first = false;
	        } else {
                endPos.X = x;
                endPos.Y = y;
                endPos.Z = z;
		        first = true;
	        }
        }

        public override void addPoint(idVector3 v)
        {
	        if ( first ) {
		        startPos = v;
		        first = false;
	        } else {
		        endPos = v;
		        first = true;
	        }
        }

        public override void start(long t)
        {
	        base.start( t );
	        lastTime = startTime;
	        distSoFar = 0.0f;
	        idVector3 temp = startPos;
	        temp -= endPos;
	        calcVelocity( temp.Length() );
        }
    };

    
    public class idSplinePosition : idCameraPosition {
        private idSplineList target = new idSplineList();
        private long lastTime;
        private float distSoFar;

        public void init() {
	        type = positionType.SPLINE;
        }

        public idSplinePosition() : base() {
	        init();
        }

        public idSplinePosition( long time ) : base( time ) {
	        init();
        }

        public override void start(long t)
        {
	        base.start( t );
	        target.initPosition( t, time );
	        lastTime = startTime;
	        distSoFar = 0.0f;
	        calcVelocity( target.totalDistance() );
        }


        public override idVector3 getPosition(long t)
        {
	        float velocity = getVelocity( t );
	        float timePassed = t - lastTime;
	        lastTime = t;

	        // convert to seconds
	        timePassed /= 1000;

	        float distToTravel = timePassed * velocity;

	        distSoFar += distToTravel;
	        double tempDistance = target.totalDistance();

	        double percent = (double)( distSoFar ) / tempDistance;

	        double targetDistance = percent * tempDistance;
	        tempDistance = 0;

	        double lastDistance1,lastDistance2;
	        lastDistance1 = lastDistance2 = 0;
	        //FIXME: calc distances on spline build
	        idVector3 temp;
	        int count = target.numSegments();
	        // TTimo fixed MSVCism
	        int i;
	        for ( i = 1; i < count; i++ ) {
		        temp = target.getSegmentPoint( i - 1 );
		        temp -= target.getSegmentPoint( i );
		        tempDistance += temp.Length();
		        if ( (i & 1) != 0 ) {
			        lastDistance1 = tempDistance;
		        } else {
			        lastDistance2 = tempDistance;
		        }
		        if ( tempDistance >= targetDistance ) {
			        break;
		        }
	        }

	        if ( i >= count - 1 ) {
		        return target.getSegmentPoint( i - 1 );
	        } else {
        #if false
		        double timeHi = target.getSegmentTime( i + 1 );
		        double timeLo = target.getSegmentTime( i - 1 );
		        double percent = ( timeHi - t ) / ( timeHi - timeLo );
		        idVec3 v1 = *target.getSegmentPoint( i - 1 );
		        idVec3 v2 = *target.getSegmentPoint( i + 1 );
		        v2 *= ( 1.0 - percent );
		        v1 *= percent;
		        v2 += v1;
		        interpolatedPos = v2;
        #else
		        if ( lastDistance1 > lastDistance2 ) {
			        double d = lastDistance2;
			        lastDistance2 = lastDistance1;
			        lastDistance1 = d;
		        }

		        idVector3 v1 = target.getSegmentPoint( i - 1 );
		        idVector3 v2 = target.getSegmentPoint( i );
		        float percent2 = (float)(( lastDistance2 - targetDistance ) / ( lastDistance2 - lastDistance1 ));
		        v2 *= ( 1.0f - percent2 );
		        v1 *= percent2;
		        v2 += v1;
		        //interpolatedPos = v2;
                return v2;
        #endif
	        }
        }

        public void addControlPoint( idVector3 v ) {
	        target.addPoint( v );
        }

        public override void parse(ref idParser parser)
        {
             string token;

            parser.ExpectNextToken( "{" );
	        do {
		         if(parser.ReachedEndOfBuffer == true)
                    break;

		        token = parser.NextToken;

		        if ( token == null || token.Length <= 0 ) {
			        break;
		        }
		        if ( token == "}" ) {
			        break;
		        }

		        // here we may have to jump over brush epairs ( only used in editor )
		        do {
			        // if token is not a brace, it is a key for a key/value pair
			        if ( token == null || token.Length <= 0 || token == "(" || token == "}" ) {
				        break;
			        }

			        parser.UngetToken();
			        string key = parser.GetNextTokenFromLine();

			        token = parser.NextToken;
			        if ( key == "target" ) {
				        target.parse( ref parser );
			        } else {
				        parser.UngetToken();
                        base.parseToken(key, ref parser );
			        }
			        token = parser.NextToken;

		         } while ( true );

		        if ( token == "}" ) {
			        break;
		        }

	        } while ( true );

	        parser.UngetToken();
	        parser.ExpectNextToken("}");
        }

        public override int numPoints()
        {
	        return target.numPoints();
        }

        public override idVector3 getPoint(int index)
        {
	        return target.getPoint( index );
        }

        public override void addPoint(idVector3 v)
        {
	        target.addPoint( v );
        }

        public override void addPoint(float x, float y, float z)
        {
            idVector3 v = new idVector3( x, y, z );
	        target.addPoint( v );
        }

    };
}
