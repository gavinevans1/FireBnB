
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly ApplicationDbContext _dbContext;
        //comments weren't given and I don't want to type them out
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);

        }

        public virtual T Get(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null)
        {
            if (includes == null) //we are not joining other objects
            {
                if (!trackChanges) //is false
                {
                    return _dbContext.Set<T>()
                        .Where(predicate)
                        .AsNoTracking()
                        .FirstOrDefault();
                }
                else //we are tracking changes (which EF does by default)
                {
                    return _dbContext.Set<T>()
                        .Where(predicate)
                        .FirstOrDefault();
                }
            }

            else //we have includes to deal with
            {
                //includes = "Comma,Separate,Objects,Without,Spaces"
                IQueryable<T> queryable = _dbContext.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }

                if (!trackChanges) //is false
                {
                    return queryable
                        .Where(predicate)
                        .AsNoTracking()
                        .FirstOrDefault();
                }
                else //we are tracking changes (which EF does by default)
                {
                    return queryable
                        .Where(predicate)
                        .FirstOrDefault();
                }
            }
        }

        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null)
        {
            IQueryable<T> queryable = _dbContext.Set<T>();
            if (predicate != null && includes == null)
            {
                return _dbContext.Set<T>()
                    .Where(predicate)
                    .AsEnumerable();
            }

            //has includes
            else if (includes != null)
            {
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }
            }

            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return queryable.AsEnumerable();
                }

                else
                {
                    return queryable.OrderBy(orderBy).ToList();
                }
            }
            else
            {
                if (orderBy == null)
                {
                    return queryable
                        .Where(predicate)
                        .ToList();
                }
                else
                {
                    return queryable
                        .Where(predicate)
                        .OrderBy(orderBy).ToList();
                }
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null)
        {
            IQueryable<T> queryable = _dbContext.Set<T>();
            if (predicate != null && includes == null)
            {
                return _dbContext.Set<T>()
                    .Where(predicate)
                    .AsEnumerable();
            }

            //has includes
            else if (includes != null)
            {
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }
            }

            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return queryable.AsEnumerable();
                }

                else
                {
                    return await queryable.OrderBy(orderBy).ToListAsync();
                }
            }
            else
            {
                if (orderBy == null)
                {
                    return await queryable
                        .Where(predicate)
                        .ToListAsync();
                }
                else
                {
                    return await queryable
                        .Where(predicate)
                        .OrderBy(orderBy).ToListAsync();
                }
            }
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null)
        {
            if (includes == null) //we are not joining other objects
            {
                if (!trackChanges) //is false
                {
                    return await _dbContext.Set<T>()
                        .Where(predicate)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                }
                else //we are tracking changes (which EF does by default)
                {
                    return await _dbContext.Set<T>()
                        .Where(predicate)
                        .FirstOrDefaultAsync();
                }
            }

            else //we have includes to deal with
            {
                //includes = "Comma,Separate,Objects,Without,Spaces"
                IQueryable<T> queryable = _dbContext.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }

                if (!trackChanges) //is false
                {
                    return queryable
                        .Where(predicate)
                        .AsNoTracking()
                        .FirstOrDefault();
                }
                else //we are tracking changes (which EF does by default)
                {
                    return queryable
                        .Where(predicate)
                        .FirstOrDefault();
                }
            }
        }

        // Increment and Decrement Shopping Cart
        

        //virtual used to modify method, property, indexer, and allows for it to be overridden
        public virtual T GetById(int? id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public ApplicationUser GetById(string id)
        {
            return _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        }
        public IEnumerable<Amenity> GetAmenitiesByPropertyId(int propertyId)
        {
            return _dbContext.PropertyAmenities
                           .Where(pa => pa.PropertyId == propertyId)
                           .Select(pa => pa.Amenity)
                           .ToList();
        }

        public DateTime? GetEarliestCheckinDate(int propertyId)
        {
            return _dbContext.Bookings
                           .Where(b => b.PropertyId == propertyId)
                           .OrderBy(b => b.Checkin)
                           .Select(b => b.Checkin)
                           .FirstOrDefault();
        }



        public IEnumerable<Property> SearchProperties(string searchQuery, DateTime? checkIn, DateTime? checkOut, int? guestNumber, decimal? costPerNight, List<int> amenityIds, int? bedroomCount, int? bathroomCount)
        {
            // Start with querying all properties
            IQueryable<Property> query = _dbContext.Properties;

            // Include related entities to avoid lazy loading
            query = query.Include(p => p.Location);

            // Filter by search query (if provided)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.Location.City.CityName.Contains(searchQuery)
                         || p.Location.County.CountyName.Contains(searchQuery)
                         || p.Location.State.StateName.Contains(searchQuery)
                         || p.Location.Address.Contains(searchQuery)
                         || p.Location.Zipcode.Contains(searchQuery));
            }

            // Filter by check-in date and check-out date (if provided)
            if (checkIn.HasValue && checkOut.HasValue)
            {
                // Get the list of property IDs that have bookings overlapping with the specified dates
                var bookedPropertyIds = _dbContext.Bookings
                    .Where(b =>
                        (b.Checkin < checkOut && b.Checkout > checkIn)
                        || (b.Checkin >= checkIn && b.Checkin < checkOut)
                        || (b.Checkout > checkIn && b.Checkout <= checkOut))
                    .Select(b => b.PropertyId)
                    .Distinct();

                // Filter out properties that have bookings overlapping with the specified dates
                query = query.Where(p => !bookedPropertyIds.Contains(p.Id));
            }

            // Filter by guest count (if provided)
            if (guestNumber.HasValue)
            {
                query = query.Where(p => p.GuestMax >= guestNumber);
            }

            // Filter by cost per night
            if (costPerNight.HasValue)
            {
                // Convert decimal to float for comparison
                float costPerNightFloat = (float)costPerNight.Value;

                // Join PropertyNightlyPrice and PriceRange to get the nightly rates within the specified date range
                query = query.Join(_dbContext.PropertyNightlyPrices,
                                    p => p.Id,
                                    pnp => pnp.PropertyId,
                                    (p, pnp) => new { Property = p, Price = pnp })
                             .Join(_dbContext.PriceRanges,
                                    pnp => pnp.Price.PriceRangeId,
                                    pr => pr.Id,
                                    (pnp, pr) => new { Property = pnp.Property, Price = pnp.Price, PriceRange = pr })
                             .Where(x => x.Price.Rate <= costPerNightFloat && x.PriceRange.StartDate <= checkIn && x.PriceRange.EndDate >= checkOut)
                             .Select(x => x.Property)
                             .Distinct();
            }

            // Filter by amenities
            if (amenityIds != null && amenityIds.Any())
            {
                // Join Property and PropertyAmenity to filter properties based on amenities
                query = query.Join(_dbContext.PropertyAmenities,
                                    p => p.Id,
                                    pa => pa.PropertyId,
                                    (p, pa) => new { Property = p, PropertyAmenity = pa })
                             .Where(x => amenityIds.Contains(x.PropertyAmenity.AmenityId))
                             .Select(x => x.Property)
                             .Distinct();
            }

            // Filter by bedroom count 
            if (bedroomCount.HasValue)
            {
                query = query.Where(p => p.BedroomNum >= bedroomCount);
            }

            // Filter by bathroom count
            if (bathroomCount.HasValue)
            {
                query = query.Where(p => p.BathroomNum >= bathroomCount);
            }

            // Execute the query and return the results
            return query.ToList();
        }

        public void Update(T entity)
        {
            //for track changes I'm flagging modified to the system
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}