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
using idLib.Engine.Public;

namespace idLib
{
    // (SA) making a list of cameras so I can use
    //		the splines as targets for other things.
    //		Certainly better ways to do this, but this lets
    //		me get underway quickly with ents that need spline
    //		targets.
    public static class idCameraManager
    {
        public const int MAX_CAMERAS = 64;

        private static idCameraDef[] camera = new idCameraDef[MAX_CAMERAS];

        public static bool loadCamera( int camNum, string name ) {
	        if ( camNum < 0 || camNum >= MAX_CAMERAS ) {
		        return false;
	        }

            if (camera[camNum] == null)
                camera[camNum] = new idCameraDef();

	        camera[camNum].clear();
	        return camera[camNum].load( name );
        }

        //
        // GetCameraInfo
        //
        public static void getCameraInfo(int camNum, int time, ref idVector3 origin, ref idVector3 angles, ref float fov)
        {
            idVector3 dir, org;

            dir = idVector3.vector_origin;
            org = origin;
            if (camera[camNum].getCameraInfoLocal(time, ref org, ref dir, ref fov))
            {
                origin[0] = org[0];
                origin[1] = org[1];
                origin[2] = org[2];
                angles[1] = (float)(System.Math.Atan2(dir[1], dir[0]) * 180 / 3.14159);
                angles[0] = (float)(System.Math.Asin(dir[2]) * 180 / 3.14159);
            }
        }
    }

    //
    // idCameraFOV
    //
    public class idCameraFOV {
        protected float fov;
        protected float startFOV;
        protected float endFOV;
        protected int startTime;
        protected int time;
        protected float length;

        public idCameraFOV() {
	        time = 0;
	        length = 0;
	        fov = 90;
        }

        public idCameraFOV( int v ) {
	        time = 0;
	        length = 0;
	        fov = v;
        }

        public idCameraFOV( int s, int e, long t ) {
	        startFOV = s;
	        endFOV = e;
	        length = t;
        }

        public void setFOV( float f ) {
	        fov = f;
        }

        public float getFOV( long t ) {
	        if ( length != 0 ) {
		        float percent = ( t - startTime ) / length;
		        if ( percent < 0.0 ) {
			        percent = 0.0f;
		        } else if ( percent > 1.0 ) {
			        percent = 1.0f;
		        }
		        float temp = endFOV - startFOV;
		        temp *= percent;
		        fov = startFOV + temp;

		        if ( percent == 1.0 ) {
			        length = 0.0f;
		        }
	        }
	        return fov;
        }

        public void start( long t ) {
	        startTime = (int)t;
        }

        public void reset( float startfov, float endfov, int start, float len ) {
	        startFOV = startfov;
	        endFOV = endfov;
	        startTime = start;
	        length = len * 1000;
        }

        public void parse( ref idParser parser )
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
			        if ( key == "fov") {
				        fov = float.Parse( token );
			        } else if ( key == "startFOV" ) {
				        startFOV = float.Parse( token );
			        } else if ( key == "endFOV" ) {
				        endFOV = float.Parse( token );
			        } else if ( key == "time" ) {
				        time = int.Parse( token );
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
    };

    //
    // idCameraEvent
    //
    public class idCameraEvent {
        public enum eventType {
	        EVENT_NA = 0x00,
	        EVENT_WAIT,             //
	        EVENT_TARGETWAIT,       //
	        EVENT_SPEED,            //
	        EVENT_TARGET,           // char(name)
	        EVENT_SNAPTARGET,       //
	        EVENT_FOV,              // int(time), int(targetfov)
	        EVENT_CMD,              //
	        EVENT_TRIGGER,          //
	        EVENT_STOP,             //
	        EVENT_CAMERA,           //
	        EVENT_FADEOUT,          // int(time)
	        EVENT_FADEIN,           // int(time)
	        EVENT_FEATHER,          //
	        EVENT_COUNT
        };
                
