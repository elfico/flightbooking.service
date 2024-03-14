using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FlightBooking.Service.Data.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        //Responses: failed=0, success=1

        //IEnumerable iterates over an in-memory collection while IQueryable does so on the DB
        // call to .ToList to enable instant query against DB

        protected FlightBookingContext _db;
        protected ILogger _logger;

        public GenericRepository(FlightBookingContext db, ILogger<GenericRepository<T>> logger)
        {
            _logger = logger;
            _db = db;
        }

        public IQueryable<T> GetAll()
        {
            return _db.Set<T>();
        }

        public DbSet<T> GetDbSet()
        {
            return _db.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _db.Set<T>().AsQueryable();
        }

        #region Async Methods

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);

            return entity;
        }

        public async Task<T?> GetByGuidAsync(Guid id)
        {
            var entity = await _db.Set<T>().FindAsync(id);

            return entity;
        }

        public async Task<int> CreateAsync(T entity, bool isSave = true)
        {
            if (entity == null)
            {
                _logger.LogError(RepositoryConstants.CreateNullError, typeof(T).Name);
                return (int)InternalCode.EntityIsNull;
            }

            _db.Set<T>().Add(entity);

            if (isSave)
            {
                return await SaveChangesToDbAsync();
            }

            return (int)InternalCode.Success;
        }

        public async Task<int> UpdateAsync(T entity, bool isSave = true)
        {
            //Check for this in each overriding implementation or services
            //var prev = await GetById(id);

            //if (prev == null)
            //{
            //    return 0;
            //}

            _db.Set<T>().Update(entity);

            if (isSave)
            {
                return await SaveChangesToDbAsync();
            }

            return (int)InternalCode.Success;
        }

        public async Task<int> DeleteAsync(int id, bool isSave = true)
        {
            T? entity = await GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogError(RepositoryConstants.DeleteNullError, typeof(T).Name);
                return (int)InternalCode.EntityNotFound;
            }

            _db.Set<T>().Remove(entity);

            if (isSave)
            {
                return await SaveChangesToDbAsync();
            }

            return (int)InternalCode.Success;
        }

        public async Task<int> BulkDeleteAsync(IEnumerable<int> entityId, bool isSave = true)
        {
            if (entityId == null || !entityId.Any())
            {
                _logger.LogError(RepositoryConstants.BulkDeleteNullError, typeof(T).Name);
                return (int)InternalCode.EntityIsNull;
            }

            DbSet<T> table = _db.Set<T>();

            foreach (int id in entityId)
            {
                T? entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    table.Remove(entity);
                }
            }

            if (isSave)
            {
                return await SaveChangesToDbAsync();
            }

            return (int)InternalCode.Success;
        }

        public async Task<int> BulkCreateAsync(IEnumerable<T> entities, bool isSave = true)
        {
            if (entities == null || !entities.Any())
            {
                _logger.LogError(RepositoryConstants.BulkCreateNullError, typeof(T).Name);
                return (int)InternalCode.EntityIsNull;
            }

            DbSet<T> table = _db.Set<T>();

            table.AddRange(entities);

            if (isSave)
            {
                return await SaveChangesToDbAsync();
            }

            return (int)InternalCode.Success;
        }

        //calling this once works since we are using just one DbContext
        //TODO: returning 0 should not lead to 500 error. 0 means no entries were added which may be because all entries have been added already
        //fix this after tests have been writing for projects
        public async Task<int> SaveChangesToDbAsync()
        {
            _logger.LogInformation(RepositoryConstants.LoggingStarted);
            int saveResult;

            try
            {
                int tempResult = await _db.SaveChangesAsync(); //give numbers of entries updated in db. in some cases e.g Update, when no data changes, this method returns 0
                if (tempResult == 0)
                {
                    _logger.LogInformation(RepositoryConstants.EmptySaveInfo);
                }
                saveResult = (int)InternalCode.Success; //means atleast one entry was made. 1 is InternalCode.Success.
                                                        //saveResult = tempResult > 0 ? 1 : 0; //means atleast one entry was made. 1 is InternalCode.Success
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, RepositoryConstants.UpdateConcurrencyException);
                saveResult = (int)InternalCode.UpdateError;
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, RepositoryConstants.UpdateException);
                saveResult = (int)InternalCode.UpdateError;
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, RepositoryConstants.SaveChangesException);
                saveResult = (int)InternalCode.UpdateError;
                throw;
            }
            return saveResult;
        }

        public async Task<bool> EntityExistsAsync(int id)
        {
            T? entityFound = await _db.Set<T>().FindAsync(id);
            if (entityFound == null)
            {
                return false;
            }

            return true;
        }

        #endregion Async Methods

        public IQueryable<T> OrderByText(IQueryable<T> data, SortOrder order, Expression<Func<T, string>> expression)
        {
            IQueryable<T> orderedData;
            if (order == SortOrder.ASC)
            {
                orderedData = data.OrderBy(expression);
            }
            else
            {
                orderedData = data.OrderByDescending(expression);
            }

            return orderedData;
        }

        public IQueryable<T> OrderByDate(IQueryable<T> data, SortOrder order, Expression<Func<T, DateTime>> expression)
        {
            IQueryable<T> orderedData;
            if (order == SortOrder.ASC)
            {
                orderedData = data.OrderBy(expression);
            }
            else
            {
                orderedData = data.OrderByDescending(expression);
            }

            return orderedData;
        }

        public async Task<List<T>> TakeAndSkipAsync(IQueryable<T> data, int pageSize, int pageIndex)
        {
            //List<T> paginatedList = new List<T>();

            //if (data == null || data.Count() <= 0)
            //    return paginatedList;

            //if (pageSize == 0 && pageIndex == 0)
            //    return paginatedList;

            int numRowSkipped = pageSize * (pageIndex - 1);

            List<T> paginated = await data.Skip(numRowSkipped).Take(pageSize).ToListAsync();

            return paginated;
        }
    }
}