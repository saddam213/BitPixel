using System.Collections.Generic;
using System.Linq;
using BitPixel.Datatables.Models;
using BitPixel.Datatables.Reflection;
using System.Data.Entity;
using System.Threading.Tasks;
using System;


namespace BitPixel.Datatables
{
	public static class DataTablesExtensions
	{
		public static DataTablesResponseData GetDataTableResponse<TSource>(this IEnumerable<TSource> data, DataTablesParam param, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			TSource[] queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(param, data.AsQueryable(), outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = filteredData.Count();
				var skipped = filteredData.Skip(param.iDisplayStart);
				queryResult = (param.iDisplayLength <= 0 ? skipped : skipped.Take(param.iDisplayLength)).ToArray();
			}
			else
			{
				queryResult = filteredData.ToArray();
				totalDisplayRecords = queryResult.Count();
			}

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = param.sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			var responseOptions = new ResponseOptions<TSource> { ArrayOutputType = null };
			var dictionaryTransform = DataTablesTypeInfo<TSource>.ToDictionary(responseOptions);
			result = result.Transform(dictionaryTransform).Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);
			result = ApplyOutputRules(result, responseOptions);

			return result;
		}


		public static async Task<DataTablesResponseData> GetDataTableResponseAsync<TSource>(this IQueryable<TSource> data, DataTablesParam param, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			List<TSource> queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(param, data, outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = await filteredData.CountAsync();
				var skipped = filteredData.Skip(param.iDisplayStart);
				queryResult = await (param.iDisplayLength <= 0 ? skipped : skipped.Take(param.iDisplayLength)).ToListAsync();
			}
			else
			{
				queryResult = await filteredData.ToListAsync();
				totalDisplayRecords = queryResult.Count();
			}

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = param.sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			var responseOptions = new ResponseOptions<TSource> { ArrayOutputType = null };
			var dictionaryTransform = DataTablesTypeInfo<TSource>.ToDictionary(responseOptions);
			result = result.Transform(dictionaryTransform).Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);
			result = ApplyOutputRules(result, responseOptions);

			return result;
		}


		public static async Task<DataTablesResponseData> GetDataTableResponseAsync<TSource>(this IQueryable<TSource> data, DataTablesParam param, Action<TSource> postQueryTransform, bool disablePaging = false)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;

			List<TSource> queryResult;
			var totalDisplayRecords = 0;
			var filteredData = filters.ApplyFiltersAndSort(param, data, outputProperties);
			if (!disablePaging)
			{
				totalDisplayRecords = await filteredData.CountAsync();
				var skipped = filteredData.Skip(param.iDisplayStart);
				queryResult = await (param.iDisplayLength <= 0 ? skipped : skipped.Take(param.iDisplayLength)).ToListAsync();
			}
			else
			{
				queryResult = await filteredData.ToListAsync();
				totalDisplayRecords = queryResult.Count();
			}

			queryResult.ForEach(postQueryTransform);

			var result = new DataTablesResponseData()
			{
				iTotalRecords = totalDisplayRecords,
				iTotalDisplayRecords = totalDisplayRecords,
				sEcho = param.sEcho,
				aaData = queryResult.Cast<object>().ToArray(),
			};

			var responseOptions = new ResponseOptions<TSource> { ArrayOutputType = null };
			var dictionaryTransform = DataTablesTypeInfo<TSource>.ToDictionary(responseOptions);
			result = result.Transform(dictionaryTransform).Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);
			result = ApplyOutputRules(result, responseOptions);
			return result;
		}

		public static async Task<IEnumerable<TSource>> GetObjectResponseAsync<TSource>(this IQueryable<TSource> data, DataTablesParam param)
		{
			var filters = new DataTablesFiltering();
			var outputProperties = DataTablesTypeInfo<TSource>.Properties;
			var filteredData = filters.ApplyFiltersAndSort(param, data, outputProperties);
			var queryResult = await filteredData.ToListAsync();
			return queryResult;
		}

		private static DataTablesResponseData ApplyOutputRules<TSource>(DataTablesResponseData sourceData, ResponseOptions<TSource> responseOptions)
		{
			responseOptions = responseOptions ?? new ResponseOptions<TSource> { ArrayOutputType = ArrayOutputType.BiDimensionalArray };
			var outputData = sourceData;

			switch (responseOptions.ArrayOutputType)
			{
				case ArrayOutputType.ArrayOfObjects:
					// Nothing is needed
					break;
				case ArrayOutputType.BiDimensionalArray:
				default:
					outputData = sourceData.Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray());
					break;
			}

			return outputData;
		}
	}
}