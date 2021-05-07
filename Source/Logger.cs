////////////////////////////////////////////////////////////////////////////////
// Logger.cs 
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

namespace MiCore
{
	/// <summary>
	///   Possible log message types.
	/// </summary>
	public enum LogType
	{
		/// <summary>
		///   For logging unrecoverable errors.
		/// </summary>
		Error,
		/// <summary>
		///   For logging recoverable warnings.
		/// </summary>
		Warning,
		/// <summary>
		///   For logging debug information.
		/// </summary>
		Debug,
		/// <summary>
		///   For logging standard messages.
		/// </summary>
		Info
	}

	/// <summary>
	///   Handles logging functionality.
	/// </summary>
	public static class Logger
	{
		/// <summary>
		///   The default log file path.
		/// </summary>
		public const string DefaultLogPath = "log.txt";

		/// <summary>
		///   If logs should be displayed in the console.
		/// </summary>
		public static bool LogToConsole
		{
			get; set;
		} = true;
		/// <summary>
		///   If logs should be written to file.
		/// </summary>
		public static bool LogToFile
		{
			get; set;
		} = true;
		/// <summary>
		///   File path to log file.
		/// </summary>
		/// <remarks>
		///   If this does not point to a valid file path and <see cref="LogToFile"/> is true. It will be
		///   set to <see cref="DefaultLogPath"/> on the next call to <see cref="Log(string, LogType)"/>.
		/// </remarks>
		public static string LogPath
		{
			get; set;
		} = DefaultLogPath;

		/// <summary>
		///   If a file exists at <see cref="LogPath"/>.
		/// </summary>
		public static bool LogFileExists
		{
			get { return !string.IsNullOrWhiteSpace( LogPath ) && File.Exists( LogPath ); }
		}

		/// <summary>
		///   Logs a message with the given log type.
		/// </summary>
		/// <param name="msg">
		///   The log message.
		/// </param>
		/// <param name="l">
		///   The log type.
		/// </param>
		public static void Log( string msg, LogType l = LogType.Info )
		{
			if( ( !LogToConsole && !LogToFile ) || string.IsNullOrWhiteSpace( msg ) )
				return;

			#if !DEBUG
				if( l == LogType.Debug )
					return;
			#endif

			if( l != LogType.Info )
				msg = $"{ Enum.GetName( typeof( LogType ), l ).ToUpper() }: { msg }";

			lock( _logsync )
			{
				if( LogToConsole )
					Console.WriteLine( msg );
				if( LogToFile )
				{
					if( string.IsNullOrWhiteSpace( LogPath ) )
						LogPath = DefaultLogPath;

					string datetime = $"{ DateTime.Now.ToLongDateString() } { DateTime.Now.ToLongTimeString() } | ";
				
					try
					{
						File.AppendAllText( LogPath, $"{ datetime } { msg } \r\n" );
					}
					catch
					{
						try
						{
							LogPath = DefaultLogPath;
							File.AppendAllText( LogPath, $"{ datetime } { msg } \r\n" );
						}
						catch
						{
							Console.WriteLine( $"Unable to log message to file: { msg }." );
						}
					}
				}
			}
		}
		/// <summary>
		///   Logs a message to the log stream before returning a value.
		/// </summary>
		/// <typeparam name="T">
		///   The type of value to return.
		/// </typeparam>
		/// <param name="msg">
		///   The log message.
		/// </param>
		/// <param name="val">
		///   The value to return.
		/// </param>
		/// <param name="l">
		///   The log type.
		/// </param>
		/// <returns>
		///   Returns <paramref name="val"/>.
		/// </returns>
		public static T LogReturn<T>( string msg, T val, LogType l = LogType.Info )
		{
			Log( msg, l );
			return val;
		}

		/// <summary>
		///   Deletes the log file.
		/// </summary>
		public static void DeleteLogFile()
		{
			if( !LogFileExists )
				return;

			bool console = LogToConsole;
			LogToConsole = true;

			try
			{
				File.Delete( LogPath );
			}
			catch( ArgumentException )
			{
				Log( "Unable to delete log file: Path contains invalid characters.", LogType.Error );
			}
			catch( NotSupportedException )
			{
				Log( "Unable to delete log file: Path is in an invalid format.", LogType.Error );
			}
			catch( UnauthorizedAccessException )
			{
				Log( "Unable to delete log file: Insufficient permissions or the file is readonly.", LogType.Error );
			}
			catch( PathTooLongException )
			{
				Log( "Unable to delete log file: Path is too long.", LogType.Error );
			}
			catch( DirectoryNotFoundException )
			{
				Log( "Unable to delete log file: Path is invalid (is it on an unmapped drive?).", LogType.Error );
			}
			catch( IOException )
			{
				Log( "Unable to delete log file: The file is in use.", LogType.Error );
			}
			catch( Exception e )
			{
				Log( $"Unable to delete log file: { e.Message }", LogType.Error );
			}

			LogToConsole = console;
		}

		private static readonly object _logsync = new();
	}
}