        private eventType type;
        private string paramStr;
        private long time;
        private bool triggered;

        public static string[] eventStr = new string[]{
	        "NA",
	        "WAIT",
	        "TARGETWAIT",
	        "SPEED",
	        "TARGET",
	        "SNAPTARGET",
	        "FOV",
	        "CMD",
	        "TRIGGER",
	        "STOP",
	        "CAMERA",
	        "FADEOUT",
	        "FADEIN",
	        "FEATHER"
        };

        public idCameraEvent() {
	        paramStr = "";
	        type = eventType.EVENT_NA;
	        time = 0;
        }

        public idCameraEvent( eventType t, string param, long n ) {
	        type = t;
	        paramStr = param;
	        time = n;
        }

        public eventType getType() {
	        return type;
        }

        public string typeStr() {
	        return eventStr[(int)eventType.EVENT_COUNT];
        }

        public string getParam() {
	        return paramStr;
        }

        public long getTime() {
	        return time;
        }

        public void setTime( long n ) {
	        time = n;
        }

        public void Parse( ref idParser parser )
        {
            string token;

            parser.ExpectNextToken( "{" );
	        do {
                if( parser.ReachedEndOfBuffer )
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
			        if (key == "type" ) {
				        type = (idCameraEvent.eventType)( int.Parse( token ) );
			        } else if ( key == "param" ) {
				        paramStr = token;
			        } else if ( key == "time" ) {
				        time = int.Parse( token );
			        }
			        token = parser.NextToken;

		        } while ( true );

		        if ( token == "}" ) {
			        break;
		        }

	        } while ( true );

	        parser.UngetToken();
            parser.ExpectNextToken( "}" );
        }

        public void setTriggered( bool b ) {
	        triggered = b;
        }

        public bool getTriggered() {
	        return triggered;
        }

    };

    public class idCameraDef
    {
        protected string name;
        protected int currentCameraPosition;
        protected idVector3 lastDirection;
        protected bool cameraRunning;
        protected idCameraPosition cameraPosition;
        protected List<idCameraPosition> targetPositions = new List<idCameraPosition>();
        protected List<idCameraEvent> events = new List<idCameraEvent>();
        protected idCameraFOV fov = new idCameraFOV();
        protected int activeTarget;
        protected float totalTime;
        protected float baseTime;
        protected long startTime;

        //
        // getActiveSegmentInfo
        //
        private void getActiveSegmentInfo( int segment, ref idVector3 origin, ref idVector3 direction, ref float fov ) 
        {
        #if false
	        if ( !cameraSpline.validTime() ) {
		        buildCamera();
	        }
	        double d = (double)segment / numSegments();
	        getCameraInfo( d * totalTime * 1000, origin, direction, fov );
        #endif
        /*
	        if (!cameraSpline.validTime()) {
		        buildCamera();
	        }
	        origin = *cameraSpline.getSegmentPoint(segment);


	        idVec3 temp;

	        int numTargets = getTargetSpline()->controlPoints.Num();
	        int count = cameraSpline.splineTime.Num();
	        if (numTargets == 0) {
		        // follow the path
		        if (cameraSpline.getActiveSegment() < count - 1) {
			        temp = *cameraSpline.splinePoints[cameraSpline.getActiveSegment()+1];
		        }
	        } else if (numTargets == 1) {
		        temp = *getTargetSpline()->controlPoints[0];
	        } else {
		        temp = *getTargetSpline()->getSegmentPoint(segment);
	        }

	        temp -= origin;
	        temp.Normalize();
	        direction = temp;
        */
        }

