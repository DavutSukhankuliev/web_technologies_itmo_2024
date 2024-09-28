using System.Data;
using System.Reflection;

namespace web_technologies_itmo_2024.Services;

public static class DataRowExtensions
{
	public static T DataRowToObject<T>(DataRow row) where T : new()
	{
		T obj = new T();
		foreach (DataColumn column in row.Table.Columns)
		{
			PropertyInfo prop = typeof(T).GetProperty(column.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (prop != null && row[column] != DBNull.Value)
			{
				prop.SetValue(obj, Convert.ChangeType(row[column], prop.PropertyType), null);
			}
		}
		return obj;
	}
}