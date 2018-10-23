using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Data.Entity;


namespace Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbSet<TEntity> _dbSet;

        protected readonly DbContext _context;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        /// <summary>
        /// Obtém elemento por ID da DB.
        /// </summary>
        /// <param name="id">id <see cref="object"/></param>
        /// <returns>Entidade. <see cref="TEntity"/></returns>
        public TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// Obtém todos os elementos na BD.
        /// </summary>
        /// <param name="id">id <see cref="object"/></param>
        /// <returns>Entidade. <see cref="TEntity"/></returns>
        public IQueryable<TEntity> GetAllIQueryable()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        ///  Obtém todos os elementos da tabela da BD com condição "where".
        /// </summary>
        /// <param name="where">cláusula da condição.<see cref="Expression{Func{TEntity, bool}}"/></param>
        /// <returns>Lista de valores. <see cref="IEnumerable{TEntity}"/></returns>
        public IQueryable<TEntity> GetAllIQueryableWithCondition(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where).AsQueryable();
        }

        /// <summary>
        /// Obtém todos os elementos da tabela da BD.
        /// </summary>
        /// <returns>Lista de valores. <see cref="IEnumerable{TEntity}"/></returns>
        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        /// <summary>
        /// Obtém todos os elementos da tabela da BD com condição "where".
        /// </summary>
        /// <param name="where">cláusula da condição.<see cref="Expression{Func{TEntity, bool}}"/></param>
        /// <returns>Lista de valores. <see cref="IEnumerable{TEntity}"/></returns>
        public IEnumerable<TEntity> GetAllWithCondition(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where).ToList();
        }

        /// <summary>
        /// Obtém todos os elementos da tabela da BD com "join" a outra tabela.
        /// </summary>
        /// <param name="T">Entidade a efetuar o join.<see cref="Expression{Func{TEntity, object}}"/></param>
        /// <returns>Lista de valores.<see cref="IEnumerable{TEntity}"/></returns>
        public IEnumerable<TEntity> GetAllWithInclude(Expression<Func<TEntity, object>> T)
        {
            return _dbSet.Include(T).ToList();
        }

        /// <summary>
        ///  Obtém todos os elementos da tabela da BD com condição "where" e o "join" a outra tabela.
        /// </summary>
        /// <param name="T">Tabela a incluir no join <see cref="Expression{Func{TEntity, object}}"/></param>
        /// <param name="where">Cláusula where <see cref="Expression{Func{TEntity, bool}}"/></param>
        /// <returns>The <see cref="IEnumerable{TEntity}"/></returns>
        public IQueryable<TEntity> GetAllWithIncludeAndCondition(Expression<Func<TEntity, object>> T, Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where).Include(T);
        }

        /// <summary>
        ///  Obtém todos os elementos da tabela da BD com condição "where" e duplo "join" as outras tabelas.
        /// </summary>
        /// <param name="T">Tabela Join <see cref="Expression{Func{TEntity, object}}"/></param>
        /// <param name="E">Tabela Join<see cref="Expression{Func{TEntity, object}}"/></param>
        /// <param name="where">Clausula de condição<see cref="Expression{Func{TEntity, bool}}"/></param>
        /// <returns>The <see cref="IEnumerable{TEntity}"/></returns>
        public IEnumerable<TEntity> GetAllWithMultipleIncludeAndCondition(Expression<Func<TEntity, object>> T, Expression<Func<TEntity, object>> E, Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where).Include(T).Include(E).ToList();
        }

        /// <summary>
        /// Insere novo registo na BD.
        /// </summary>
        /// <param name="entity">The <see cref="TEntity"/></param>
        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// The Delete- Remove um registo na BD
        /// </summary>
        /// <param name="entity">The <see cref="TEntity"/></param>
        public virtual void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public IEnumerable<Object> FilterList(IEnumerable<Object> listObject, int skip, int take)
        {
            return listObject.Skip(skip).Take(take).ToList();
        }

        public TEntity GetEntity()
        {

            return _dbSet.FirstOrDefault();
        }
    }
}