        //------------------------------------------------------------------------------------
         // This method simulates the classic C string function 'strtok' (and 'wcstok').
         // Note that the .NET string 'Split' method cannot be used to simulate 'strtok' since
         // it doesn't allow changing the delimiters between each token retrieval.
         //------------------------------------------------------------------------------------
         private static string activestring;
         private static int activeposition;
         internal static string strtok(string stringtotokenize, string delimiters)
         {
              if (stringtotokenize != null)
              {
               activestring = stringtotokenize;
               activeposition = -1;
              }

              //the stringtotokenize was never set:
              if (activestring == null)
               return null;

              //all tokens have already been extracted:
              if (activeposition == activestring.Length)
               return null;

              //bypass delimiters:
              activeposition++;
              while (activeposition < activestring.Length && delimiters.IndexOf(activestring[activeposition]) > -1)
              {
               activeposition++;
              }

              //only delimiters were left, so return null:
              if (activeposition == activestring.Length)
               return null;

              //get starting position of string to return:
              int startingposition = activeposition;

              //read until next delimiter:
              do
              {
               activeposition++;
              } while (activeposition < activestring.Length && delimiters.IndexOf(activestring[activeposition]) == -1);

              return activestring.Substring(startingposition, activeposition - startingposition);
         }

        
        private void setActiveTargetByName( string name ) {
	        for ( int i = 0; i < targetPositions.Count; i++ ) {
		        if ( name == targetPositions[i].getName() ) {
			        setActiveTarget( i );
			        return;
		        }
	        }
        }

        private void setActiveTarget(int index)
        {
	        activeTarget = index;
        }

        public void setRunning( bool b ) {
	        cameraRunning = b;
        }

        public void setBaseTime(float f)
        {
	        baseTime = f;
        }

        public idCameraPosition getActiveTarget() {
	        if ( targetPositions.Count == 0 ) {
		        addTarget( null, idCameraPosition.positionType.FIXED );
	        }
	        return targetPositions[activeTarget];
        }

        //
        // getCameraInfoLocal
        //
        public bool getCameraInfoLocal( long time, ref idVector3 origin, ref idVector3 direction, ref float fv ) {

	        string buf;

	        if ( ( time - startTime ) / 1000 > totalTime ) {
		        return false;
	        }


	        for ( int i = 0; i < events.Count; i++ ) {
		        if ( time >= startTime + events[i].getTime() && !events[i].getTriggered() ) {
			        events[i].setTriggered( true );
			        if ( events[i].getType() == idCameraEvent.eventType.EVENT_TARGET ) {
				        setActiveTargetByName( events[i].getParam() );
				        getActiveTarget().start( startTime + events[i].getTime() );
				        //Com_Printf("Triggered event switch to target: %s\n",events[i]->getParam());
			        } else if ( events[i].getType() == idCameraEvent.eventType.EVENT_TRIGGER ) {
				        // empty!
			        } else if ( events[i].getType() == idCameraEvent.eventType.EVENT_FOV ) {
                        buf = events[i].getParam();
				        string param1 = strtok( buf, " \t,\0" );
				        string param2 = strtok( null, " \t,\0" );
				        float len = ( param2 != null ) ? float.Parse( param2 ) : 0;
				        float newfov = ( param1 != null ) ? float.Parse( param1 ) : 90;
				        fov.reset( fov.getFOV( time ), newfov, (int)time, len );
				        //*fv = fov = atof(events[i]->getParam());
			        } else if ( events[i].getType() == idCameraEvent.eventType.EVENT_FADEIN ) {
				        float time2 = float.Parse( events[i].getParam() );
				        Engine.Public.Engine.cmdSystem.Cbuf_AddText( "fade 0 0 0 0 " + time2 );
				        Engine.Public.Engine.cmdSystem.Cbuf_Execute();
			        } else if ( events[i].getType() == idCameraEvent.eventType.EVENT_FADEOUT ) {
				        float time2 = float.Parse( events[i].getParam() );
				        Engine.Public.Engine.cmdSystem.Cbuf_AddText( "fade 0 0 0 255 " + time2 );
				        Engine.Public.Engine.cmdSystem.Cbuf_Execute();
			        } else if ( events[i].getType() == idCameraEvent.eventType.EVENT_FADEOUT ) {
				        buf = events[i].getParam();
				        string param1 = strtok( buf, " \t,\0" );
				        string param2 = strtok( null, " \t,\0" );

				        if ( param2 != null ) {
                            idCameraManager.loadCamera(int.Parse(param1), "cameras/" + param2 + ".camera");
					        startCamera( time );
				        } else {
                            idCameraManager.loadCamera(0, "cameras/" + events[i].getParam() + ".camera");
					        startCamera( time );
				        }
				        return true;
			        } else if ( events[i].getType() == idCameraEvent.eventType.EVENT_FADEOUT ) {
				        return false;
			        }
		        }
	        }

	        origin = cameraPosition.getPosition( time );

	       // CHECK_NAN_VEC( origin );

	        fv = fov.getFOV( time );

	        idVector3 temp = origin;

	        int numTargets = targetPositions.Count;
	        if ( numTargets == 0 ) {
		        // empty!
	        } else {
		        temp = getActiveTarget().getPosition( time );
	        }

	        temp -= origin;
	        temp.Normalize();
	        direction = temp;

	        return true;
        }

