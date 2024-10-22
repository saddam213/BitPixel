﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BitPixel.Datatables.DynamicLinq
{
	public static class DynamicQueryable
	{

		public static IQueryable Join(this IQueryable outer, System.Collections.IEnumerable inner,
			string outerSelector, string innerSelector, string resultsSelector, params object[] values)
		{
			if (inner == null) throw new ArgumentNullException("inner");
			if (outerSelector == null) throw new ArgumentNullException("outerSelector");
			if (innerSelector == null) throw new ArgumentNullException("innerSelector");
			if (resultsSelector == null) throw new ArgumentNullException("resultsSelector");

			LambdaExpression outerSelectorLambda = DynamicExpression.ParseLambda(itType: outer.ElementType, resultType: null, expression: outerSelector, baseType: null, values: values);
			LambdaExpression innerSelectorLambda = DynamicExpression.ParseLambda(itType: inner.AsQueryable().ElementType, resultType: null, expression: innerSelector, baseType: null, values: values);
			ParameterExpression[] parameters = new ParameterExpression[] { Expression.Parameter(outer.ElementType, "outer"), Expression.Parameter(inner.AsQueryable().ElementType, "inner") };
			LambdaExpression resultsSelectorLambda = DynamicExpression.ParseLambda(parameters: parameters, baseType: null, resultType: null, expression: resultsSelector, values: values);
			return outer.Provider.CreateQuery(
				Expression.Call(typeof(Queryable), "Join", new Type[]
															   {
																   outer.ElementType, inner.AsQueryable().ElementType, outerSelectorLambda.Body.Type, resultsSelectorLambda.Body.Type
															   }, outer.Expression, inner.AsQueryable().Expression, Expression.Quote(outerSelectorLambda), Expression.Quote(innerSelectorLambda), Expression.Quote(resultsSelectorLambda)));
		}       //The generic overload.    

		public static IQueryable<T> Join<T>(this IQueryable<T> outer, IEnumerable<T> inner, string outerSelector, string innerSelector, string resultsSelector, params object[] values)
		{
			return (IQueryable<T>)Join((IQueryable)outer, (System.Collections.IEnumerable)inner, outerSelector, innerSelector, resultsSelector, values);
		}

		public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
		{
			return (IQueryable<T>)Where((IQueryable)source, predicate, values);
		}

		public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");

			LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, null, values);
			return source.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Where",
					new Type[] { source.ElementType },
					source.Expression, Expression.Quote(lambda)));
		}

		public static IQueryable Select(this IQueryable source, string selector, params object[] values)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (selector == null) throw new ArgumentNullException("selector");
			LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, null, values);
			return source.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Select",
					new Type[] { source.ElementType, lambda.Body.Type },
					source.Expression, Expression.Quote(lambda)));
		}



		public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
		{
			return (IQueryable<T>)OrderBy((IQueryable)source, ordering, values);
		}

		public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (ordering == null) throw new ArgumentNullException("ordering");
			ParameterExpression[] parameters = new ParameterExpression[] {
				Expression.Parameter(source.ElementType, "") };
			ExpressionParser parser = new ExpressionParser(parameters, ordering, values);
			IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
			Expression queryExpr = source.Expression;
			string methodAsc = "OrderBy";
			string methodDesc = "OrderByDescending";
			foreach (DynamicOrdering o in orderings)
			{
				queryExpr = Expression.Call(
					typeof(Queryable), o.Ascending ? methodAsc : methodDesc,
					new Type[] { source.ElementType, o.Selector.Type },
					queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
				methodAsc = "ThenBy";
				methodDesc = "ThenByDescending";
			}
			return source.Provider.CreateQuery(queryExpr);
		}

		public static IQueryable Take(this IQueryable source, int count)
		{
			if (source == null) throw new ArgumentNullException("source");
			return source.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Take",
					new Type[] { source.ElementType },
					source.Expression, Expression.Constant(count)));
		}

		public static IQueryable Skip(this IQueryable source, int count)
		{
			if (source == null) throw new ArgumentNullException("source");
			return source.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Skip",
					new Type[] { source.ElementType },
					source.Expression, Expression.Constant(count)));
		}

		public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, params object[] values)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (keySelector == null) throw new ArgumentNullException("keySelector");
			if (elementSelector == null) throw new ArgumentNullException("elementSelector");
			LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, null, values);
			LambdaExpression elementLambda = DynamicExpression.ParseLambda(source.ElementType, null, elementSelector, null, values);
			return source.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "GroupBy",
					new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
					source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
		}

		public static bool Any(this IQueryable source)
		{
			if (source == null) throw new ArgumentNullException("source");
			return (bool)source.Provider.Execute(
				Expression.Call(
					typeof(Queryable), "Any",
					new Type[] { source.ElementType }, source.Expression));
		}

		public static int Count(this IQueryable source)
		{
			if (source == null) throw new ArgumentNullException("source");
			return (int)source.Provider.Execute(
				Expression.Call(
					typeof(Queryable), "Count",
					new Type[] { source.ElementType }, source.Expression));
		}
	}
}
