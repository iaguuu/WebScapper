using System.Collections.Generic;
public interface IRepository<TEntity> where TEntity : class
{

    void BulkCopy(List<TEntity> entityList);

    List<TEntity> ListAll();
 
    //TO IMPLEMENT
    
    //void update(List<TEntity> entityList);
    //void insert(T entity);
    //void delete(T entity);
    // T GetByTicker(int id);

}