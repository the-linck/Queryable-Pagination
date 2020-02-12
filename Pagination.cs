using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;



namespace QueryablePagination
{
    /// <summary>
    ///  <para>
    ///   General set of properties needed for pagination of queries without looking into the queried data.
    ///  </para>
    ///  <para>
    ///   This interface is intended to be used as a model a "generic" Partial View for pagination on MVC,
    ///   allowing better code reuse in a project.
    ///  </para>
    /// </summary>
    public interface IPaged
    {
        /// <summary>
        /// Current page on the results.
        /// </summary>
        int Page
        {
            get;
        }
        /// <summary>
        /// Number of result pages.
        /// </summary>
        int PageCount
        {
            get;
        }
        /// <summary>
        /// Number of records to read on each page.
        /// </summary>
        int PerPage
        {
            get;
        }
        /// <summary>
        /// Number of records avaliable to read.
        /// </summary>
        int Total
        {
            get;
        }
    }

    /// <summary>
    ///  Standard implementation of <see cref="QueryablePagination.IPaged"/>, 
    ///  providing basic pagination functionality without loading the data from the query,
    ///  only the number of avaliable records is read.
    /// </summary>
    /// <typeparam name="TQueryable">Type of Entity that will be queryied</typeparam>
    /// <typeparam name="TResult">Type in wich the query results will be stored</typeparam>
    public abstract class Pagination<TQueryable, TResult> : IPaged
    {
        /// <summary>
        /// Data that will be loaded by the query, already on the desired format.
        /// </summary>
        protected TResult Data
        {
            get;
            set;
        }
        /// <inheritdoc/>
        public int Page
        {
            get;
        }
        /// <inheritdoc />
        public int PageCount
        {
            get;
        }
        /// <inheritdoc />
        public int PerPage
        {
            get;
        }
        /// <summary>
        /// Readonly access to the loaded query data.
        /// </summary>
        public TResult Results
        {
            get
            {
                return Data;
            }
        }
        /// <inheritdoc />
        public int Total
        {
            get;
        }
        /// <summary>
        ///  <para>
        ///   Number of records to skip from the begining.
        ///  </para>
        ///  <para>
        ///   Meant to be used only internally, for pagination.
        ///  </para>
        /// </summary>
        protected int ToSkip;



