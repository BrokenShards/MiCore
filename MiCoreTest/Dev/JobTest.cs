using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiCore.Test
{
	public class JobTest : TestModule
	{
		static int runcount = 0;
		static readonly object _lock = new();

		// Test function for the job to run.
		static void TestDelegate( MiEntity _ )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}

		protected override bool OnTest()
		{
			Logger.Log( "Running Job tests..." );

			runcount = 0;

			// Create a new job by assigning it a delegate.
			MiJob job = new( TestDelegate );
			MiEntity ent = new( "Tester" );

			for( int i = 0; i < 50; i++ )
				if( !ent.AddChild( new MiEntity( "L" + i.ToString() ) ) )
					return Logger.LogReturn( "Failed! Unable to add child.", false, LogType.Error );

			// Run the job on the entity.
			job.Run( ent );

			int totalruns = ent.ChildCount + 1;

			// Ensuring job ran successfully.
			if( runcount != totalruns )
				return Logger.LogReturn( $"Failed! Job missed { totalruns - runcount } runs.", false, LogType.Error );

			runcount = 0;

			// Run the job asyncronously on the entity. We call `Wait` here to wait for the task
			// to finish so it can be called syncronously.
			Task.WaitAll( job.RunASync( ent ) );

			// Ensuring job ran successfully.
			if( runcount != totalruns )
				return Logger.LogReturn( $"Failed! ASync Job missed { totalruns - runcount } runs.", false, LogType.Error );

			return Logger.LogReturn( "Success!", true );
		}
	}
	class JobListTest : TestModule
	{
		static int runcount = 0;
		static readonly object _lock = new();

		// Test functions for the job list to run.
		static void TestDelegate1( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate2( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate3( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate4( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate5( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}

		protected override bool OnTest()
		{
			Logger.Log( "Running JobList tests..." );

			runcount = 0;

			// Create a new job list by assigning it jobs.
			JobList list = new()
			{
				new MiJob( TestDelegate1 ),
				new MiJob( TestDelegate2 ),
				new MiJob( TestDelegate3 )
			};

			// Try adding more jobs to the list.
			if( !list.Add( new MiJob( TestDelegate4 ), new MiJob( TestDelegate5 ) ) )
				return Logger.LogReturn( "Failed! Unable to add jobs to list.", false, LogType.Error );

			MiEntity ent = new( "Tester" );

			for( int i = 0; i < 20; i++ )
				if( !ent.AddChild( new MiEntity( "L" + i.ToString() ) ) )
					return Logger.LogReturn( "Failed! Unable to add child.", false, LogType.Error );

			// Run the job list on the entity.
			list.Run( ent );

			int totalruns = ( ent.ChildCount + 1 ) * list.Count;

			// Ensuring job ran successfully.
			if( runcount != totalruns )
				return Logger.LogReturn( $"Failed! JobList missed { totalruns - runcount } runs.", false, LogType.Error );

			runcount = 0;

			// Run the job list asyncronously on the entity. We call `Wait` here to wait for the
			// task to finish so it can be called syncronously.
			Task.WaitAll( list.RunASync( ent ) );

			// Ensuring job ran successfully.
			if( runcount != totalruns )
				return Logger.LogReturn( $"Failed! ASync JobList missed { totalruns - runcount } runs.", false, LogType.Error );

			return Logger.LogReturn( "Success!", true );
		}
	}
	class JobManagerTest : TestModule
	{
		static int runcount = 0;
		static readonly object _lock = new();

		// Test functions for the job list to run.
		static void TestDelegate1( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate2( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate3( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate4( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}
		static void TestDelegate5( MiEntity e )
		{
			lock( _lock )
			{
				Thread.Sleep( 100 );
				runcount++;
			}
		}

		protected override bool OnTest()
		{
			Logger.Log( "Running JobManager tests..." );

			runcount = 0;

			// Create a new job manager.
			JobManager man = new();

			// Try adding job lists to the manager with given priorities.
			if( !man.Add( 10, new JobList( new MiJob( TestDelegate1 ) ) ) ||
				!man.Add( 20, new JobList( new MiJob( TestDelegate2 ) ) ) ||
				!man.Add( 30, new JobList( new MiJob( TestDelegate3 ) ) ) ||
				!man.Add( 40, new JobList( new MiJob( TestDelegate4 ) ) ) ||
				!man.Add( 50, new JobList( new MiJob( TestDelegate5 ) ) ) )
				return Logger.LogReturn( "Failed! Unable to add job lists to manager.", false, LogType.Error );

			// Ensuring the right amount of job lists were added.
			if( man.Count is not 5 )
				return Logger.LogReturn( "Failed! Job manager reporting wrong job count.", false, LogType.Error );

			// Ensuring the jobs were added at the right priority.
			if( !man.HasJob( 10 ) || !man.HasJob( 20 ) || !man.HasJob( 30 ) ||
				!man.HasJob( 40 ) || !man.HasJob( 50 ) )
				return Logger.LogReturn( "Failed! Job manager reporting no jobs for assigned priorities.", false, LogType.Error );

			// Remove a job list mapped to a given priority.
			if( !man.Remove( 40 ) )
				return Logger.LogReturn( "Failed! Job manager reporting unable to remove assigned priorty.", false, LogType.Error );

			// Ensuring the job list was actually removed.
			if( man.Count is not 4 )
				return Logger.LogReturn( "Failed! Job manager reporting wrong job count.", false, LogType.Error );

			MiEntity ent = new( "Tester" );

			for( int i = 0; i < 20; i++ )
				if( !ent.AddChild( new MiEntity( "L" + i.ToString() ) ) )
					return Logger.LogReturn( "Failed! Unable to add child.", false, LogType.Error );

			// Run all jobs in the manager in priority order on the entity.
			man.RunAll( ent );

			int totalruns = ( ent.ChildCount + 1 ) * man.Count;

			// Ensuring job ran successfully.
			if( runcount != totalruns )
				return Logger.LogReturn( $"Failed! JobManager missed { totalruns - runcount } runs.", false, LogType.Error );

			runcount = 0;

			// Run all jobs in the manager in priority order asyncronously on the entity. We 
			// call `Wait` here to wait for the task to finish so it can be called syncronously.
			Task.WaitAll( man.RunAllASync( ent ) );

			// Ensuring job ran successfully.
			if( runcount != totalruns )
				return Logger.LogReturn( $"Failed! ASync JobManager missed { totalruns - runcount } runs.", false, LogType.Error );

			return Logger.LogReturn( "Success!", true );
		}
	}
}
