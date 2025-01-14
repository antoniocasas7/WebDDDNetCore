﻿using Dapper;
using System.Collections.Generic;
using System.Data;

namespace Infrastructure.Persistence.SQL
{
    /// <summary>
    ///     Dapper + QueryObject extensions
    /// </summary>
    internal static class QueryObjectExtensions
    {
        /// <summary>
        ///     Dapper + QueryObject extension for Query
        /// </summary>
        public static IEnumerable<T> Query<T>(this IDbConnection cnn, QueryObject queryObject, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.Query<T>(queryObject.Sql, queryObject.QueryParams, transaction, buffered, commandTimeout, commandType);
        }

        /// <summary>
        ///     Dapper + QueryObject extension for Execute
        /// </summary>
        public static int Execute(this IDbConnection cnn, QueryObject queryObject, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return cnn.Execute(queryObject.Sql, queryObject.QueryParams, transaction, commandTimeout, commandType);
        }
    }
}
