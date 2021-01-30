////////////////////////////////////////////////////////////////////////////////
// MiJob.cs 
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiCore
{
	/// <summary>
	///   An job delegate.
	/// </summary>
	/// <param name="e">
	///   Entity to run the job on.
	/// </param>
	public delegate void JobDelegate( MiEntity e );

	/// <summary>
	///   An asynchronous job.
	/// </summary>
	public class MiJob
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public MiJob()
		{
			m_job = null;
			RequiredComponents = null;
		}
		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <exception cref="ArgumentNullException">
		///   If job is null.
		/// </exception>
		public MiJob( MiJob job )
		{
			if( job == null )
				throw new ArgumentNullException();

			m_job = new JobDelegate( job.m_job );
			RequiredComponents = new string[ job.RequiredComponents.Length ];

			for( int i = 0; i < job.RequiredComponents.Length; i++ )
				RequiredComponents[ i ] = new string( job.RequiredComponents[ i ].ToCharArray() );
		}
		/// <summary>
		///   Constructor setting the job delegate.
		/// </summary>
		/// <exception cref="ArgumentNullException">
		///   If job is null.
		/// </exception>
		public MiJob( JobDelegate job )
		{
			if( job == null )
				throw new ArgumentNullException();

			m_job = new JobDelegate( job );
			RequiredComponents = null;
		}

		/// <summary>
		///   Job event.
		/// </summary>
		public event JobDelegate Job
		{
			add
			{
				lock( m_lock )
				{
					if( m_job == null )
						m_job = new JobDelegate( value );
					else
						m_job += value;
				}
			}
			remove
			{
				lock( m_lock )
					m_job -= value;
			}
		}

		/// <summary>
		///   Contains the types of components required by the job.
		/// </summary>
		public string[] RequiredComponents
		{
			get; protected set;
		}

		/// <summary>
		///   Checks if the job requires a component with the given component name.
		/// </summary>
		/// <param name="typename">
		///   The name of the component to check.
		/// </param>
		/// <returns>
		///   True if the job requires on the given component type name.
		/// </returns>
		public bool Requires( string typename )
		{
			if( typename != null && RequiredComponents != null )
				foreach( string s in RequiredComponents )
					if( s.Equals( typename ) )
						return true;

			return false;
		}
		/// <summary>
		///   Checks if the job requires a component of the given type.
		/// </summary>
		/// <param name="type">
		///   The component type.
		/// </param>
		/// <returns>
		///   True if the job requires on the given component type.
		/// </returns>
		public bool Requires( Type type )
		{
			if( type == null || RequiredComponents == null )
				return false;

			string name = null;

			foreach( var v in ComponentRegister.Manager )
				if( v.Value.Equals( type ) )
					name = new string( v.Key.ToCharArray() );

			return Requires( name );
		}
		/// <summary>
		///   Checks if the job requires a component of the given type.
		/// </summary>
		/// <typeparam name="T">
		///   The component type.
		/// </typeparam>
		/// <returns>
		///   True if the job requires on the given component type.
		/// </returns>
		public bool Requires<T>() where T : MiComponent, new()
		{
			using( T t = new T() )
				return Requires( t.TypeName );
		}

		/// <summary>
		///   Checks if the entity contains all necessary components needed for the job.
		/// </summary>
		/// <param name="e">
		///   The target entity.
		/// </param>
		/// <returns>
		///   True if the entity contains all necessary components needed for the job, otherwise
		///   false.
		/// </returns>
		public bool ContainsRequired( MiEntity e )
		{
			if( e == null )
				return false;

			if( RequiredComponents != null )
				foreach( string r in RequiredComponents )
					if( !e.HasComponent( r ) )
						return false;

			return true;
		}

		/// <summary>
		///   Adds another jobs' requirements and job delegates to this one.
		/// </summary>
		/// <param name="job">
		///   The job to add.
		/// </param>
		public void Add( MiJob job )
		{
			if( job == null )
				return;

			if( job.RequiredComponents != null && job.RequiredComponents.Length > 0 )
			{
				List<string> req = RequiredComponents == null ? new List<string>() :
																new List<string>( RequiredComponents );
				foreach( string r in job.RequiredComponents )
					if( !req.Contains( r ) )
						req.Add( r );

				RequiredComponents = req.ToArray();
			}

			if( job.m_job != null )
				Job += job.m_job;
		}

		/// <summary>
		///   Runs the job on the entity and its children.
		/// </summary>
		/// <param name="e">
		///   The target entity.
		/// </param>
		public void Run( MiEntity e )
		{
			if( e == null || m_job == null )
				return;

			m_job.Invoke( e );

			if( e.HasChildren )
			{
				List<MiEntity> list = new List<MiEntity>( e.AllChildren );

				foreach( MiEntity me in list )
					if( ContainsRequired( me ) )
						m_job.Invoke( me );
			}
		}
		/// <summary>
		///   Runs the job on the entity and its children asyncronously.
		/// </summary>
		/// <param name="e">
		///   The target entity.
		/// </param>
		public async Task RunASync( MiEntity e )
		{
			if( e == null )
				return;

			List<MiEntity> list = new List<MiEntity>();
			list.Add( e );

			if( e.HasChildren )
				list.AddRange( e.AllChildren );

			List<Task> tasks = new List<Task>();

			foreach( MiEntity c in list )
				if( ContainsRequired( c ) )
					tasks.Add( Task.Run( () => m_job.Invoke( c ) ) );

			await Task.WhenAll( tasks ).ConfigureAwait( false );
		}

		JobDelegate m_job;
		readonly object m_lock = new object();
	}
}