        public bool waitEvent( int index ) {
	        //for (int i = 0; i < events.Num(); i++) {
	        //	if (events[i]->getSegment() == index && events[i]->getType() == idCameraEvent::EVENT_WAIT) {
	        //		return true;
	        //	}
	        //}
	        return false;
        }


        public const int NUM_CCELERATION_SEGS = 10;
        public const int CELL_AMT = 5;

        private void buildCamera() {
	        int i;
	        //int lastSwitch = 0; // TTimo: unused
	        List<float> waits = new List<float>();
	        List<int> targets = new List<int>();

	        totalTime = baseTime;
	        cameraPosition.setTime( (long)(totalTime * 1000) );
	        // we have a base time layout for the path and the target path
	        // now we need to layer on any wait or speed changes
	        for ( i = 0; i < events.Count; i++ ) {
		        //idCameraEvent *ev = events[i]; // TTimo: unused
		        events[i].setTriggered( false );
		        switch ( events[i].getType() ) {
		        case idCameraEvent.eventType.EVENT_TARGET: {
			        targets.Add( i );
			        break;
		        }
		        case idCameraEvent.eventType.EVENT_FEATHER: {
			        long startTime = 0;
			        float speed = 0;
			        long loopTime = 10;
			        float stepGoal = cameraPosition.getBaseVelocity() / ( 1000 / loopTime );
			        while ( startTime <= 1000 ) {
				        cameraPosition.addVelocity( startTime, loopTime, speed );
				        speed += stepGoal;
				        if ( speed > cameraPosition.getBaseVelocity() ) {
					        speed = cameraPosition.getBaseVelocity();
				        }
				        startTime += loopTime;
			        }

			        // TTimo gcc warns: assignment to `long int' from `float'
			        // more efficient to do (long int)(totalTime) * 1000 - 1000
			        // safer to (long int)(totalTime * 1000 - 1000)
			        startTime = ( long )( totalTime * 1000 - 1000 );
			        long endTime = startTime + 1000;
			        speed = cameraPosition.getBaseVelocity();
			        while ( startTime < endTime ) {
				        speed -= stepGoal;
				        if ( speed < 0 ) {
					        speed = 0;
				        }
				        cameraPosition.addVelocity( startTime, loopTime, speed );
				        startTime += loopTime;
			        }
			        break;

		        }
		        case idCameraEvent.eventType.EVENT_WAIT: {
			        waits.Add( float.Parse( events[i].getParam() ) );

			        //FIXME: this is quite hacky for Wolf E3, accel and decel needs
			        // do be parameter based etc..
			        long startTime = events[i].getTime() - 1000;
			        if ( startTime < 0 ) {
				        startTime = 0;
			        }
			        float speed = cameraPosition.getBaseVelocity();
			        long loopTime = 10;
			        float steps = speed / ( ( events[i].getTime() - startTime ) / loopTime );
			        while ( startTime <= events[i].getTime() - loopTime ) {
				        cameraPosition.addVelocity( startTime, loopTime, speed );
				        speed -= steps;
				        startTime += loopTime;
			        }
			        cameraPosition.addVelocity( events[i].getTime(), (long)float.Parse( events[i].getParam() ) * 1000, 0 );

			        startTime = ( long )( events[i].getTime() + float.Parse( events[i].getParam() ) * 1000 );
			        long endTime = startTime + 1000;
			        speed = 0;
			        while ( startTime <= endTime ) {
				        cameraPosition.addVelocity( startTime, loopTime, speed );
				        speed += steps;
				        startTime += loopTime;
			        }
			        break;
		        }
		        case idCameraEvent.eventType.EVENT_TARGETWAIT: {
			        //targetWaits.Append(i);
			        
		        }
                    break;
		        case idCameraEvent.eventType.EVENT_SPEED: {
        /*
				        // take the average delay between up to the next five segments
				        float adjust = atof(events[i]->getParam());
				        int index = events[i]->getSegment();
				        total = 0;
				        count = 0;

				        // get total amount of time over the remainder of the segment
				        for (j = index; j < cameraSpline.numSegments() - 1; j++) {
					        total += cameraSpline.getSegmentTime(j + 1) - cameraSpline.getSegmentTime(j);
					        count++;
				        }

				        // multiply that by the adjustment
				        double newTotal = total * adjust;
				        // what is the difference..
				        newTotal -= total;
				        totalTime += newTotal / 1000;

				        // per segment difference
				        newTotal /= count;
				        int additive = newTotal;

				        // now propogate that difference out to each segment
				        for (j = index; j < cameraSpline.numSegments(); j++) {
					        cameraSpline.addSegmentTime(j, additive);
					        additive += newTotal;
				        }
				        break;
        */
                    break;
                }
		        default:
			        break;
		        }
	        }


	        for ( i = 0; i < waits.Count; i++ ) {
		        totalTime += waits[i];
	        }

	        // on a new target switch, we need to take time to this point ( since last target switch )
	        // and allocate it across the active target, then reset time to this point
	        long timeSoFar = 0;
	        long total = ( long )( totalTime * 1000 );
	        for ( i = 0; i < targets.Count; i++ ) {
		        long t;
		        if ( i < targets.Count - 1 ) {
			        t = events[targets[i + 1]].getTime();
		        } else {
			        t = total - timeSoFar;
		        }
		        // t is how much time to use for this target
		        setActiveTargetByName( events[targets[i]].getParam() );
		        getActiveTarget().setTime( t );
		        timeSoFar += t;
	        }

            waits.Clear();
            targets.Clear();
        }

