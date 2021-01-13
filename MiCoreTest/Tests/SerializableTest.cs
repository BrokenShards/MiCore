////////////////////////////////////////////////////////////////////////////////
// SerializableTest.cs 
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

namespace MiCore.Test
{
	public static class SerializableTest
	{
		// Class that supports serialization to and from text files.
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

			// Here we override `LoadFromStream` to load our class data from a text stream.
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

			// Here we can override `SaveToStream` to save our class data to a text stream.
			// If it is not overriden, it would look like this, where `ToString` is written to
			// the stream.
			public override bool SaveToStream( StreamWriter sw )
			{
				if( sw == null )
					return false;

				try
				{
					sw.Write( ToString() );
				}
				catch
				{
					return false;
				}

				return true;
			}

			// We override `ToString` with the data to be written to the stream when
			// `SaveToStream` is called.
			public override string ToString()
			{
				return TextData;
			}

			public void Dispose()
			{
				Logger.Log( "TextSerial disposed." );
			}
		}
		// Class that supports serialization to and from binary files.
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

			// Here we override `LoadFromStream` to load our class data from a binary stream.
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
			// Here we override `SaveToStream` to save our class data to a binary stream.
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

		public class TextSerializableTest : TestModule
		{
			// Text serialization path.
			const string TextSerialPath = "text.txt";

			protected override bool OnTest()
			{
				Logger.Log( "Running TextSerializable Test..." );

				TextSerial text1 = new TextSerial( "Tester Object" );

				// Try saving text1 to file using its `SaveToStream` method.
				if( !TextSerializable.ToFile( text1, TextSerialPath, true ) )
					return Logger.LogReturn( "Failed! Unable to serialize to file.", false );

				// Try loading previously saved object from file using its `SaveToStream` method.
				TextSerial text2 = TextSerializable.FromFile<TextSerial>( TextSerialPath );

				if( text2 == null )
					return Logger.LogReturn( "Failed! Unable to deserialize from file.", false );

				// Comparing saved and loaded data.
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
		public class BinarySerializableTest : TestModule
		{
			// Binary serialization path.
			const string BinarySerialPath = "binary.bin";

			protected override bool OnTest()
			{
				Logger.Log( "Running BinarySerializable Test..." );

				BinarySerial bin1 = new BinarySerial( 10 );

				// Try saving text1 to file. `ToFile<T>` is a convenience method that opens a
				// file stream and calls `SaveToStream` on the object.
				if( !BinarySerializable.ToFile( bin1, BinarySerialPath, true ) )
					return Logger.LogReturn( "Failed! Unable to serialize to file.", false );

				// Try loading previously saved object from file. `FromFile<T>` is a convenience
				// method that opens a file stream and calls `LoadFromStream` on the object.
				BinarySerial bin2 = BinarySerializable.FromFile<BinarySerial>( BinarySerialPath );

				if( bin2 == null )
					return Logger.LogReturn( "Failed! Unable to deserialize from file.", false );

				// Comparing saved and loaded data.
				if( bin1.NumberData != bin2.NumberData ||
					!bin1.TextData.Equals( bin2.TextData ) )
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
		public class TextSerializableDBTest : TestModule
		{
			// Database class that supports serialization to and from text files.
			class TextDB : DisposableTextDatabase<TextSerial>
			{
				public TextDB()
				:	base()
				{ }

				// Override with the file path that the database is intended to be saved to/
				// loaded from.
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

				// Adding elements to the database.
				for( int i = 0; i < 10; i++ )
					if( !db1.Add( i.ToString(), new TextSerial( i.ToString() ) ) )
						return Logger.LogReturn( "Failed! Unable to add object to DB.", false );

				// Trying to save database to file.
				if( !db1.SaveToFile() )
					return Logger.LogReturn( "Failed! Unable to save DB to file.", false );

				TextDB db2 = new TextDB();

				// Trying to load previously saved database from file.
				if( !db2.LoadFromFile() )
					return Logger.LogReturn( "Failed! Unable to load DB from file.", false );

				// Checking for equality.
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
		public class BinarySerializableDBTest : TestModule
		{
			// Database class that supports serialization to and from binary files.
			class BinaryDB : DisposableBinaryDatabase<BinarySerial>
			{
				public BinaryDB()
				:	base()
				{ }

				// Override with the file path that the database is intended to be saved to/
				// loaded from.
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

				// Adding elements to the database.
				for( int i = 0; i < 10; i++ )
					if( !db1.Add( i.ToString(), new BinarySerial( i, i.ToString() ) ) )
						return Logger.LogReturn( "Failed! Unable to add object to DB.", false );

				// Trying to save database to file.
				if( !db1.SaveToFile() )
					return Logger.LogReturn( "Failed! Unable to save DB to file.", false );

				BinaryDB db2 = new BinaryDB();

				// Trying to load previously saved database from file.
				if( !db2.LoadFromFile() )
					return Logger.LogReturn( "Failed! Unable to load DB from file.", false );

				// Checking for equality.
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

		public static bool Run()
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
}
