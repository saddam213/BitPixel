using System;
using System.Collections.Generic;
using BitPixel.Base.Extensions;
using BitPixel.Datatables.Reflection;

namespace BitPixel.Datatables
{
	static class TypeFilters
	{
		private static readonly Func<string, Type, object> ParseValue =
			(input, t) => t.IsEnum ? Enum.Parse(t, input) : Convert.ChangeType(input, t);

		internal static string FilterMethod(string q, List<object> parametersForLinqQuery, Type type)
		{
			Func<string, string, string> makeClause = (method, query) =>
			{
				parametersForLinqQuery.Add(ParseValue(query, type));
				var indexOfParameter = parametersForLinqQuery.Count - 1;
				return string.Format("{0}(@{1})", method, indexOfParameter);
			};
			if (q.StartsWith("^"))
			{
				if (q.EndsWith("$"))
				{
					parametersForLinqQuery.Add(ParseValue(q.Substring(1, q.Length - 2), type));
					var indexOfParameter = parametersForLinqQuery.Count - 1;
					return string.Format("Equals((object)@{0})", indexOfParameter);
				}
				return makeClause("StartsWith", q.Substring(1));
			}
			else
			{
				if (q.EndsWith("$"))
				{
					return makeClause("EndsWith", q.Substring(0, q.Length - 1));
				}
				return makeClause("Contains", q);
			}
		}

		public static string NumericFilter(string query, string columnname, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
		{
			if (query.StartsWith("^")) query = query.TrimStart('^');
			if (query.EndsWith("$")) query = query.TrimEnd('$');

			if (query == "~") return string.Empty;

			if (query.Contains("~"))
			{
				var parts = query.Split('~');
				var clause = null as string;
				try
				{
					parametersForLinqQuery.Add(ChangeType(propertyInfo, parts[0]));
					clause = string.Format("{0} >= @{1}", columnname, parametersForLinqQuery.Count - 1);
				}
				catch (FormatException)
				{
				}

				try
				{
					parametersForLinqQuery.Add(ChangeType(propertyInfo, parts[1]));
					if (clause != null) clause += " and ";
					clause += string.Format("{0} <= @{1}", columnname, parametersForLinqQuery.Count - 1);
				}
				catch (FormatException)
				{
				}
				return clause ?? "true";
			}
			else
			{
				try
				{
					parametersForLinqQuery.Add(ChangeType(propertyInfo, query));
					return string.Format("{0} == @{1}", columnname, parametersForLinqQuery.Count - 1);
				}
				catch (FormatException)
				{
				}
				return null;
			}
		}

		private static object ChangeType(DataTablesPropertyInfo propertyInfo, string query)
		{
			if (propertyInfo.PropertyInfo.PropertyType.IsGenericType &&
					propertyInfo.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				Type u = Nullable.GetUnderlyingType(propertyInfo.Type);
				return Convert.ChangeType(query, u);
			}
			else
			{
				return Convert.ChangeType(query, propertyInfo.Type);
			}
		}

		public static string DateTimeOffsetFilter(string query, string columnname, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
		{
			if (query == "~") return string.Empty;
			if (query.Contains("~"))
			{
				var parts = query.Split('~');

				var filterString = null as string;

				DateTimeOffset start, end;
				if (DateTimeOffset.TryParse(parts[0] ?? "", out start))
				{
					filterString = columnname + " >= @" + parametersForLinqQuery.Count;
					parametersForLinqQuery.Add(start);
				}

				if (DateTimeOffset.TryParse(parts[1] ?? "", out end))
				{
					filterString = (filterString == null ? null : filterString + " and ") + columnname + " <= @" + parametersForLinqQuery.Count;
					parametersForLinqQuery.Add(end);
				}

				return filterString ?? "";
			}
			else
			{
				return string.Format("{1}.ToString(\"u\").{0}", FilterMethod(query, parametersForLinqQuery, propertyInfo.Type), columnname);
			}
		}

		public static string DateTimeFilter(string query, string columnname, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
		{
			if (query == "~") return string.Empty;
			if (query.Contains("~"))
			{
				var parts = query.Split('~');

				var filterString = null as string;

				DateTime start, end;
				if (DateTime.TryParse(parts[0] ?? "", out start))
				{
					filterString = columnname + " >= @" + parametersForLinqQuery.Count;
					parametersForLinqQuery.Add(start);
				}

				if (DateTime.TryParse(parts[1] ?? "", out end))
				{
					filterString = (filterString == null ? null : filterString + " and ") + columnname + " <= @" + parametersForLinqQuery.Count;
					parametersForLinqQuery.Add(end);
				}

				return filterString ?? "";
			}
			else
			{
				return string.Format("{1}.ToString(\"u\").{0}", FilterMethod(query, parametersForLinqQuery, propertyInfo.Type), columnname);
			}
		}

		public static string BoolFilter(string query, string columnname, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
		{
			if (query != null)
				query = query.TrimStart('^').TrimEnd('$');
			var lowerCaseQuery = query.ToLowerInvariant();
			if (lowerCaseQuery == "false" || lowerCaseQuery == "true")
			{
				if (query.ToLower() == "true") return columnname + " == true";
				return columnname + " == false";
			}
			if (propertyInfo.Type == typeof(bool?))
			{
				if (lowerCaseQuery == "null") return columnname + " == null";
			}
			return null;
		}

		public static string StringFilter(string q, string columnname, DataTablesPropertyInfo columntype, List<object> parametersforlinqquery)
		{
			if (q == ".*") return "";
			if (q.StartsWith("^"))
			{
				if (q.EndsWith("$"))
				{
					parametersforlinqquery.Add(q.Substring(1, q.Length - 2));
					var parameterArg = "@" + (parametersforlinqquery.Count - 1);
					return string.Format("{0} ==  {1}", columnname, parameterArg);
				}
				else
				{
					parametersforlinqquery.Add(q.Substring(1));
					var parameterArg = "@" + (parametersforlinqquery.Count - 1);
					return string.Format("({0} != null && {0} != \"\" && ({0} ==  {1} || {0}.StartsWith({1})))", columnname, parameterArg);
				}
			}
			else
			{
				parametersforlinqquery.Add(q);
				var parameterArg = "@" + (parametersforlinqquery.Count - 1);
				//return string.Format("{0} ==  {1}", columnname, parameterArg);
				return
					string.Format("({0} != null && {0} != \"\" && ({0} ==  {1} || {0}.StartsWith({1}) || {0}.Contains({1})))", columnname, parameterArg);
			}
		}

		public static string EnumFilter(string q, string columnname, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
		{
			if (q.StartsWith("^")) q = q.Substring(1);
			if (q.EndsWith("$")) q = q.Substring(0, q.Length - 1);
			var values = Enum.GetValues(propertyInfo.Type);
			foreach (Enum val in values)
			{
				var displayName = val.GetDisplayName().ToLower();
				if (displayName.StartsWith(q.ToLower()))
				{
					parametersForLinqQuery.Add(val);
					return columnname + " == @" + (parametersForLinqQuery.Count - 1);
				}
			}

			return null;
		}
	}
}