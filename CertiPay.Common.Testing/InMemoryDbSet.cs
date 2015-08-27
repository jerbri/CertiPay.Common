namespace CertiPay.Common.Testing
{
    // Borrowed from NuGet's WebBackgrounder OSS project
    // Augmented with Async support from https://msdn.microsoft.com/en-us/data/dn314429.aspx

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class InMemoryDbSet<TEntity> : IDbAsyncEnumerable<TEntity>, IDbSet<TEntity> where TEntity : class
    {
        private readonly HashSet<TEntity> _set;
        private readonly IQueryable<TEntity> _queryableSet;

        public InMemoryDbSet()
            : this(Enumerable.Empty<TEntity>())
        {
        }

        public InMemoryDbSet(IEnumerable<TEntity> entities)
        {
            _set = new HashSet<TEntity>();
            foreach (var entity in entities)
            {
                _set.Add(entity);
            }
            _queryableSet = _set.AsQueryable();
        }

        public TEntity Add(TEntity entity)
        {
            _set.Add(entity);
            return entity;
        }

        public TEntity Attach(TEntity entity)
        {
            _set.Add(entity);
            return entity;
        }

        public TEntity Remove(TEntity entity)
        {
            _set.Remove(entity);
            return entity;
        }

        public virtual TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Create()
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<TEntity> Local
        {
            get { return new ObservableCollection<TEntity>(_queryableSet); }
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _queryableSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryableSet.GetEnumerator();
        }

        public Type ElementType
        {
            get { return _queryableSet.ElementType; }
        }

        public Expression Expression
        {
            get { return _queryableSet.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return new TestDbAsyncQueryProvider<TEntity>(_queryableSet.Provider); }
        }

        public IDbAsyncEnumerator<TEntity> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerable<TEntity>(_queryableSet).GetAsyncEnumerator();
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }

    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestDbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestDbAsyncQueryProvider<T>(this); }
        }
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public T Current
        {
            get { return _inner.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}