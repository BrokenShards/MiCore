////////////////////////////////////////////////////////////////////////////////
// JobManager.cs 
////////////////////////////////////////////////////////////////////////////////
//
// MiCore - Core library for my programs and other libraries.
// Copyright (C) 2021 Michael Furlong <michaeljfurlong@outlook.com>
//
// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for 
// more details.
// 
// You should have received a copy of the GNU General Public License along with
// this program. If not, see <https://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiCore
{
	/// <summary>
	///   Priority based job manager.
	/// </summary>
	public class JobManager : IEnumerable<KeyValuePair<uint, JobList>>
	{
		/// <summary>
		///   Maximum job priority.
		/// </summary>
		public const uint MaxPriority = 10000000;

		/// <summary>
		///   Constructor.
		/// </summary>
		public JobManager()
		{
			m_jobs = new SortedDictionary<uint, JobList>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="sys">
		///   Object to copy.
		/// </param>
		public JobManager( JobManager sys )
		{
			if( sys == null )
				throw new ArgumentNullException();

			m_jobs = new SortedDictionary<uint, JobList>();

			foreach( var v in sys )
				Add( v.Key, v.Value );
		}

		/// <summary>
		///   If the job system contains no jobs.
		/// </summary>
		public bool Empty
		{
			get { return Count != 0; }
		}
		/// <summary>
		///   The amount of registered job priorities.
		/// </summary>
		public int Count
		{
			get { return m_jobs.Count; }
		}

		/// <summary>
		///   If a job is registered to the priority.
		/// </summary>
		/// <param name="priority">
		///   The job priority.
		/// </param>
		/// <returns>
		///   True if the system contains a job with the given priority, otherwise null.
		/// </returns>
		public bool HasJob( uint priority )
		{
			return m_jobs.ContainsKey( priority );
		}
		/// <summary>
		///   Gets the amount of jobs registered to the given priority.
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <returns>
		///   The amount of jobs registered to the priority.
		/// </returns>
		public int JobCount( uint priority )
		{
			if( !HasJob( priority ) )
				return 0;

			return m_jobs[ priority ].Count;
		}

		/// <summary>
		///   Gets the jobs registered to the given priority.
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <returns>
		///   The jobs registered to the given priority.
		/// </returns>
		public JobList Get( uint priority )
		{
			if( priority > MaxPriority )
				return null;

			foreach( var v in this )
			{
				if( v.Key == priority )
					return v.Value;
				if( v.Key > priority )
					break;
			}

			return null;
		}
		/// <summary>
		///   Gets all jobs up to and optionally including the given priority.
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		/// <returns>
		///   All jobs up to (and including if <paramref name="inclusive"/> is true) the given
		///   priority.
		/// </returns>
		public JobList[] GetUpTo( uint priority, bool inclusive = true )
		{
			List<JobList> jobs = new List<JobList>();

			if( priority > MaxPriority )
				priority = MaxPriority;

			foreach( var v in this )
			{
				if( inclusive )
				{
					if( v.Key <= priority )
						jobs.Add( v.Value );
					else if( v.Key > priority )
						break;
				}
				else
				{
					if( v.Key < priority )
						jobs.Add( v.Value );
					else if( v.Key >= priority )
						break;
				}
			}

			return jobs.ToArray();
		}
		/// <summary>
		///   Gets all jobs after and optionally including the given priority.
		/// </summary>
		/// <param name="priority">
		///   The priority to start from.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		/// <returns>
		///   All jobs from (and including if <paramref name="inclusive"/> is true) the given
		///   priority.
		/// </returns>
		public JobList[] GetFrom( uint priority, bool inclusive = true )
		{
			if( priority > MaxPriority )
				return null;

			List<JobList> jobs = new List<JobList>();

			foreach( var v in this )
				if( ( inclusive && v.Key >= priority ) || ( !inclusive && v.Key > priority ) )
					jobs.Add( v.Value );

			return jobs.ToArray();
		}
		/// <summary>
		///   Gets all jobs in priority order.
		/// </summary>
		/// <returns>
		///   All jobs in priority order.
		/// </returns>
		public JobList[] GetAll()
		{
			List<JobList> jobs = new List<JobList>();

			foreach( var v in this )
				jobs.Add( v.Value );

			return jobs.ToArray();
		}

		/// <summary>
		///   Runs all jobs of a given priority on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		/// <param name="priority">
		///   The job priority.
		/// </param>
		public void Run( MiEntity e, uint priority )
		{
			JobList jobs = Get( priority );

			if( jobs == null || jobs.Count == 0 || e == null )
				return;

			for( int i = 0; i < jobs.Count; i++ )
				jobs[ i ].Run( e );
		}
		/// <summary>
		///   Runs all jobs up to a given priority on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		/// <param name="priority">
		///   The job priority.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		public void RunUpTo( MiEntity e, uint priority, bool inclusive = true )
		{
			JobList[] joblist = GetUpTo( priority, inclusive );

			if( joblist == null || joblist.Length == 0 || e == null )
				return;

			for( int i = 0; i < joblist.Length; i++ )
				for( int j = 0; j < joblist[ i ].Count; j++ )
					joblist[ i ][ j ].Run( e );
		}
		/// <summary>
		///   Runs all jobs from a given priority on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		/// <param name="priority">
		///   The job priority.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		public void RunFrom( MiEntity e, uint priority, bool inclusive = true )
		{
			JobList[] joblist = GetFrom( priority, inclusive );

			if( joblist == null || joblist.Length == 0 || e == null )
				return;

			for( int i = 0; i < joblist.Length; i++ )
				for( int j = 0; j < joblist[ i ].Count; j++ )
					joblist[ i ][ j ].Run( e );
		}
		/// <summary>
		///   Runs all jobs on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		public void RunAll( MiEntity e )
		{
			JobList[] joblist = GetAll();

			if( joblist == null || joblist.Length == 0 || e == null )
				return;

			for( int i = 0; i < joblist.Length; i++ )
				for( int j = 0; j < joblist[ i ].Count; j++ )
					joblist[ i ][ j ].Run( e );
		}

		/// <summary>
		///   Asyncronously runs all jobs of a given priority on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		/// <param name="priority">
		///   The job priority.
		/// </param>
		public async Task RunASync( MiEntity e, uint priority )
		{
			JobList jobs = Get( priority );

			if( jobs == null || e == null )
				return;

			List<Task> tasks = new List<Task>( jobs.Count );

			for( int i = 0; i < jobs.Count; i++ )
				tasks.Add( jobs[ i ].RunASync( e ) );

			await Task.WhenAll( tasks ).ConfigureAwait( false );
		}
		/// <summary>
		///   Asyncronously runs all jobs up to a given priority on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		/// <param name="priority">
		///   The job priority.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		public async Task RunUpToASync( MiEntity e, uint priority, bool inclusive = true )
		{
			JobList[] joblist = GetUpTo( priority, inclusive );

			if( joblist == null || joblist.Length == 0 || e == null )
				return;

			for( int i = 0; i < joblist.Length; i++ )
			{
				List<Task> tasks = new List<Task>( joblist[ i ].Count );

				for( int j = 0; j < joblist[ i ].Count; j++ )
					tasks.Add( joblist[ i ][ j ].RunASync( e ) );

				await Task.WhenAll( tasks ).ConfigureAwait( false );
			}
		}
		/// <summary>
		///   Asyncronously runs all jobs from a given priority on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		/// <param name="priority">
		///   The job priority.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		public async Task RunFromASync( MiEntity e, uint priority, bool inclusive = true )
		{
			JobList[] joblist = GetFrom( priority, inclusive );

			if( joblist == null || joblist.Length == 0 || e == null )
				return;

			for( int i = 0; i < joblist.Length; i++ )
			{
				List<Task> tasks = new List<Task>( joblist[ i ].Count );

				for( int j = 0; j < joblist[ i ].Count; j++ )
					tasks.Add( joblist[ i ][ j ].RunASync( e ) );

				await Task.WhenAll( tasks ).ConfigureAwait( false );
			}
		}
		/// <summary>
		///   Asyncronously runs all jobs on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job on.
		/// </param>
		public async Task RunAllASync( MiEntity e )
		{
			JobList[] joblist = GetAll();

			if( joblist == null || joblist.Length == 0 || e == null )
				return;

			for( int i = 0; i < joblist.Length; i++ )
			{
				List<Task> tasks = new List<Task>( joblist[ i ].Count );

				for( int j = 0; j < joblist[ i ].Count; j++ )
					tasks.Add( joblist[ i ][ j ].RunASync( e ) );

				await Task.WhenAll( tasks ).ConfigureAwait( false );
			}
		}

		/// <summary>
		///   Registers the given jobs to the given priority. 
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <param name="jobs">
		///   The jobs to add.
		/// </param>
		/// <returns>
		///   True if priority is within range, jobs is not null, and all jobs were registered
		///   successfully, otherwise false.
		/// </returns>
		public bool Add( uint priority, JobList jobs )
		{
			if( jobs == null || priority > MaxPriority )
				return false;

			if( !m_jobs.ContainsKey( priority ) )
				m_jobs.Add( priority, new JobList( jobs ) );
			else
				m_jobs[ priority ].Add( jobs.Jobs );

			return true;
		}

		/// <summary>
		///   Removes all jobs registered to the given priority.
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <returns>
		///   True if a job was registered to priority and was removed successfully.
		/// </returns>
		public bool Remove( uint priority )
		{
			if( !HasJob( priority ) )
				return false;

			return m_jobs.Remove( priority );
		}
		/// <summary>
		///   Removes all jobs registered up until the given priority.
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		public void RemoveUpTo( uint priority, bool inclusive = true )
		{
			List<uint> ps = new List<uint>();

			if( priority > MaxPriority )
				priority = MaxPriority;

			foreach( var v in this )
			{
				if( inclusive )
				{
					if( v.Key <= priority )
						ps.Add( v.Key );
					else if( v.Key > priority )
						break;
				}
				else
				{
					if( v.Key < priority )
						ps.Add( v.Key );
					else if( v.Key >= priority )
						break;
				}
			}

			foreach( uint i in ps )
				Remove( i );
		}
		/// <summary>
		///   Removes all jobs registered after the given priority.
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <param name="inclusive">
		///   Include priority too?
		/// </param>
		public void RemoveFrom( uint priority, bool inclusive = true )
		{
			if( priority > MaxPriority )
				return;

			List<uint> ps = new List<uint>();

			foreach( var v in this )
				if( ( inclusive && v.Key >= priority ) || ( !inclusive && v.Key > priority ) )
					ps.Add( v.Key );

			for( int i = 0; i < ps.Count; i++ )
				Remove( ps[ i ] );
		}
		/// <summary>
		///   Removes and unregisters the given jobs from the given priority.
		/// </summary>
		/// <param name="priority">
		///   Job priority.
		/// </param>
		/// <param name="jobs">
		///   Jobs to remove.
		/// </param>
		public void Remove( uint priority, params JobDelegate[] jobs )
		{
			if( !HasJob( priority ) )
				return;

			for( int d = 0; d < jobs.Length; d++ )
				for( int j = 0; j < m_jobs[ priority ].Count; j++ )
					m_jobs[ priority ][ j ].Job -= jobs[ d ];
		}
		/// <summary>
		///   Removes all jobs from all priorities.
		/// </summary>
		public void RemoveAll()
		{
			m_jobs.Clear();
		}

		/// <summary>
		///   Gets an enumerator that can be used to iterate through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<uint, JobList>> GetEnumerator()
		{
			return ( (IEnumerable<KeyValuePair<uint, JobList>>)m_jobs ).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable)m_jobs ).GetEnumerator();
		}

		SortedDictionary<uint, JobList> m_jobs;
	}
}
