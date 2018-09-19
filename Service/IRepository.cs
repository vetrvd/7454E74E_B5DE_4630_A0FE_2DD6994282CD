using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD
{
    /// <summary>
    /// IRepository pattern, access to db layout collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
        where T : IEntity
    {
        /// <summary>
        /// Get unique entity
        /// </summary>
        /// <param name="id">primary key</param>
        /// <returns></returns>
        Task<T> GetById(string id);
        
        /// <summary>
        /// Get all data
        /// </summary>
        /// <param name="skip">skip first {skip} element</param>
        /// <param name="limit">response limit</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll(int? skip, int? limit);
        
        /// <summary>
        /// Get data using query filter
        /// </summary>
        /// <param name="jsonFilter">json query to make request</param>
        /// <param name="skip">skip first {skip} element</param>
        /// <param name="limit">response limit</param>
        /// <returns></returns>
        Task<IEnumerable<T>> Get(string jsonFilter, int? skip, int? limit);

        /// <summary>
        /// Add new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> Add(T entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Update(T entity);

        /// <summary>
        /// Delete entity by key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(string id);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Delete(T entity);
    }
}