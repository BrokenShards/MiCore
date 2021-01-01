////////////////////////////////////////////////////////////////////////////////
// Test.cs 
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
using System.Xml;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace MiCore.Test
{
	static class Tests
	{
		class IDTest : TestModule
		{
			// Valid string ID.
			const string ValidID = "valid_id_23";
			// Invalid string ID.
			const string InvalidID = " in-valid-ID";

			protected override bool OnTest()
			{
				Logger.Log( "Running Identifiable Test..." );

				bool result = true;

				// Ensure valid ID is not reported as invalid.
				if( !Identifiable.IsValid( ValidID ) )
					result = Logger.LogReturn( "Failed! Valid ID reported as invalid.", false );

				// Ensure invalid ID is not reported as valid.
				if( Identifiable.IsValid( InvalidID ) )
					result = Logger.LogReturn( "Failed! Invalid ID reported as valid.", false );

				// Ensure validated ID is not reported as invalid.
				if( !Identifiable.IsValid( Identifiable.AsValid( InvalidID ) ) )
					result = Logger.LogReturn( "Failed! Validated ID reported as invalid.", false );

				// Ensure random ID is not reported as invalid.
				if( !Identifiable.IsValid( Identifiable.RandomStringID( 8 ) ) )
					result = Logger.LogReturn( "Failed! Random ID reported as invalid.", false );

				// Ensure new ID is not reported as invalid.
				if( !Identifiable.IsValid( Identifiable.NewStringID() ) )
					result = Logger.LogReturn( "Failed! New ID reported as invalid.", false );

				return Logger.LogReturn( result ? "Identifiable test succeeded!." : "Identifiable test failed!.", result );
			}
		}
		class NameTest : TestModule
		{
			// Valid string name.
			const string ValidName = "Mr. Valid Name";
			// Invalid string name.
			const string InvalidName = "\t Mr. Invalid Name\n";

			protected override bool OnTest()
			{
				Logger.Log( "Running Naming Test..." );

				bool result = true;

				// Ensure valid name is not reported as invalid.
				if( !Naming.IsValid( ValidName ) )
					result = Logger.LogReturn( "Failed! Valid name reported as invalid.", false );

				// Ensure invalid name is not reported as valid.
				if( Naming.IsValid( InvalidName ) )
					result = Logger.LogReturn( "Failed! Invalid name reported as valid.", false );

				// Ensure validated name is not reported as invalid.
				if( !Naming.IsValid( Naming.AsValid( InvalidName ) ) )
					result = Logger.LogReturn( "Failed! Validated name reported as invalid.", false );

				// Ensure random name is not reported as invalid.
				if( !Naming.IsValid( Naming.RandomName( 8 ) ) )
					result = Logger.LogReturn( "Failed! Random name reported as invalid.", false );

				// Ensure new name is not reported as invalid.
				if( !Naming.IsValid( Naming.NewName() ) )
					result = Logger.LogReturn( "Failed! Random name reported as invalid.", false );

				return Logger.LogReturn( result ? "Naming test succeeded!." : "Naming test failed!.", result );
			}
		}

		static class SerializableTests
		{
			/// <summary>
			///   Class that supports serialization to and from text files.
			/// </summary>
			class TextSerial : TextSerializable, IDisposable
			{
				public TextSerial()
				{
					TextData = "New text data.";
				}
				public TextSerial( string data )
				{
					TextData = string.IsNullOrWhiteSpace( data ) ? "New text data." : data;
				}

				public string TextData
				{
					get; set;
				}

				public override bool LoadFromStream( StreamReader sr )
				{
					if( sr == null )
						return false;

					try
					{
						TextData = sr.ReadLine();
					}
					catch
					{
						return false;
					}

					return true;
				}
				public override bool SaveToStream( StreamWriter sw )
				{
					if( sw == null )
						return false;

					try
					{
						sw.Write( TextData );
					}
					catch
					{
						return false;
					}

					return true;
				}

				public void Dispose()
				{
					Logger.Log( "TextSerial disposed." );
				}
			}
			/// <summary>
			///   Class that supports serialization to and from binary files.
			/// </summary>
			class BinarySerial : BinarySerializable, IDisposable
			{
				public BinarySerial()
				{
					NumberData = 10;
					TextData = "New text data.";
				}
				public BinarySerial( float num, string data = null )
				{
					NumberData = num;
					TextData = string.IsNullOrWhiteSpace( data ) ? "New text data." : data;
				}

				public float NumberData
				{
					get; set;
				}
				public string TextData
				{
					get; set;
				}

				public override bool LoadFromStream( BinaryReader sr )
				{
					if( sr == null )
						return false;

					try
					{
						NumberData = sr.ReadSingle();
						TextData = sr.ReadString();
					}
					catch
					{
						return false;
					}

					return true;
				}
				public override bool SaveToStream( BinaryWriter sw )
				{
					if( sw == null )
						return false;

					try
					{
						sw.Write( NumberData );
						sw.Write( TextData );
					}
					catch
					{
						return false;
					}

					return true;
				}

				public void Dispose()
				{
					Logger.Log( "BinarySerial disposed." );
				}
			}

			class TextSerializableTest : TestModule
			{
				/// <summary>
				///   Text serialization path.
				/// </summary>
				const string TextSerialPath = "text.txt";

				protected override bool OnTest()
				{
					Logger.Log( "Running TextSerializable Test..." );

					TextSerial text1 = new TextSerial( "Tester Object" );

					if( !TextSerializable.ToFile( text1, TextSerialPath, true ) )
						return Logger.LogReturn( "Failed! Unable to serialize to file.", false );

					TextSerial text2 = TextSerializable.FromFile<TextSerial>( TextSerialPath );

					if( text2 == null )
						return Logger.LogReturn( "Failed! Unable to deserialize from file.", false );

					if( text1.TextData != text2.TextData )
						return Logger.LogReturn( "Failed! Deserialized object contains different values.", false );

					text1.Dispose();
					text2.Dispose();

					try
					{
						File.Delete( TextSerialPath );
					}
					catch
					{
						Logger.Log( "Failed! Unable to delete " + TextSerialPath + ".", LogType.Warning );
					}

					return Logger.LogReturn( "TextSerializable test succeeded!", true );
				}
			}
			class BinarySerializableTest : TestModule
			{
				/// <summary>
				///   Binary serialization path.
				/// </summary>
				const string BinarySerialPath = "binary.bin";

				protected override bool OnTest()
				{
					Logger.Log( "Running BinarySerializable Test..." );

					BinarySerial bin1 = new BinarySerial( 10 );

					if( !BinarySerializable.ToFile( bin1, BinarySerialPath, true ) )
						return Logger.LogReturn( "Failed! Unable to serialize to file.", false );

					BinarySerial bin2 = BinarySerializable.FromFile<BinarySerial>( BinarySerialPath );

					if( bin2 == null )
						return Logger.LogReturn( "Failed! Unable to deserialize from file.", false );

					if( bin1.NumberData != bin2.NumberData )
						return Logger.LogReturn( "Failed! Deserialized object contains different values.", false );

					bin1.Dispose();
					bin2.Dispose();

					try
					{
						File.Delete( BinarySerialPath );
					}
					catch
					{
						Logger.Log( "Failed! Unable to delete " + BinarySerialPath + ".", LogType.Warning );
					}

					return Logger.LogReturn( "BinarySerializable test succeeded!", true );
				}
			}
			class TextSerializableDBTest : TestModule
			{
				/// <summary>
				///   Database class that supports serialization to and from text files.
				/// </summary>
				class TextDB : DisposableTextDatabase<TextSerial>
				{
					public TextDB()
					: base()
					{ }

					public override string FilePath
					{
						get
						{
							return "test_text.db";
						}
					}
				}
				protected override bool OnTest()
				{
					Logger.Log( "Running TextDatabase Test..." );

					TextDB db1 = new TextDB();

					for( int i = 0; i < 10; i++ )
						if( !db1.Add( i.ToString(), new TextSerial( i.ToString() ) ) )
							return Logger.LogReturn( "Failed! Unable to add object to DB.", false );

					if( !db1.SaveToFile() )
						return Logger.LogReturn( "Failed! Unable to save DB to file.", false );

					TextDB db2 = new TextDB();

					if( !db2.LoadFromFile() )
						return Logger.LogReturn( "Failed! Unable to load DB from file.", false );

					if( db1.Count != db2.Count )
						return Logger.LogReturn( "Failed! Loaded DB has a different size.", false );

					foreach( string s in db1.Keys )
						if( db2[ s ] == null || db2[ s ].TextData != db1[ s ].TextData )
							return Logger.LogReturn( "Failed! Loaded DB has different data.", false );

					try
					{
						File.Delete( db1.FilePath );
					}
					catch
					{
						Logger.Log( "Failed! Unable to delete " + db1.FilePath + ".", LogType.Warning );
					}

					db1?.Dispose();
					db2?.Dispose();

					return Logger.LogReturn( "TextDatabase test succeeded!", true );
				}
			}
			class BinarySerializableDBTest : TestModule
			{
				/// <summary>
				///   Database class that supports serialization to and from binary files.
				/// </summary>
				class BinaryDB : DisposableBinaryDatabase<BinarySerial>
				{
					public BinaryDB()
					: base()
					{ }

					public override string FilePath
					{
						get
						{
							return "test_binary.db";
						}
					}
				}
				protected override bool OnTest()
				{
					Logger.Log( "Running BinaryDatabase Test..." );

					BinaryDB db1 = new BinaryDB();

					for( int i = 0; i < 10; i++ )
						if( !db1.Add( i.ToString(), new BinarySerial( i, i.ToString() ) ) )
							return Logger.LogReturn( "Failed! Unable to add object to DB.", false );

					if( !db1.SaveToFile() )
						return Logger.LogReturn( "Failed! Unable to save DB to file.", false );

					BinaryDB db2 = new BinaryDB();

					if( !db2.LoadFromFile() )
						return Logger.LogReturn( "Failed! Unable to load DB from file.", false );

					if( db1.Count != db2.Count )
						return Logger.LogReturn( "Failed! Loaded DB has a different size.", false );

					foreach( string s in db1.Keys )
						if( db2[ s ] == null || db2[ s ].NumberData != db1[ s ].NumberData )
							return Logger.LogReturn( "Failed! Loaded DB has different data.", false );

					try
					{
						File.Delete( db1.FilePath );
					}
					catch
					{
						Logger.Log( "Failed! Unable to delete " + db1.FilePath + ".", LogType.Warning );
					}

					db1?.Dispose();
					db2?.Dispose();

					return Logger.LogReturn( "BinaryDatabase test succeeded!", true );
				}
			}

			public static bool Run( string[] args )
			{
				bool result = true;

				if( !Testing.Test<TextSerializableTest>() )
					result = false;
				if( !Testing.Test<BinarySerializableTest>() )
					result = false;
				if( !Testing.Test<TextSerializableDBTest>() )
					result = false;
				if( !Testing.Test<BinarySerializableDBTest>() )
					result = false;

				return result;
			}
		}
		static class ECSTests
		{
			/// <summary>
			///   Component containing a width and height.
			/// </summary>
			class SizeComponent : Component
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

				public override string TypeName
				{
					// This is used by the component register for component type identification.
					// This should always be `nameof( [ComponentType] )`.
					get { return nameof( SizeComponent ); }
				}

				public override bool LoadFromStream( BinaryReader sr )
				{
					if( !base.LoadFromStream( sr ) )
						return false;

					try
					{
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
				public override bool SaveToStream( BinaryWriter sw )
				{
					if( !base.SaveToStream( sw ) )
						return false;

					try
					{
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
				public override bool LoadFromXml( XmlElement element )
				{
					if( !base.LoadFromXml( element ) )
						return false;

					Width  = 1.0f;
					Height = 1.0f;

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

				// ToString should return the xml representation of the component as it is used
				// to save the component to xml.
				public override string ToString()
				{
					StringBuilder sb = new StringBuilder();

					sb.Append( '<' );
					sb.Append( nameof( SizeComponent ) );
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

				public override void Dispose()
				{ }

				float m_width,
					  m_height;
			}
			/// <summary>
			///   Component containing a scale. 
			/// </summary>
			class ScaleComponent : Component
			{
				public ScaleComponent()
				:	base()
				{
					// Here we add the names of any components that are required by this component.
					// When adding this component to an Entity, these required components will also
					// be added if needed.
					RequiredComponents = new string[] { nameof( SizeComponent ) };
					Scale = 1.0f;
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
						if( Parent == null || !Parent.Contains<SizeComponent>() )
							return 0.0f;

						return Parent.Get<SizeComponent>().Width * Scale;
					}
				}
				public float Height
				{
					get
					{
						if( Parent == null || !Parent.Contains<SizeComponent>() )
							return 0.0f;

						return Parent.Get<SizeComponent>().Height * Scale;
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

				public override void Dispose()
				{ }

				float m_scale;
			}

			class ComponentTest : TestModule
			{
				protected override bool OnTest()
				{
					Logger.Log( "Running Component tests..." );

					if( !ComponentRegister.Manager.Register<SizeComponent>() ||
						!ComponentRegister.Manager.Register<ScaleComponent>() )
						return Logger.LogReturn( "Failed: Unable to register components.", false, LogType.Error );

					SizeComponent size = new SizeComponent( 100, 100 );
					ScaleComponent scale = new ScaleComponent( 2.5f );

					if( !size.Enabled || !scale.Enabled )
						return Logger.LogReturn( "Failed: Newly constructed component is in disabled state.", false, LogType.Error );
					if( !size.Visible || !scale.Visible )
						return Logger.LogReturn( "Failed: Newly constructed component is in invisible state.", false, LogType.Error );

					if( size.Width != 100 )
						return Logger.LogReturn( "Failed: SizeComponent Width was not set correctly.", false, LogType.Error );
					if( size.Height != 100 )
						return Logger.LogReturn( "Failed: SizeComponent Height was not set correctly.", false, LogType.Error );

					return Logger.LogReturn( "Success!", true );
				}
			}
			class ComponentRegisterTest : TestModule
			{
				protected override bool OnTest()
				{
					Logger.Log( "Running ComponentRegister tests..." );

					if( !ComponentRegister.Manager.Register<SizeComponent>() )
						return Logger.LogReturn( "Failed: Unable to add SizeComponent.", false, LogType.Error );
					if( !ComponentRegister.Manager.Registered<SizeComponent>() )
						return Logger.LogReturn( "Failed: SizeComponent registered successfully but is reported as not registered.", false, LogType.Error );

					if( !ComponentRegister.Manager.Register<ScaleComponent>() )
						return Logger.LogReturn( "Failed: Unable to add ScaleComponent.", false, LogType.Error );
					if( !ComponentRegister.Manager.Registered<ScaleComponent>() )
						return Logger.LogReturn( "Failed: ScaleComponent registered successfully but is reported as not registered.", false, LogType.Error );

					return Logger.LogReturn( "Success!", true );
				}
			}
			class EntityTest : TestModule
			{
				protected override bool OnTest()
				{
					Logger.Log( "Running Entity tests..." );

					if( !ComponentRegister.Manager.Register<SizeComponent>() ||
						!ComponentRegister.Manager.Register<ScaleComponent>() )
						return Logger.LogReturn( "Failed: Unable to register components.", false, LogType.Error );

					using( Entity entity = new Entity( "TestEntity" ) )
					{
						if( !entity.Enabled )
							return Logger.LogReturn( "Failed: New entity is in disabled state.", false, LogType.Error );
						if( !entity.Visible )
							return Logger.LogReturn( "Failed: New entity is in invisible state.", false, LogType.Error );

						if( !entity.Add<ScaleComponent>() )
							return Logger.LogReturn( "Failed: Unable to add ScaleComponent.", false, LogType.Error );
						if( !entity.Contains<ScaleComponent>() )
							return Logger.LogReturn( "Failed: Succeeded adding ScaleComponent yet it is not contained by the entity.", false, LogType.Error );
						if( !entity.Contains<SizeComponent>() )
							return Logger.LogReturn( "Failed: Required SizeComponent was not added with ScaleComponent.", false, LogType.Error );

						SizeComponent size = entity.Get<SizeComponent>();
						ScaleComponent scale = entity.Get<ScaleComponent>();

						if( scale.Parent != entity )
							return Logger.LogReturn( "Failed: Entity is not parent of directly added component.", false, LogType.Error );
						if( size.Parent != entity )
							return Logger.LogReturn( "Failed: Entity is not parent of indirectly added component.", false, LogType.Error );

						if( !scale.Enabled )
							return Logger.LogReturn( "Failed: Added component is in disabled state.", false, LogType.Error );
						if( !scale.Visible )
							return Logger.LogReturn( "Failed: Added component is in invisible state.", false, LogType.Error );

						if( !size.Enabled )
							return Logger.LogReturn( "Failed: Newly added required component is in disabled state.", false, LogType.Error );
						if( !size.Visible )
							return Logger.LogReturn( "Failed: Newly added required component is in invisible state.", false, LogType.Error );

						size.Width = 150.0f;
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

			public static bool Run( string[] args )
			{
				bool result = true;

				if( !Testing.Test<ComponentRegisterTest>() )
					result = false;
				if( !Testing.Test<ComponentTest>() )
					result = false;
				if( !Testing.Test<EntityTest>() )
					result = false;

				return result;
			}
		}

		class XmlTest : TestModule
		{
			protected override bool OnTest()
			{
				Logger.Log( "Running Xml Test..." );

				bool result = true;

				if( !Vec2fTest() )
					result = false;
				if( !Vec2iTest() )
					result = false;
				if( !Vec2uTest() )
					result = false;
				if( !IRectTest() )
					result = false;
				if( !FRectTest() )
					result = false;
				if( !ColorTest() )
					result = false;
				if( !VideoModeTest() )
					result = false;

				return Logger.LogReturn( result ? "Xml test succeeded!." : "Xml test failed!.", result );
			}

			bool Vec2fTest()
			{
				Logger.Log( "Running Vector2f Test..." );

				Vector2f vec = new Vector2f( 13, 25 );
				XmlDocument doc = new XmlDocument();

				try
				{
					doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vec ) );

					if( !vec.Equals( Xml.ToVec2f( doc.DocumentElement ) ) )
						return Logger.LogReturn( "Failed! Loaded Vector2f has incorrect values.", false );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed! " + e.Message, false );
				}

				return Logger.LogReturn( "Success!", true );
			}
			bool Vec2iTest()
			{
				Logger.Log( "Running Vector2i Test..." );

				Vector2i vec = new Vector2i( 13, 25 );
				XmlDocument doc = new XmlDocument();

				try
				{
					doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vec ) );

					if( !vec.Equals( Xml.ToVec2i( doc.DocumentElement ) ) )
						return Logger.LogReturn( "Failed! Loaded Vector2i has incorrect values.", false );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed! " + e.Message, false );
				}

				return Logger.LogReturn( "Success!", true );
			}
			bool Vec2uTest()
			{
				Logger.Log( "Running Vector2u Test..." );

				Vector2u vec = new Vector2u( 13, 25 );
				XmlDocument doc = new XmlDocument();

				try
				{
					doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vec ) );

					if( !vec.Equals( Xml.ToVec2u( doc.DocumentElement ) ) )
						return Logger.LogReturn( "Failed! Loaded Vector2u has incorrect values.", false );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed! " + e.Message, false );
				}

				return Logger.LogReturn( "Success!", true );
			}

			bool IRectTest()
			{
				Logger.Log( "Running IntRect Test..." );

				IntRect rect = new IntRect( 13, 25, 45, 56 );
				XmlDocument doc = new XmlDocument();

				try
				{
					doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( rect ) );

					if( !rect.Equals( Xml.ToIRect( doc.DocumentElement ) ) )
						return Logger.LogReturn( "Failed! Loaded IntRect has incorrect values.", false );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed! " + e.Message, false );
				}

				return Logger.LogReturn( "Success!", true );
			}
			bool FRectTest()
			{
				Logger.Log( "Running FloatRect Test..." );

				FloatRect rect = new FloatRect( 13, 25, 45, 56 );
				XmlDocument doc = new XmlDocument();

				try
				{
					doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( rect ) );

					if( !rect.Equals( Xml.ToFRect( doc.DocumentElement ) ) )
						return Logger.LogReturn( "Failed! Loaded FloatRect has incorrect values.", false );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed! " + e.Message, false );
				}

				return Logger.LogReturn( "Success!", true );
			}

			bool ColorTest()
			{
				Logger.Log( "Running Color Test..." );

				Color col = new Color( 13, 25, 45, 56 );
				XmlDocument doc = new XmlDocument();

				try
				{
					doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( col ) );

					if( !col.Equals( Xml.ToColor( doc.DocumentElement ) ) )
						return Logger.LogReturn( "Failed! Loaded Color has incorrect values.", false );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed! " + e.Message, false );
				}

				return Logger.LogReturn( "Success!", true );
			}
			bool VideoModeTest()
			{
				Logger.Log( "Running VideoMode Test..." );

				VideoMode vm = new VideoMode( 800, 600, 32 );
				XmlDocument doc = new XmlDocument();

				try
				{
					doc.LoadXml( Xml.Header + "\r\n" + Xml.ToString( vm ) );

					if( !vm.Equals( Xml.ToVideoMode( doc.DocumentElement ) ) )
						return Logger.LogReturn( "Failed! Loaded VideoMode has incorrect values.", false );
				}
				catch( Exception e )
				{
					return Logger.LogReturn( "Failed! " + e.Message, false );
				}

				return Logger.LogReturn( "Success!", true );
			}
		}

		static void Main( string[] args )
		{
			Logger.LogToFile = true;
			Logger.Log( "Running MiCore Tests..." );

			bool result = true;

			if( !Testing.Test<IDTest>() )
				result = false;
			if( !Testing.Test<NameTest>() )
				result = false;
			if( !SerializableTests.Run( args ) )
				result = false;
			if( !ECSTests.Run( args ) )
				result = false;
			if( !Testing.Test<XmlTest>() )
				result = false;

			Logger.Log( result ? "All MiCore tests completed successfully!" : "One or more MiCore tests failed!" );

			Logger.Log( "Press enter to exit." );
			Console.ReadLine();
		}
	}
}
