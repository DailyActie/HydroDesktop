﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;

namespace HydroDesktop.ImportExport
{
	/// <summary>
	/// Helper class for CSV file parsing
	/// </summary>
    public class CsvFileParser
	{
		#region Private Members

		/// <summary>
		/// Creates a name for a column that has not been used for any existing columns in the given data table
		/// </summary>
		/// <param name="dataTable">The data table for which a unique column name is to be created</param>
		/// <returns>A unique column name for the data table</returns>
		private static string GetUniqueColumnName ( DataTable dataTable )
		{
			return GetUniqueColumnName ( dataTable, "Column" );
		}

		/// <summary>
		/// Creates a name for a column that has not been used for any existing columns in the given data table
		/// </summary>
		/// <param name="dataTable">The data table for which a unique column name is to be created</param>
		/// <param name="baseColumnName">The base column name to start with when creating a unique column name. A number will be appended to the base column name until a unique column name is found.</param>
		/// <returns>A unique column name for the data table</returns>
		private static string GetUniqueColumnName ( DataTable dataTable, string baseColumnName )
		{
			// Check the input column name
			if ( baseColumnName == null || baseColumnName == String.Empty )
			{
				baseColumnName = "Column";
			}

			if ( dataTable.Columns.Contains ( baseColumnName ) == false )
			{
				return baseColumnName;
			}

			// Add a number to the column name until we find a unique name
			int counter = 1;
			while ( true )
			{
				string columnName = baseColumnName + counter++;
				if ( dataTable.Columns.Contains ( columnName ) == false )
				{
					return columnName;
				}
			}
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Counts the number of lines in a file 
		/// </summary>
		/// <param name="fileName">The full path to and name of the file to count lines in</param>
		/// <returns>The number of lines in the file</returns>
		public static long CountLinesInFile ( string fileName )
		{
			long lineCount = 0;
			using ( StreamReader reader = new StreamReader ( fileName ) )
			{
				string line;
				while ( (line = reader.ReadLine ()) != null )
				{
					lineCount++;
				}
			}
			return lineCount;
		}

		/// <summary>
		/// Parses a comma separated file into a DataTable
		/// </summary>
		/// <param name="fileToParse">The full path to and name of the CSV file to parse</param>
		/// <param name="hasHeaders">True if the file has column headers; false otherwise</param>
		/// <returns>DataTable of the parsed data</returns>
		public static DataTable ParseFileToDataTable ( string fileToParse, bool hasHeaders )
		{
			return ParseFileToDataTable ( fileToParse, hasHeaders, null, null );
		}

		/// <summary>
		/// Parses a comma separated file into a DataTable
		/// </summary>
		/// <param name="fileToParse">The full path to and name of the CSV file to parse</param>
		/// <param name="hasHeaders">True if the file has column headers; false otherwise</param>
		/// <param name="bgWorker">BackgroundWorker (may be null), in order to show progress</param>
		/// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
		/// <returns>DataTable of the parsed data</returns>
		public static DataTable ParseFileToDataTable ( string fileToParse, bool hasHeaders, BackgroundWorker bgWorker, DoWorkEventArgs e )
		{
			DataTable dataTable = new DataTable ();

			// Get the number of lines in the file
			long totalSteps = 0;
			long currentStep = 0;
			int percentComplete = 0;
			int previousPercentComplete = 0;

			if ( e != null && bgWorker != null )
			{
				// Check for cancel
				if ( bgWorker.CancellationPending )
				{
					e.Cancel = true;
					return dataTable;
				}

				// Report progress
				if ( bgWorker.WorkerReportsProgress == true )
				{
					bgWorker.ReportProgress ( 0, "Opening file..." );
					totalSteps = CountLinesInFile ( fileToParse );
				}

			}

			using ( TextReader stream = new StreamReader ( fileToParse ) )
			{
				CsvStreamReader csv = new CsvStreamReader ( stream );
				string[] line = csv.ReadLine ();
				if ( line == null )
				{
					return dataTable;
				}

				// Get the column headers
				if ( hasHeaders == true )
				{
					// Check for cancel
					if ( e != null && bgWorker != null )
					{
						if ( bgWorker.CancellationPending )
						{
							e.Cancel = true;
							return dataTable;
						}

						// Report progress
						if ( bgWorker.WorkerReportsProgress == true )
						{
							currentStep++;
							percentComplete = (int)(100 * currentStep / totalSteps);
							if ( percentComplete > previousPercentComplete )
							{
								bgWorker.ReportProgress ( percentComplete, "Reading data header..." );
								previousPercentComplete = percentComplete;
							}
						}
					}

					foreach ( string part in line )
					{
						// Get a unique column header
						string columnHeader = part;

						if ( columnHeader == null || columnHeader == String.Empty )
						{
							columnHeader = GetUniqueColumnName ( dataTable );
						}
						else if ( dataTable.Columns.Contains ( columnHeader ) == true )
						{
							columnHeader = GetUniqueColumnName ( dataTable, columnHeader );
						}

						// Add the column to the table
						dataTable.Columns.Add ( columnHeader, typeof ( string ) );
					}

					line = csv.ReadLine ();
				}

				// Parse the rest of the file
				while ( line != null )
				{
					// Check for cancel
					if ( e != null && bgWorker != null )
					{
						if ( bgWorker.CancellationPending )
						{
							e.Cancel = true;
							return dataTable;
						}

						// Report progress
						if ( bgWorker.WorkerReportsProgress == true )
						{
							currentStep++;
							percentComplete = (int)(100 * currentStep / totalSteps);
							if ( percentComplete > previousPercentComplete )
							{
								bgWorker.ReportProgress ( percentComplete, "Reading line " + currentStep + " of " + totalSteps + "..." );
								previousPercentComplete = percentComplete;
							}
						}
					}

					// Add columns if the current line has more columns than the data table
					while ( line.Length > dataTable.Columns.Count )
					{
						dataTable.Columns.Add ( GetUniqueColumnName ( dataTable ), typeof ( string ) );
					}

					// Add the row to the data table
					dataTable.Rows.Add ( line );
					line = csv.ReadLine ();
				}

			}

			// Report progress
			if ( e != null && bgWorker != null )
			{
				if ( bgWorker.CancellationPending )
				{
					e.Cancel = true;
					return dataTable;
				}

				if ( bgWorker.WorkerReportsProgress == true )
				{
					bgWorker.ReportProgress ( 100, "All lines read from file" );
				}
			}

			return dataTable;
		}

		#endregion
	}
}
