using Ducksoft.SOA.Common.DataContracts;
using Ducksoft.SOA.Common.EFHelpers.Interfaces;
using Ducksoft.SOA.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Services.Client;
using System.Data.Services.Common;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace Ducksoft.SOA.Common.EFHelpers.Models
{
    /// <summary>
    /// Class which is used to query entity set data through given WCF data service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EfDataServiceRepository<TEntity> : EntityLoader<DataServiceContext>,
        IQueryableRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfDataServiceRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="dataServiceUrl">The data service URL.</param>
        public EfDataServiceRepository(Uri dataServiceUrl)
            : this(dataServiceUrl, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfDataServiceRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="dataServiceUrl">The data service URL.</param>
        /// <param name="connectionInfo">The connection.</param>
        public EfDataServiceRepository(Uri dataServiceUrl, DbConnectionInfo connectionInfo)
            : base(dataServiceUrl, connectionInfo)
        {
            //Hp --> Logic: To get name of the entity set in pluralization format.
            var service = DbConfiguration.DependencyResolver.GetService<IPluralizationService>();
            EntitySetName = service.Pluralize(typeof(TEntity).Name);
            EntitySet = DataSvcClient.CreateQuery<TEntity>(EntitySetName);
        }

        /// <summary>
        /// Gets the name of the entity set.
        /// </summary>
        /// <value>
        /// The name of the entity set.
        /// </value>
        public string EntitySetName { get; private set; }

        /// <summary>
        /// Gets the entity set.
        /// </summary>
        /// <value>
        /// The entity set.
        /// </value>
        public DataServiceQuery<TEntity> EntitySet { get; private set; }

        /// <summary>
        /// Gets the rows count.
        /// </summary>
        /// <value>
        /// The rows count.
        /// </value>
        public int RowsCount
        {
            get
            {
                ErrorBase.Require(null != EntitySet);
                return (EntitySet.Count());
            }
        }

        /// <summary>
        /// Gets all records.
        /// Note: If server side pagination is set then it will return records based on that limit.
        /// Also, If records to retreive is larger then it may throw low memory exception.
        /// </summary>
        /// <param name="isPaginationSet">if set to <c>true</c> [is pagination set].</param>
        /// <returns></returns>
        public virtual IEnumerable<IQueryable<TEntity>> GetAll(bool isPaginationSet)
        {
            ErrorBase.Require(null != EntitySet);
            ErrorBase.Require(null != DataSvcClient);

            // Hp --> Logic: In order to get all records we need to loop untill data service 
            //continuation token is null.
            DataServiceQueryContinuation<TEntity> token = null;
            var response = EntitySet.Execute() as QueryOperationResponse<TEntity>;
            do
            {
                // If nextLink is not null, then there is a new page to load. 
                if (null != token)
                {
                    // Load the new page from the next link URI.
                    response = DataSvcClient.Execute(token);
                }

                //Hp --> Logic: To improve the performance yield the return value.
                yield return (response.AsQueryable());

            } while ((!isPaginationSet) && (null != (token = response.GetContinuation())));
        }

        /// <summary>
        /// Gets all records based on given query options.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns></returns>
        public virtual IEnumerable<IQueryable<TEntity>> GetAll(IList<QueryOption> queryOptions)
        {
            ErrorBase.Require(null != EntitySet);
            ErrorBase.Require(null != DataSvcClient);
            ErrorBase.CheckArgIsNullOrDefault(queryOptions, () => queryOptions);

            // Hp --> Logic: Create query by adding given query options.
            DataServiceQuery<TEntity> query = null;
            queryOptions.ToList().ForEach(item =>
            {
                if (null == query)
                {
                    query = EntitySet.AddQueryOption(item.Option, item.Query);
                }
                else
                {
                    query = query.AddQueryOption(item.Option, item.Query);
                }
            });

            // Hp --> Logic: In order to get all records we need to loop untill data service 
            //continuation token is null.
            DataServiceQueryContinuation<TEntity> token = null;
            var response = query.Execute() as QueryOperationResponse<TEntity>;
            do
            {
                // If nextLink is not null, then there is a new page to load. 
                if (null != token)
                {
                    // Load the new page from the next link URI.
                    response = DataSvcClient.Execute(token);
                }

                //Hp --> Logic: To improve the performance yield the return value.
                yield return (response.AsQueryable());

            } while (null != (token = response.GetContinuation()));
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public virtual IEnumerable<IQueryable<TEntity>> GetAll(
            Expression<Func<TEntity, bool>> predicate)
        {
            ErrorBase.Require(null != EntitySet);
            ErrorBase.Require(null != DataSvcClient);
            ErrorBase.CheckArgIsNull(predicate, () => predicate);

            // Hp --> Logic: Create query by using predicate expression.
            var query = EntitySet.Where(predicate) as DataServiceQuery<TEntity>;

            // Hp --> Logic: In order to get all records we need to loop untill data service 
            //continuation token is null.
            DataServiceQueryContinuation<TEntity> token = null;
            var response = query.Execute() as QueryOperationResponse<TEntity>;
            do
            {
                // If nextLink is not null, then there is a new page to load. 
                if (null != token)
                {
                    // Load the new page from the next link URI.
                    response = DataSvcClient.Execute(token);
                }

                //Hp --> Logic: To improve the performance yield the return value.
                yield return (response.AsQueryable());

            } while (null != (token = response.GetContinuation()));
        }

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public virtual PaginationData<TEntity> GetPaginationData(int pageNumber, int pageSize)
        {
            ErrorBase.CheckArgIsValid(pageNumber, () => pageNumber, index => (0 < index));
            ErrorBase.CheckArgIsValid(pageSize, () => pageSize, size => (0 < size));
            ErrorBase.Require(null != EntitySet);

            //Hp --> Note: If server side pagination limit is set, then it will retreive records
            //based on that limit. i.e., If pagesize provided is greater than pagination limit then
            //it will always retreive records based on pagination limit.
            return (new PaginationData<TEntity>()
            {
                TotalItems = RowsCount,
                PageData = EntitySet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            });
        }

        /// <summary>
        /// Gets the pagination data.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public virtual PaginationData<TEntity> GetPaginationData(string expression, int skip, int take)
        {
            ErrorBase.CheckArgIsNullOrDefault(expression, () => expression);
            ErrorBase.CheckArgIsValid(skip, () => skip, index => (0 <= index));
            ErrorBase.CheckArgIsValid(take, () => take, size => (0 < size));
            ErrorBase.Require(null != EntitySet);

            var result = EntitySet.Where(expression);
            return (new PaginationData<TEntity>()
            {
                TotalItems = result.Count(),
                PageData = result.Skip(skip).Take(take).ToList()
            });
        }

        /// <summary>
        /// Searches the by.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> SearchBy(Expression<Func<TEntity, bool>> predicate)
        {
            ErrorBase.CheckArgIsNull(predicate, () => predicate);
            ErrorBase.Require(null != EntitySet);

            return (EntitySet.Where(predicate));
        }

        /// <summary>
        /// Searches the by.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public IQueryable<TEntity> SearchBy(string expression)
        {
            ErrorBase.CheckArgIsNullOrDefault(expression, () => expression);
            return (EntitySet.Where(expression));
        }

        /// <summary>
        /// Finds the first.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public virtual TEntity FindFirst(Expression<Func<TEntity, bool>> predicate)
        {
            ErrorBase.CheckArgIsNull(predicate, () => predicate);
            ErrorBase.Require(null != EntitySet);

            return (EntitySet.Where(predicate).ToList().DefaultIfEmpty(null).First());
        }

        /// <summary>
        /// Gets the primary key based predicate expression dynamically.
        /// </summary>
        /// <param name="srcEntity">The source entity.</param>
        /// <returns></returns>
        public virtual Expression<Func<TEntity, bool>> GetPKExpression(TEntity srcEntity)
        {
            // Find primary key names based on data service key attribute.
            var myDsKeyAttr = typeof(TEntity).GetCustomAttributes(
                typeof(DataServiceKeyAttribute), true).FirstOrDefault(k =>
                    (0 != ((DataServiceKeyAttribute)k).KeyNames.Count)) as DataServiceKeyAttribute;

            // Create entity => portion of lambda expression
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression body = null;

            foreach (var key in myDsKeyAttr.KeyNames)
            {
                // create entity.Id portion of lambda expression
                var property = Expression.Property(parameter, key);

                // create 'id' portion of lambda expression
                var id = srcEntity.GetType().GetProperty(key).GetValue(srcEntity, null);
                var equalsTo = Expression.Constant(id);

                // create entity.Id == 'id' portion of lambda expression
                var equality = Expression.Equal(property, equalsTo);

                //TODO: Hp --> Needs to check whether below line of code works for multiple Pkeys.
                body = (null != body) ? Expression.AndAlso(body, equality) : equality;
            }

            // finally create entire expression - entity => entity.Id == 'id'
            var result =
                Expression.Lambda<Func<TEntity, bool>>(body, new[] { parameter });

            return (result);
        }

        /// <summary>
        /// Adds the new entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        public virtual void AddNew(TEntity entity, bool commit = true)
        {
            ErrorBase.CheckArgIsNull(entity, () => entity);
            ErrorBase.Require(null != DataSvcClient);

            DataSvcClient.AddObject(EntitySetName, entity);
            try
            {
                if (commit)
                {
                    DataSvcClient.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        public virtual void Insert(TEntity entity, bool commit = true)
        {
            ErrorBase.CheckArgIsNull(entity, () => entity);
            ErrorBase.Require(null != DataSvcClient);

            DataSvcClient.AddObject(EntitySetName, entity);
            try
            {
                if (commit)
                {
                    DataSvcClient.SaveChanges();
                }
            }
            catch
            {
                //Hp --> Logic: Need to restore back the original data if commit fails.
                DataSvcClient.DeleteObject(entity);
                throw;
            }
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        /// <exception cref="System.ApplicationException">The</exception>
        public virtual void Update(TEntity entity, bool commit = true)
        {
            ErrorBase.CheckArgIsNull(entity, () => entity);
            ErrorBase.Require(null != DataSvcClient);

            var matchResult = FindFirst(GetPKExpression(entity));
            if (null == matchResult)
            {
                throw (new ApplicationException("Entity doesn't exist in the repository"));
            }

            try
            {
                DataSvcClient.UpdateObject(entity);
            }
            catch (ArgumentException)
            {
                // Hp --> Logic: This means Context didn't track this entity, so we need to detach 
                //the old one and attach this one.
                try
                {
                    DataSvcClient.Detach(matchResult);
                    DataSvcClient.AttachTo(EntitySetName, entity);
                    DataSvcClient.UpdateObject(entity);
                }
                catch
                {
                    //Hp --> Logic: Roll back
                    DataSvcClient.Detach(entity);
                    DataSvcClient.AttachTo(EntitySetName, matchResult);
                    throw;
                }
            }

            try
            {
                if (commit)
                {
                    DataSvcClient.SaveChanges();
                }
            }
            catch (DataServiceRequestException ex)
            {
                var response = ex.Response.FirstOrDefault();
                //Hp --> Logic: Concurrency Exception - PreCondition Failed (412) or Conflict occured (409)
                if ((null != response) && ((412 == response.StatusCode) || (409 == response.StatusCode)))
                {
                    var oldOp = DataSvcClient.MergeOption;
                    DataSvcClient.MergeOption = MergeOption.PreserveChanges;
                    entity = matchResult;
                    DataSvcClient.SaveChanges();
                    DataSvcClient.MergeOption = oldOp;
                }
            }
            catch
            {
                //Hp --> Logic: Need to restore back the original data if commit fails.
                var oldOp = DataSvcClient.MergeOption;
                DataSvcClient.MergeOption = MergeOption.OverwriteChanges;
                entity = matchResult;
                DataSvcClient.MergeOption = oldOp;
                throw;
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commit">if set to <c>true</c> [commit].</param>
        public virtual void Delete(TEntity entity, bool commit = true)
        {
            ErrorBase.CheckArgIsNull(entity, () => entity);
            ErrorBase.Require(null != DataSvcClient);

            var matchResult = FindFirst(GetPKExpression(entity));
            DataSvcClient.DeleteObject(matchResult);
            try
            {
                if (commit)
                {
                    DataSvcClient.SaveChanges();
                }
            }
            catch
            {
                //Hp --> Logic: Need to restore back the original data if commit fails.
                DataSvcClient.AddObject(EntitySetName, entity);
                throw;
            }
        }

        /// <summary>
        /// Commits this instance changes.
        /// </summary>
        public virtual void Commit()
        {
            ErrorBase.CheckArgIsNull(DataSvcClient, () => DataSvcClient);
            DataSvcClient.SaveChanges();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            ErrorBase.CheckArgIsNull(EntitySet, () => EntitySet);
            return (EntitySet.GetEnumerator());
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (GetEnumerator());
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public Type ElementType
        {
            get { return (typeof(TEntity)); }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable" />.
        /// </summary>
        public Expression Expression
        {
            get
            {
                ErrorBase.CheckArgIsNull(EntitySet, () => EntitySet);
                return (EntitySet.Expression);
            }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public IQueryProvider Provider
        {
            get
            {
                ErrorBase.CheckArgIsNull(EntitySet, () => EntitySet);
                return (EntitySet.Provider);
            }
        }
    }
}