        public void startCamera( long t ) {
	        cameraPosition.clearVelocities();
	        cameraPosition.start( t );
	        buildCamera();
	        fov.reset( 90, 90, (int)t, 0 );
	        //for (int i = 0; i < targetPositions.Num(); i++) {
	        //	targetPositions[i]->
	        //}
	        startTime = t;
	        cameraRunning = true;
        }


        private void parse( ref idParser parser  ) 
        {
	        string token;
	        do {
                if(parser.ReachedEndOfBuffer == true)
                    break;

		        token = parser.NextToken.ToLower();

		        if ( token == null || token.Length <= 0 ) {
			        break;
		        }
		        if ( token == "}" ) {
			        break;
		        }

		        if ( token == "time" ) {
			        baseTime = parser.NextFloat;
		        } 
                if ( token == "camera_fixed" )        
                {
			        cameraPosition = new idFixedPosition();
			        cameraPosition.parse( ref parser );
		        } 
                if (  token == "camera_interpolated"  )        
                {
			        cameraPosition = new idInterpolatedPosition();
			        cameraPosition.parse( ref parser );
		        } 
                if (  token == "camera_spline" )        {
			        cameraPosition = new idSplinePosition();
			        cameraPosition.parse( ref parser );
		        } 
                if (  token == "target_fixed" )        {
			        idFixedPosition pos = new idFixedPosition();
			        pos.parse( ref parser );
			        targetPositions.Add( pos );
		        } 
                if (  token == "target_interpolated"  )        {
			        idInterpolatedPosition pos = new idInterpolatedPosition();
			        pos.parse( ref parser );
			        targetPositions.Add( pos );
		        } 
                if (  token == "target_spline"  )        {
			        idSplinePosition pos = new idSplinePosition();
			        pos.parse( ref parser );
			        targetPositions.Add( pos );
		        } 
                if (  token == "fov"  )        {
			        fov.parse( ref parser );
		        } 
                if (  token == "event" )        {
			        idCameraEvent evt = new idCameraEvent();
			        evt.Parse( ref parser );
			        addEvent( evt );
		        }


	        } while ( true );

	      //  if ( !cameraPosition ) {
		  //      Com_Printf( "no camera position specified\n" );
		        // prevent a crash later on
		 //       cameraPosition = new idFixedPosition();
	    //    }

	        parser.UngetToken();
            parser.ExpectNextToken("}");
        }

