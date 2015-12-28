//	DataExtensions: Created 12/27/2015 - Johnny Olsa

using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Data.SqlClient
{
	/// <summary>
	/// Data Extensions
	/// </summary>
	public static class DataExtensions
	{
		/// <summary>
		/// Returns the current SqlConnection in an opened state.
		/// Handy for using statement.
		/// e.g. using (var connection = new SqlConnection(connectionString).Opened())
		/// </summary>
		public static SqlConnection Opened(this IDbConnection connection)
		{
			if (connection.State == ConnectionState.Closed)
				connection.Open();
			return (SqlConnection)connection;
		}
		/// <summary>
		/// Executes the command returning a DataSet.
		/// </summary>
		public static DataSet ToDataSet(this SqlCommand command)
		{
			var dataset = new DataSet();
			using (var da = new SqlDataAdapter(command))
				da.Fill(dataset);
			return dataset;
		}
		/// <summary>
		/// Executes the command returning a DataTable.
		/// </summary>
		public static DataTable ToDataTable(this SqlCommand command)
		{
			var table = new DataTable();
			using (var da = new SqlDataAdapter(command))
				da.Fill(table);
			return table;
		}
		/// <summary>
		/// Executes the command (using SqlDataReader) returning a List of IDataRecord objects.
		/// </summary>
		public static List<IDataRecord> GetRecords(this SqlCommand command)
		{
			bool close = command.Connection.State == ConnectionState.Closed;
			if (close)
				command.Connection.Open();
			var records = command.ExecuteReader().Cast<IDataRecord>().ToList();
			if (close)
				command.Connection.Close();
			return records;
		}
		/// <summary>
		/// Returns the value of the specified column or null.
		/// </summary>
		public static object GetColumn(this IDataRecord row, string columnName)
		{
			return row.GetColumn<object>(columnName);
		}
		/// <summary>
		/// Returns the value of the specified column cast as type T.
		/// default(T) is returned if value is null.
		/// </summary>
		public static T GetColumn<T>(this IDataRecord row, string columnName)
		{
			var value = row[columnName];
			return value == DBNull.Value ? default(T) : (T)value;
		}
		/// <summary>
		/// Returns the value of the specified column or null.
		/// </summary>
		public static object GetColumn(this DataRow row, string columnName)
		{
			return row.GetColumn<object>(columnName);
		}
		/// <summary>
		/// Returns the value of the specified column cast as type T.
		/// default(T) is returned if value is null.
		/// </summary>
		public static T GetColumn<T>(this DataRow row, string columnName)
		{
			var value = row[columnName];
			return value == DBNull.Value ? default(T) : (T)value;
		}
	}
}