        /// <summary>
        /// Basic constructor that reads the number of avaliable records from
        /// the query and does some pagination calcs.
        /// </summary>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public Pagination(
            IOrderedQueryable<TQueryable> Query,
            int Page = 1,
            int PerPage = 100
        )
        {
            // Getting the number of registers
            Total = Query.Count();

            this.Page = Page;
            this.PerPage = PerPage;
            PageCount = (int)Math.Ceiling((decimal)(Total / PerPage));

            ToSkip = (Page - 1) * PerPage;
        }
    }



    /// <summary>
    /// Array-based implementation of <see cref="QueryablePagination.Pagination"/>,
    /// implementing pagination with an array of <typeparamref name="TQueryable"/>.
    /// </summary>
    /// <typeparam name="TQueryable">Type of Entity that will be queryied.</typeparam>
    public class PagedArray<TQueryable> : Pagination<TQueryable, Array>
    {
        /// <summary>
        /// Creates a new <see cref="QueryablePagination.Pagination"/> instance with
        /// data stored in an Array.
        /// </summary>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public PagedArray(
            IOrderedQueryable<TQueryable> Query,
            int Page = 1,
            int PerPage = 100
        ) : base(Query, Page, PerPage)
        {
            Data = Query.Skip(ToSkip).Take(PerPage).ToArray();
        }
    }

    /// <summary>
    /// Dictionary-based implementation of <see cref="QueryablePagination.Pagination"/>,
    /// implementing pagination with a dictionary of <typeparamref name="TQueryable"/>
    /// that uses <typeparamref name="TKey"/> keys.
    /// </summary>
    /// <typeparam name="TQueryable">Type of Entity that will be queryied.</typeparam>
    /// <typeparam name="TKey">Type of key for the dictionary.</typeparam>
    public class PagedDictionary<TKey, TQueryable> : Pagination<TQueryable, Dictionary<TKey, TQueryable>>
    {
        /// <summary>
        /// Creates a new <see cref="QueryablePagination.Pagination"/> instance with
        /// data stored in a Dictionary.
        /// </summary>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="keySelector">Function to get keys for the records.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public PagedDictionary(
            IOrderedQueryable<TQueryable> Query,
            Func<TQueryable, TKey> keySelector,
            int Page = 1,
            int PerPage = 100
        ) : base(Query, Page, PerPage)
        {
            Data = Query.Skip(ToSkip).Take(PerPage).ToDictionary(keySelector);
        }
    }

    /// <summary>
    /// HashSet-based implementation of <see cref="QueryablePagination.Pagination"/>,
    /// implementing pagination with a HashSet of <typeparamref name="TQueryable"/>.
    /// </summary>
    /// <typeparam name="TQueryable">Type of Entity that will be queryied.</typeparam>
    public class PagedHashSet<TQueryable> : Pagination<TQueryable, HashSet<TQueryable>>
    {
        /// <summary>
        /// Creates a new <see cref="QueryablePagination.Pagination"/> instance with
        /// data stored in a HashSet.
        /// </summary>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public PagedHashSet(
            IOrderedQueryable<TQueryable> Query,
            int Page = 1,
            int PerPage = 100
        ) : base(Query, Page, PerPage)
        {
            Data = Query.Skip(ToSkip).Take(PerPage).ToHashSet();
        }
    }

    /// <summary>
    /// Lookup-based implementation of <see cref="QueryablePagination.Pagination"/>,
    /// implementing pagination with a lookup of <typeparamref name="TQueryable"/>
    /// that uses <typeparamref name="TKey"/> keys.
    /// </summary>
    /// <typeparam name="TQueryable">Type of Entity that will be queryied.</typeparam>
    /// <typeparam name="TKey">Type of key for the lookup.</typeparam>
    public class PagedLookup<TKey, TQueryable> : Pagination<TQueryable, ILookup<TKey, TQueryable>>
    {
        /// <summary>
        /// Creates a new <see cref="QueryablePagination.Pagination"/> instance with
        /// data stored in a Lookup.
        /// </summary>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="keySelector">Function to get keys for the records.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public PagedLookup(
            IOrderedQueryable<TQueryable> Query,
            Func<TQueryable, TKey> keySelector,
            int Page = 1,
            int PerPage = 100
        ) : base(Query, Page, PerPage)
        {
            Data = Query.Skip(ToSkip).Take(PerPage).ToLookup(keySelector);
        }
    }

    /// <summary>
    /// List-based implementation of <see cref="QueryablePagination.Pagination"/>,
    /// implementing pagination with a list of <typeparamref name="TQueryable"/>.
    /// </summary>
    /// <typeparam name="TQueryable">Type of Entity that will be queryied.</typeparam>
    public class PagedList<TQueryable> : Pagination<TQueryable, List<TQueryable>>
    {
        /// <summary>
        /// Creates a new <see cref="QueryablePagination.Pagination"/> instance with
        /// data stored in a List.
        /// </summary>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public PagedList(
            IOrderedQueryable<TQueryable> Query,
            int Page = 1,
            int PerPage = 100
        ) : base(Query, Page, PerPage)
        {
            Data = Query.Skip(ToSkip).Take(PerPage).ToList();
        }
    }



    /// <summary>
    /// Set of extension methods that allow to easily create a <see cref="QueryablePagination.Pagination"/>
    /// instance from a <see cref="System.Linq.IOrderedQueryable"/>.
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="QueryablePagination.Pagination"/>,
        /// that uses an array of <typeparamref name="T"/> to store results.
        /// </summary>
        /// <typeparam name="T">Type of Entity that will be queryied.</typeparam>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public static PagedArray<T> ToPagedArray<T>(
            this IOrderedQueryable<T> Query,
            int Page = 1,
            int PerPage = 100
        )
        {
            return new PagedArray<T>(Query, Page, PerPage);
        }
        /// <summary>
        /// Creates an instance of <see cref="QueryablePagination.Pagination"/>,
        /// that uses a Dictionary of <typeparamref name="T"/> with
        /// <typeparamref name="TKey"/> keys to store results.
        /// </summary>
        /// <typeparam name="T">Type of Entity that will be queryied.</typeparam>
        /// <typeparam name="TKey">Type of key for the dictionary.</typeparam>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public static PagedDictionary<TKey, T> ToPagedDictionary<TKey, T>(
            this IOrderedQueryable<T> Query,
            Func<T, TKey> keySelector,
            int Page = 1,
            int PerPage = 100
        )
        {
            return new PagedDictionary<TKey, T>(Query, keySelector, Page, PerPage);
        }
        /// <summary>
        /// Creates an instance of <see cref="QueryablePagination.Pagination"/>,
        /// that uses a HashSet of <typeparamref name="T"/> to store results.
        /// </summary>
        /// <typeparam name="T">Type of Entity that will be queryied.</typeparam>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public static PagedHashSet<T> ToPagedHashSet<T>(
            this IOrderedQueryable<T> Query,
            int Page = 1,
            int PerPage = 100
        )
        {
            return new PagedHashSet<T>(Query, Page, PerPage);
        }
        /// <summary>
        /// Creates an instance of <see cref="QueryablePagination.Pagination"/>,
        /// that uses a Lookup of <typeparamref name="T"/> with
        /// <typeparamref name="TKey"/> keys to store results.
        /// </summary>
        /// <typeparam name="T">Type of Entity that will be queryied.</typeparam>
        /// <typeparam name="TKey">Type of key for the lookup.</typeparam>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public static PagedLookup<TKey, T> ToPagedLookup<TKey, T>(
            this IOrderedQueryable<T> Query,
            Func<T, TKey> keySelector,
            int Page = 1,
            int PerPage = 100
        )
        {
            return new PagedLookup<TKey, T>(Query, keySelector, Page, PerPage);
        }
        /// <summary>
        /// Creates an instance of <see cref="QueryablePagination.Pagination"/>,
        /// that uses a list of <typeparamref name="T"/> to store results.
        /// </summary>
        /// <typeparam name="T">Type of Entity that will be queryied.</typeparam>
        /// <param name="Query">The query to paginate, must be ordered.</param>
        /// <param name="Page">Current page of the results.</param>
        /// <param name="PerPage">Number of records to read on each page.</param>
        public static PagedList<T> ToPagedList<T>(
            this IOrderedQueryable<T> Query,
            int Page = 1,
            int PerPage = 100
        )
        {
            return new PagedList<T>(Query, Page, PerPage);
        }
    }
}