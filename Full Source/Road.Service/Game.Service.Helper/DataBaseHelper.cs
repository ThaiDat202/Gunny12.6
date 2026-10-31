using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Game.Service.Helper;

public class DataBaseHelper
{
	private SqlConnection _SqlConnection;

	private SqlCommand _SqlCommand;

	private SqlDataAdapter _SqlDataAdapter;

	public DataBaseHelper()
	{
		_SqlConnection = new SqlConnection();
	}

	public DataBaseHelper(string Conn_DB)
	{
		_SqlConnection = new SqlConnection(ConfigurationManager.AppSettings[Conn_DB]);
	}

	private static bool OpenConnection(SqlConnection _SqlConnection)
	{
		try
		{
			if (_SqlConnection.State != ConnectionState.Open)
			{
				_SqlConnection.Open();
				return true;
			}
			return true;
		}
		catch (SqlException ex)
		{
			bool flag = false;
			throw new Exception(ex.ToString());
		}
	}

	public bool ExecuteNonQuery(string sqlstr)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandText = sqlstr;
			_SqlCommand.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			throw new Exception(ex.ToString());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return true;
	}

	public bool ExecuteReader(string sqlstr, ref SqlDataReader sqlDataReader)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.Text;
			_SqlCommand.CommandText = sqlstr;
			sqlDataReader = _SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			return true;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.ToString());
		}
		finally
		{
			Dispose(disposing: true);
		}
	}

	public DataSet GetDateSet(string sqlstr)
	{
		if (!OpenConnection(_SqlConnection))
		{
		}
		try
		{
			_SqlDataAdapter = new SqlDataAdapter(sqlstr, _SqlConnection);
			DataSet dataSet = new DataSet();
			_SqlDataAdapter.Fill(dataSet);
			return dataSet;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.ToString());
		}
		finally
		{
			_SqlConnection.Close();
			_SqlConnection.Dispose();
		}
	}

	private void Dispose(bool disposing)
	{
		if (!disposing || _SqlDataAdapter == null)
		{
			return;
		}
		if (_SqlDataAdapter.SelectCommand != null)
		{
			if (_SqlCommand.Connection != null)
			{
				_SqlDataAdapter.SelectCommand.Connection.Dispose();
			}
			_SqlDataAdapter.SelectCommand.Dispose();
		}
		_SqlDataAdapter.Dispose();
		_SqlDataAdapter = null;
	}
}
