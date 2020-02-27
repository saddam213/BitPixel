using BitPixel.Datatables;
using System;
using System.Web.Mvc;

namespace BitPixel.Helpers
{
	public class DataTablesModelBinder : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var valueProvider = bindingContext.ValueProvider;
			int columns = GetValue<int>(valueProvider, "iColumns");
			if (columns == 0)
			{
				return BindV10Model(valueProvider);
			}
			else
			{
				return BindLegacyModel(valueProvider, columns);
			}
		}

		private object BindV10Model(IValueProvider valueProvider)
		{
			DataTablesParam obj = new DataTablesParam
			{
				iDisplayStart = GetValue<int>(valueProvider, "start"),
				iDisplayLength = GetValue<int>(valueProvider, "length"),
				sSearch = GetValue<string>(valueProvider, "search[value]"),
				bEscapeRegex = GetValue<bool>(valueProvider, "search[regex]"),
				sEcho = GetValue<int>(valueProvider, "draw")
			};

			int colIdx = 0;
			while (true)
			{
				string colPrefix = String.Format("columns[{0}]", colIdx);
				string colName = GetValue<string>(valueProvider, colPrefix + "[data]");
				if (String.IsNullOrWhiteSpace(colName))
				{
					break;
				}
				obj.sColumnNames.Add(colName);
				obj.bSortable.Add(GetValue<bool>(valueProvider, colPrefix + "[orderable]"));
				obj.bSearchable.Add(GetValue<bool>(valueProvider, colPrefix + "[searchable]"));
				obj.sSearchValues.Add(GetValue<string>(valueProvider, colPrefix + "[search][value]"));
				obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, colPrefix + "[searchable][regex]"));
				colIdx++;
			}
			obj.iColumns = colIdx;
			colIdx = 0;
			while (true)
			{
				string colPrefix = String.Format("order[{0}]", colIdx);
				int? orderColumn = GetValue<int?>(valueProvider, colPrefix + "[column]");
				if (orderColumn.HasValue)
				{
					obj.iSortCol.Add(orderColumn.Value);
					obj.sSortDir.Add(GetValue<string>(valueProvider, colPrefix + "[dir]"));
					colIdx++;
				}
				else
				{
					break;
				}
			}
			obj.iSortingCols = colIdx;
			return obj;
		}

		private DataTablesParam BindLegacyModel(IValueProvider valueProvider, int columns)
		{
			DataTablesParam obj = new DataTablesParam(columns)
			{
				iDisplayStart = GetValue<int>(valueProvider, "iDisplayStart"),
				iDisplayLength = GetValue<int>(valueProvider, "iDisplayLength"),
				sSearch = GetValue<string>(valueProvider, "sSearch"),
				bEscapeRegex = GetValue<bool>(valueProvider, "bEscapeRegex"),
				iSortingCols = GetValue<int>(valueProvider, "iSortingCols"),
				sEcho = GetValue<int>(valueProvider, "sEcho")
			};

			for (int i = 0; i < obj.iColumns; i++)
			{
				obj.bSortable.Add(GetValue<bool>(valueProvider, "bSortable_" + i));
				obj.bSearchable.Add(GetValue<bool>(valueProvider, "bSearchable_" + i));
				obj.sSearchValues.Add(GetValue<string>(valueProvider, "sSearch_" + i));
				obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, "bEscapeRegex_" + i));
				obj.iSortCol.Add(GetValue<int>(valueProvider, "iSortCol_" + i));
				obj.sSortDir.Add(GetValue<string>(valueProvider, "sSortDir_" + i));
			}
			return obj;
		}

		private static T GetValue<T>(IValueProvider valueProvider, string key)
		{
			ValueProviderResult valueResult = valueProvider.GetValue(key);
			return (valueResult == null)
				? default(T)
				: (T)valueResult.ConvertTo(typeof(T));
		}
	}
}