        public void clear() {
	        currentCameraPosition = 0;
	        cameraRunning = false;
	        lastDirection = idVector3.vector_origin;
	        baseTime = 30;
	        activeTarget = 0;
	        name = "camera01";
	        fov.setFOV( 90 );
	        cameraPosition = null;
	        events.Clear();
	        targetPositions.Clear();
        }

        public bool load( string filename ) {
            idFile f = Engine.Public.Engine.fileSystem.OpenFileRead( filename, true );

            if( f == null )
                return false;

            clear();

            idParser parser = new idParser( f );
            parse(ref parser);
            parser.Dispose();

            Engine.Public.Engine.fileSystem.CloseFile( ref f );

	        return true;
        }

        private int sortEvents( object p1, object p2 ) {
	        idCameraEvent ev1 = ( idCameraEvent )( p1 );
	        idCameraEvent ev2 = ( idCameraEvent )( p2 );

	        if ( ev1.getTime() > ev2.getTime() ) {
		        return -1;
	        }
	        if ( ev1.getTime() < ev2.getTime() ) {
		        return 1;
	        }
	        return 0;
        }

        private void addEvent( idCameraEvent ev ) {
	        events.Add( ev );
	        //events.Sort(&sortEvents);

        }

        private void addEvent( idCameraEvent.eventType t, string param, long time ) {
	        addEvent( new idCameraEvent( t, param, time ) );
	        buildCamera();
        }

        private idCameraPosition newFromType(idCameraPosition.positionType t)
        {
            switch (t)
            {
                case idCameraPosition.positionType.FIXED: return new idFixedPosition();
                case idCameraPosition.positionType.INTERPOLATED: return new idInterpolatedPosition();
                case idCameraPosition.positionType.SPLINE: return new idSplinePosition();
                default:
                    break;
            };
            return null;
        }

        private int numTargets()
        {
            return targetPositions.Count;
        }


        private void addTarget( string name, idCameraPosition.positionType type ) {
	        // TTimo: unused
	        //const char *text = (name == NULL) ? va("target0%d", numTargets()+1) : name;
	        idCameraPosition pos = newFromType( type );
	        if ( pos != null ) {
		        pos.setName( name );
		        targetPositions.Add( pos );
		        activeTarget = numTargets() - 1;
		        if ( activeTarget == 0 ) {
			        // first one
			        addEvent( idCameraEvent.eventType.EVENT_TARGET, name, 0 );
		        }
	        }
        }
    }
}
