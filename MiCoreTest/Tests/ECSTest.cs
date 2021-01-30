////////////////////////////////////////////////////////////////////////////////
// ECSTest.cs 
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
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MiCore.Test
{
	public static class ECSTest
	{
		// Component containing a width and height.
		class SizeComponent : MiComponent
		{
			public SizeComponent()
			:	base()
			{
				Width  = 1.0f;
				Height = 1.0f;
			}
			public SizeComponent( float w, float h )
			:	base()
			{
				Width  = w;
				Height = h;
			}
			public SizeComponent( SizeComponent c )
			:	base( c )
			{
				Width  = c?.Width  ?? 1.0f;
				Height = c?.Height ?? 1.0f;
			}

			public float Width
			{
				get { return m_width; }
				set { m_width = value <= 0 ? 1.0f : value; }
			}
			public float Height
			{
				get { return m_height; }
				set { m_height = value <= 0 ? 1.0f : value; }
			}

			// `TypeName` is used by `ComponentRegister` for component type identification. This
			// should always be `nameof( <ComponentType> )`.
			public override string TypeName
			{
				get { return nameof( SizeComponent ); }
			}

			// Load object from binary stream. See `Tests/SerializableTest.cs` for more info.
			public override bool LoadFromStream( BinaryReader sr )
			{
				// Call the base implementation to load inherited data.
				if( !base.LoadFromStream( sr ) )
					return false;

				try
				{
					// Read data from stream.
					Width  = sr.ReadSingle();
					Height = sr.ReadSingle();
				}
				catch( Exception e )
				{
					Console.WriteLine( "Unable to load SizeComponent from stream: " + e.Message );
					return false;
				}

				return true;
			}
			// Save object to binary stream. See `Tests/SerializableTest.cs` for more info.
			public override bool SaveToStream( BinaryWriter sw )
			{
				// Call the base implementation to save inherited data.
				if( !base.SaveToStream( sw ) )
					return false;

				try
				{
					// Write data to stream.
					sw.Write( Width );
					sw.Write( Height );
				}
				catch( Exception e )
				{
					Console.WriteLine( "Unable to save SizeComponent to stream: " + e.Message );
					return false;
				}

				return true;
			}
			// Load object from an xml element.
			public override bool LoadFromXml( XmlElement element )
			{
				// Call the base implementation to load inherited data.
				if( !base.LoadFromXml( element ) )
					return false;

				// Reset data.
				Width  = 1.0f;
				Height = 1.0f;

				// Check for needed attributes.
				if( !element.HasAttribute( nameof( Width ) ) )
				{
					Console.WriteLine( "Unable to load SizeComponent: No Width attribute." );
					return false;
				}
				if( !element.HasAttribute( nameof( Height ) ) )
				{
					Console.WriteLine( "Unable to load SizeComponent: No Height attribute." );
					return false;
				}

				// Try parsing attributes and assigning data.
				if( !float.TryParse( element.GetAttribute( nameof( Width ) ), out float w ) )
				{
					Console.WriteLine( "Unable to load SizeComponent: Failed parsing Width attribute." );
					return false;
				}
				if( !float.TryParse( element.GetAttribute( nameof( Height ) ), out float h ) )
				{
					Console.WriteLine( "Unable to load SizeComponent: Failed parsing Height attribute." );
					return false;
				}

				Width  = w;
				Height = h;
				return true;
			}

			// `ToString` should return the xml element string representation of the component as it
			// is used to save the component to xml.
			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				sb.Append( '<' );
				sb.Append( TypeName );
				sb.Append( " " );
				sb.Append( nameof( Enabled ) );
				sb.Append( "=\"" );
				sb.Append( Enabled );
				sb.AppendLine( "\"" );

				sb.Append( "                " );
				sb.Append( nameof( Visible ) );
				sb.Append( "=\"" );
				sb.Append( Visible );
				sb.AppendLine( "\"" );

				sb.Append( "                " );
				sb.Append( nameof( Width ) );
				sb.Append( "=\"" );
				sb.Append( Width );
				sb.AppendLine( "\"" );

				sb.Append( "                " );
				sb.Append( nameof( Height ) );
				sb.Append( "=\"" );
				sb.Append( Height );
				sb.Append( "\"/>" );

				return sb.ToString();
			}

			// `Clone` must be implemented as it is used to make deep coppies of components of
			// initially unknown types.
			public override object Clone()
			{
				return new SizeComponent( this );
			}

			float m_width,
				  m_height;
		}
		// Component containing a scale.
		class ScaleComponent : MiComponent
		{
			public ScaleComponent()
			:	base()
			{
				Scale = 1.0f;

				// Here we add the names of any components that are required by this component to
				// `RequiredComponents`. When adding this component to an entity, these required
				// components will also be added if needed.
				RequiredComponents = new string[] { nameof( SizeComponent ) };

				// The opposite of `RequiredComponents` is `IncompatibleComponents`, any components
				// named in it cannot be added to an entity containing this component.
			}
			public ScaleComponent( float scl )
			:	base()
			{
				RequiredComponents = new string[] { nameof( SizeComponent ) };
				Scale = scl;
			}
			public ScaleComponent( ScaleComponent c )
			:	base( c )
			{
				RequiredComponents = new string[] { nameof( SizeComponent ) };
				Scale = c?.Scale ?? 1.0f;
			}

			public float Scale
			{
				get { return m_scale; }
				set { m_scale = value < 0 ? 0.0f : value; }
			}

			public float Width
			{
				get
				{
					if( Parent == null || !Parent.HasComponent<SizeComponent>() )
						return 0.0f;

					// Here we use `Stack` to access the parent stack and call `Get<T>` to get
					// the `SizeComponent` in the same component stack. Because `RequiredComponents`
					// includes `nameof( SizeComponent )`, it is safe to assume the stack will
					// contain a component of that type and we do not need to check for it.
					return Parent.GetComponent<SizeComponent>().Width * Scale;
				}
			}
			public float Height
			{
				get
				{
					if( Parent == null || !Parent.HasComponent<SizeComponent>() )
						return 0.0f;

					return Parent.GetComponent<SizeComponent>().Height * Scale;
				}
			}

			public override string TypeName
			{
				get { return nameof( ScaleComponent ); }
			}

			public override bool LoadFromStream( BinaryReader sr )
			{
				if( !base.LoadFromStream( sr ) )
					return false;

				try
				{
					Scale = sr.ReadSingle();
				}
				catch( Exception e )
				{
					Console.WriteLine( "Unable to load ScaleComponent from stream: " + e.Message );
					return false;
				}

				return true;
			}
			public override bool SaveToStream( BinaryWriter sw )
			{
				if( !base.SaveToStream( sw ) )
					return false;

				try
				{
					sw.Write( Scale );
				}
				catch( Exception e )
				{
					Console.WriteLine( "Unable to save ScaleComponent to stream: " + e.Message );
					return false;
				}

				return true;
			}
			public override bool LoadFromXml( XmlElement element )
			{
				if( !base.LoadFromXml( element ) )
					return false;

				Scale = 1.0f;

				if( !element.HasAttribute( nameof( Scale ) ) )
				{
					Console.WriteLine( "Unable to load ScaleComponent: No Scale attribute." );
					return false;
				}
				if( !float.TryParse( element.GetAttribute( nameof( Scale ) ), out float s ) )
				{
					Console.WriteLine( "Unable to load ScaleComponent: Failed parsing Scale attribute." );
					return false;
				}

				Scale = s;
				return true;
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				sb.Append( '<' );
				sb.Append( nameof( ScaleComponent ) );
				sb.Append( " " );
				sb.Append( nameof( Enabled ) );
				sb.Append( "=\"" );
				sb.Append( Enabled );
				sb.AppendLine( "\"" );

				sb.Append( "                " );
				sb.Append( nameof( Visible ) );
				sb.Append( "=\"" );
				sb.Append( Visible );
				sb.AppendLine( "\"" );

				sb.Append( "                " );
				sb.Append( nameof( Scale ) );
				sb.Append( "=\"" );
				sb.Append( Scale );
				sb.Append( "\"/>" );

				return sb.ToString();
			}

			public override object Clone()
			{
				return new ScaleComponent( this );
			}

			float m_scale;
		}

		class ComponentTest : TestModule
		{
			protected override bool OnTest()
			{
				Logger.Log( "Running Component tests..." );

				// Before using any components, they MUST be registered first. This should be done
				// at the beginning of your program before your main loop.
				if( !ComponentRegister.Manager.Register<SizeComponent>() ||
					!ComponentRegister.Manager.Register<ScaleComponent>() )
					return Logger.LogReturn( "Failed: Unable to register components.", false, LogType.Error );

				SizeComponent  size = new SizeComponent( 100, 100 );
				ScaleComponent scale = new ScaleComponent( 2.5f );

				// Ensuring newly created components are enabled.
				if( !size.Enabled || !scale.Enabled )
					return Logger.LogReturn( "Failed: Newly constructed component is in disabled state.", false, LogType.Error );
				// Ensuring newly created components are visible.
				if( !size.Visible || !scale.Visible )
					return Logger.LogReturn( "Failed: Newly constructed component is in invisible state.", false, LogType.Error );

				// Ensuring constructor set width properly.
				if( size.Width != 100 )
					return Logger.LogReturn( "Failed: SizeComponent Width was not set correctly.", false, LogType.Error );
				// Ensuring constructor set height properly.
				if( size.Height != 100 )
					return Logger.LogReturn( "Failed: SizeComponent Height was not set correctly.", false, LogType.Error );
				// Ensuring constructor set scale properly.
				if( scale.Scale != 2.5f )
					return Logger.LogReturn( "Failed: ScaleComponent Scale was not set correctly.", false, LogType.Error );

				scale.Dispose();
				size.Dispose();

				return Logger.LogReturn( "Success!", true );
			}
		}
		class ComponentRegisterTest : TestModule
		{
			protected override bool OnTest()
			{
				Logger.Log( "Running ComponentRegister tests..." );

				// Here we attempt to register our custom components to the register.
				if( !ComponentRegister.Manager.Register<SizeComponent>() )
					return Logger.LogReturn( "Failed: Unable to add SizeComponent.", false, LogType.Error );
				if( !ComponentRegister.Manager.Register<ScaleComponent>() )
					return Logger.LogReturn( "Failed: Unable to add ScaleComponent.", false, LogType.Error );

				// Here we check that our components did actually register properly.
				if( !ComponentRegister.Manager.Registered<SizeComponent>() )
					return Logger.LogReturn( "Failed: SizeComponent registered successfully but is reported as not registered.", false, LogType.Error );
				if( !ComponentRegister.Manager.Registered<ScaleComponent>() )
					return Logger.LogReturn( "Failed: ScaleComponent registered successfully but is reported as not registered.", false, LogType.Error );

				return Logger.LogReturn( "Success!", true );
			}
		}
		class MiEntityTest : TestModule
		{
			protected override bool OnTest()
			{
				Logger.Log( "Running MiEntity tests..." );

				if( !ComponentRegister.Manager.Register<SizeComponent>() ||
					!ComponentRegister.Manager.Register<ScaleComponent>() )
					return Logger.LogReturn( "Failed: Unable to register components.", false, LogType.Error );

				using( MiEntity ent = new MiEntity() )
				{
					if( !ent.Enabled )
						return Logger.LogReturn( "Failed: New MiEntity is in disabled state.", false, LogType.Error );
					if( !ent.Visible )
						return Logger.LogReturn( "Failed: New MiEntity is in invisible state.", false, LogType.Error );

					// Trying to add a new component of a given type. This will add any components
					// named in `RequiredComponents` too. This will fail if the entity already
					// contains a component named in `IncompatibleComponents`.
					if( !ent.AddNewComponent<ScaleComponent>() )
						return Logger.LogReturn( "Failed: Unable to add ScaleComponent.", false, LogType.Error );

					// Checking if the entity contains a component of a given type.
					if( !ent.HasComponent<ScaleComponent>() )
						return Logger.LogReturn( "Failed: Succeeded adding ScaleComponent yet it is not contained by the entity.", false, LogType.Error );
					// Checking if the entity contains a component of a given type name.
					if( !ent.HasComponent( nameof( SizeComponent ) ) )
						return Logger.LogReturn( "Failed: Required SizeComponent was not added with ScaleComponent.", false, LogType.Error );

					// Accessing components of different types from the entity.
					SizeComponent  size  = ent.GetComponent<SizeComponent>();
					ScaleComponent scale = ent.GetComponent<ScaleComponent>();

					// Ensuring the component parent is set properly.
					if( scale.Parent != ent )
						return Logger.LogReturn( "Failed: MiEntity is not parent of directly added component.", false, LogType.Error );
					if( size.Parent != ent )
						return Logger.LogReturn( "Failed: MiEntity is not parent of indirectly added component.", false, LogType.Error );

					// Manipulating component data.
					size.Width  = 150.0f;
					size.Height = 200.0f;
					scale.Scale = 2.0f;

					if( scale.Width != size.Width * scale.Scale )
						return Logger.LogReturn( "Failed: ScaleComponent is scaling Width incorrectly.", false, LogType.Error );
					if( scale.Height != size.Height * scale.Scale )
						return Logger.LogReturn( "Failed: ScaleComponent is scaling Height incorrectly.", false, LogType.Error );
				}

				return Logger.LogReturn( "Success!", true );
			}
		}
		class MiNodeTest : TestModule
		{
			// Here, our test node implements `MiNode<TestNode>` so that it contains a reference
			// to a parent `TestNode` object and a list of children `TestNode` objects. This is so
			// it can be used as a node in a tree-based structure.
			class TestNode : MiNode<TestNode>
			{
				public TestNode()
				:	base()
				{ }
				public TestNode( TestNode t )
				:	base( t )
				{ }
				public TestNode( string id, string name = null )
				:	base( id, name )
				{ }

				public override object Clone()
				{
					return new TestNode( this );
				}
			}

			protected override bool OnTest()
			{
				Logger.Log( "Running MiNode tests..." );

				using( TestNode node = new TestNode( "root", "Root Node" ) )
				{
					int count = 25;

					// Iterating through adding children to the root node.
					for( int i = 0; i < count; i++ )
						if( !node.AddChild( new TestNode( "child" + i.ToString(), "Child " + i.ToString() ), true ) )
							return Logger.LogReturn( "Failed: Unable to add valid children.", false, LogType.Error );

					// Ensuring the right amount of children were added.
					if( node.ChildCount != count )
						return Logger.LogReturn( "Failed: Children added successfully but ChildCount is wrong.", false, LogType.Error );
				}

				return Logger.LogReturn( "Success!", true );
			}
		}

		class JobTest : TestModule
		{
			static int runcount = 0;
			static readonly object _lock = new object();

			// Test function for the job to run.
			static void TestDelegate( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}

			protected override bool OnTest()
			{
				Logger.Log( "Running Job tests..." );

				runcount = 0;

				// Create a new job by assigning it a delegate.
				MiJob job = new MiJob( TestDelegate );
				MiEntity ent = new MiEntity( "Tester", "Test Entity" );

				for( int i = 0; i < 500; i++ )
					if( !ent.AddChild( new MiEntity( "L" + i.ToString(), "Level" + i.ToString() ) ) )
						return Logger.LogReturn( "Failed! Unable to add child.", false, LogType.Error );

				// Run the job on the entity.
				job.Run( ent );

				int totalruns = ent.ChildCount + 1;

				// Ensuring job ran successfully.
				if( runcount != totalruns )
					return Logger.LogReturn( "Failed! Job missed " + ( totalruns - runcount ).ToString() + " runs.", false, LogType.Error );

				runcount = 0;

				// Run the job asyncronously on the entity. We call `Wait` here to wait for the task
				// to finish so it can be called syncronously.
				job.RunASync( ent ).Wait();

				// Ensuring job ran successfully.
				if( runcount != totalruns )
					return Logger.LogReturn( "Failed! ASync job missed " + ( totalruns - runcount ).ToString() + " runs.", false, LogType.Error );

				return Logger.LogReturn( "Success!", true );
			}
		}
		class JobListTest : TestModule
		{
			static int runcount = 0;
			static readonly object _lock = new object();

			// Test functions for the job list to run.
			static void TestDelegate1( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate2( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate3( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate4( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate5( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}

			protected override bool OnTest()
			{
				Logger.Log( "Running JobList tests..." );

				runcount = 0;

				// Create a new job list by assigning it jobs.
				JobList list = new JobList
				{
					new MiJob( TestDelegate1 ),
					new MiJob( TestDelegate2 ),
					new MiJob( TestDelegate3 )
				};

				// Try adding more jobs to the list.
				if( !list.Add( new MiJob( TestDelegate4 ), new MiJob( TestDelegate5 ) ) )
					return Logger.LogReturn( "Failed! Unable to add jobs to list.", false, LogType.Error );

				MiEntity ent = new MiEntity( "Tester", "Test Entity" );

				for( int i = 0; i < 200; i++ )
					if( !ent.AddChild( new MiEntity( "L" + i.ToString(), "Level" + i.ToString() ) ) )
						return Logger.LogReturn( "Failed! Unable to add child.", false, LogType.Error );

				// Run the job list on the entity.
				list.Run( ent );

				int totalruns = ( ent.ChildCount + 1 ) * list.Count;

				// Ensuring job ran successfully.
				if( runcount != totalruns )
					return Logger.LogReturn( "Failed! JobList missed " + ( totalruns - runcount ).ToString() + " runs.", false, LogType.Error );

				runcount = 0;

				// Run the job list asyncronously on the entity. We call `Wait` here to wait for the
				// task to finish so it can be called syncronously.
				list.RunASync( ent ).Wait();

				// Ensuring job ran successfully.
				if( runcount != totalruns )
					return Logger.LogReturn( "Failed! ASync JobList missed " + ( totalruns - runcount ).ToString() + " runs.", false, LogType.Error );

				return Logger.LogReturn( "Success!", true );
			}
		}
		class JobManagerTest : TestModule
		{
			static int runcount = 0;
			static readonly object _lock = new object();

			// Test functions for the job list to run.
			static void TestDelegate1( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate2( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate3( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate4( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}
			static void TestDelegate5( MiEntity e )
			{
				lock( _lock )
					runcount++;
			}

			protected override bool OnTest()
			{
				Logger.Log( "Running JobManager tests..." );

				runcount = 0;

				// Create a new job manager.
				JobManager man = new JobManager();

				// Try adding job lists to the manager with given priorities.
				if( !man.Add( 10, new JobList( new MiJob( TestDelegate1 ) ) ) ||
					!man.Add( 20, new JobList( new MiJob( TestDelegate2 ) ) ) ||
					!man.Add( 30, new JobList( new MiJob( TestDelegate3 ) ) ) ||
					!man.Add( 40, new JobList( new MiJob( TestDelegate4 ) ) ) ||
					!man.Add( 50, new JobList( new MiJob( TestDelegate5 ) ) ) )
					return Logger.LogReturn( "Failed! Unable to add job lists to manager.", false, LogType.Error );

				// Ensuring the right amount of job lists were added.
				if( man.Count != 5 )
					return Logger.LogReturn( "Failed! Job manager reporting wrong job count.", false, LogType.Error );

				// Ensuring the jobs were added at the right priority.
				if( !man.HasJob( 10 ) || !man.HasJob( 20 ) || !man.HasJob( 30 ) ||
					!man.HasJob( 40 ) || !man.HasJob( 50 ) )
					return Logger.LogReturn( "Failed! Job manager reporting no jobs for assigned priorities.", false, LogType.Error );

				// Remove a job list mapped to a given priority.
				if( !man.Remove( 40 ) )
					return Logger.LogReturn( "Failed! Job manager reporting unable to remove assigned priorty.", false, LogType.Error );

				// Ensuring the job list was actually removed.
				if( man.Count != 4 )
					return Logger.LogReturn( "Failed! Job manager reporting wrong job count.", false, LogType.Error );

				MiEntity ent = new MiEntity( "Tester", "Test Entity" );

				for( int i = 0; i < 200; i++ )
					if( !ent.AddChild( new MiEntity( "L" + i.ToString(), "Level" + i.ToString() ) ) )
						return Logger.LogReturn( "Failed! Unable to add child.", false, LogType.Error );

				// Run all jobs in the manager in priority order on the entity.
				man.RunAll( ent );

				int totalruns = ( ent.ChildCount + 1 ) * man.Count;

				// Ensuring job ran successfully.
				if( runcount != totalruns )
					return Logger.LogReturn( "Failed! JobManager missed " + ( totalruns - runcount ).ToString() + " runs.", false, LogType.Error );

				runcount = 0;

				// Run all jobs in the manager in priority order asyncronously on the entity. We 
				// call `Wait` here to wait for the task to finish so it can be called syncronously.
				man.RunAllASync( ent ).Wait();

				// Ensuring job ran successfully.
				if( runcount != totalruns )
					return Logger.LogReturn( "Failed! ASync JobManager missed " + ( totalruns - runcount ).ToString() + " runs.", false, LogType.Error );

				return Logger.LogReturn( "Success!", true );
			}
		}

		public static bool Run()
		{
			bool result = true;

			if( !Testing.Test<ComponentRegisterTest>() )
				result = false;
			if( !Testing.Test<ComponentTest>() )
				result = false;
			if( !Testing.Test<MiEntityTest>() )
				result = false;
			if( !Testing.Test<MiNodeTest>() )
				result = false;
			if( !Testing.Test<JobTest>() )
				result = false;
			if( !Testing.Test<JobListTest>() )
				result = false;
			if( !Testing.Test<JobManagerTest>() )
				result = false;

			return result;
		}
	}
}
