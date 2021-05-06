////////////////////////////////////////////////////////////////////////////////
// JobList.cs 
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
	///   A list of jobs that share the same priority.
	/// </summary>
	public class JobList : IEnumerable<MiJob>, IEquatable<JobList>
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public JobList()
		{
			m_jobs = new List<MiJob>();
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="list">
		///   The object to copy.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If list is null.
		/// </exception>
		public JobList( JobList list )
		{
			if( list == null )
				throw new ArgumentNullException();

			m_jobs = list.Empty ? new List<MiJob>() : new List<MiJob>( list.Count );

			if( !list.Empty )
				Add( list.m_jobs.ToArray() );
		}
		/// <summary>
		///   Constructs the object with one or multiple jobs.
		/// </summary>
		/// <param name="list">
		///   List of jobs to add.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///   If list is null.
		/// </exception>
		public JobList( params MiJob[] list )
		{
			if( list == null )
				throw new ArgumentNullException();

			m_jobs = list.Length == 0 ? new List<MiJob>() : new List<MiJob>( list.Length );

			if( list.Length > 0 )
				Add( list );
		}

		/// <summary>
		///   Job accessor by index.
		/// </summary>
		/// <param name="index">
		///   Job index.
		/// </param>
		/// <returns>
		///   The job at the given index or null if index is invalid.
		/// </returns>
		public MiJob this[ int index ]
		{
			get { return Get( index ); }
			set { Set( index, value ); }
		}

		/// <summary>
		///   If the list contains no jobs.
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
		/// <summary>
		///   The amount of jobs in the list.
		/// </summary>
		public int Count
		{
			get { return m_jobs.Count; }
		}

		/// <summary>
		///   An array containing all jobs in the list.
		/// </summary>
		public MiJob[] Jobs
		{
			get { return m_jobs.ToArray(); }
		}

		/// <summary>
		///   If the list contains the given job.
		/// </summary>
		/// <param name="job">
		///   The job to check.
		/// </param>
		/// <returns>
		///   True if the job exists within the list.
		/// </returns>
		public bool Contains( MiJob job )
		{
			return job != null && m_jobs.Contains( job );
		}
		/// <summary>
		///   Gets the index of a job contained in the list.
		/// </summary>
		/// <param name="job">
		///   The job to check.
		/// </param>
		/// <returns>
		///   The index of the job if it exists within the list, otherwise -1.
		/// </returns>
		public int IndexOf( MiJob job )
		{
			if( job != null )
				for( int i = 0; i < Count; i++ )
					if( m_jobs[ i ] == job )
						return i;

			return -1;
		}

		/// <summary>
		///   Gets the job at the given index.
		/// </summary>
		/// <param name="index">
		///   Job index.
		/// </param>
		/// <returns>
		///   The job at the given index or null if the index is out of range.
		/// </returns>
		public MiJob Get( int index )
		{
			if( index < 0 || index >= Count )
				return null;

			return m_jobs[ index ];
		}
		/// <summary>
		///   Replaces the job at the index with the given job.
		/// </summary>
		/// <remarks>
		///   If index == Count, <see cref="Add(MiJob[])"/> will be called instead.
		///   If job == null, <see cref="Remove(int)"/> will be called instead.
		/// </remarks>
		/// <param name="index">
		///   The job index.
		/// </param>
		/// <param name="job">
		///   The job to add or null to remove an existing job.
		/// </param>
		/// <returns>
		///   True if index is within range, job is valid or null and was added/removed 
		///   successfully, or if the job already existed at the index, otherwise false.
		/// </returns>
		public bool Set( int index, MiJob job )
		{
			if( index < 0 || index > Count )
				return false;
			if( index == Count )
				return Add( job );
			if( job == null )
				return Remove( index );
			if( Contains( job ) )
				return IndexOf( job ) == index;

			m_jobs[ index ] = job;
			return true;
		}
		/// <summary>
		///   Inserts the given job at the given index. If the job already exists within the list,
		///   it will be removed first.
		/// </summary>
		/// <remarks>
		///   If index == Count, <see cref="Add(MiJob[])"/> is called instead.
		/// </remarks>
		/// <param name="index">
		///   The job index.
		/// </param>
		/// <param name="job">
		///   The job to insert.
		/// </param>
		/// <returns>
		///   True if index is within range, job is valid and was added successfully, otherwise
		///   false.
		/// </returns>
		public bool Insert( int index, MiJob job )
		{
			if( index < 0 || index > Count || job == null )
				return false;
			if( index == Count )
				return Add( job );

			if( Contains( job ) )
				Remove( job );

			m_jobs.Insert( index, job );
			return true;
		}
		/// <summary>
		///   Adds one or multiple jobs to the list.
		/// </summary>
		/// <param name="jobs">
		///   The jobs to add.
		/// </param>
		/// <returns>
		///   True if jobs was valid and all jobs were added successfully.
		/// </returns>
		public bool Add( params MiJob[] jobs )
		{
			if( jobs == null )
				return false;

			for( int i = 0; i < jobs.Length; i++ )
			{
				if( jobs[ i ] == null )
					return false;
				if( !Contains( jobs[ i ] ) )
					m_jobs.Add( jobs[ i ] );
			}

			return true;
		}

		/// <summary>
		///   Removes the job at the given index.
		/// </summary>
		/// <param name="index">
		///   The job index.
		/// </param>
		/// <returns>
		///   True if index was within range and the job was removed successfully.
		/// </returns>
		public bool Remove( int index )
		{
			if( index < 0 || index >= Count )
				return false;

			m_jobs.RemoveAt( index );
			return true;
		}
		/// <summary>
		///   Removes one or multiple jobs.
		/// </summary>
		/// <param name="jobs">
		///   The job(s) to remove.
		/// </param>
		/// <returns>
		///   The amount of jobs that were successfully removed.
		/// </returns>
		public int Remove( params MiJob[] jobs )
		{
			int count = 0;

			if( jobs != null )
				for( int i = 0; i < jobs.Length; i++ )
					if( Remove( IndexOf( jobs[ i ] ) ) )
						count++;

			return count;
		}
		/// <summary>
		///   Removes all jobs from the list.
		/// </summary>
		public void RemoveAll()
		{
			m_jobs.Clear();
		}

		/// <summary>
		///   Runs all jobs in the list on the entity.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job list on.
		/// </param>
		public void Run( MiEntity e )
		{
			if( e == null )
				return;

			for( int i = 0; i < Count; i++ )
				m_jobs[ i ].Run( e );
		}
		/// <summary>
		///   Runs all jobs asyncronously in the list on the entity.
		/// </summary>
		/// <param name="e">
		///   The entity to run the job list on.
		/// </param>
		public async Task RunASync( MiEntity e )
		{
			if( e == null )
				return;

			for( int i = 0; i < Count; i++ )
				await m_jobs[ i ].RunASync( e ).ConfigureAwait( false );
		}

		/// <summary>
		///   Checks if this object is equal to another.
		/// </summary>
		/// <param name="list">
		///   The object to check against.
		/// </param>
		/// <returns>
		///   True if the given object is concidered equal to this object, otherwise false.
		/// </returns>
		public bool Equals( JobList list )
		{
			if( list == null || Count != list.Count )
				return false;

			for( int i = 0; i < Count; i++ )
				if( !m_jobs[ i ].Equals( list.m_jobs[ i ] ) )
					return false;

			return true;
		}

		/// <summary>
		///   Gets an enumerator that can be used to iterate through the collection.
		/// </summary>
		/// <returns>
		///   An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<MiJob> GetEnumerator()
		{
			return ( (IEnumerable<MiJob>)m_jobs ).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable)m_jobs ).GetEnumerator();
		}

		readonly List<MiJob> m_jobs;
	}
}
