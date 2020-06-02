using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C5;

namespace ScenarioTests.C5_Extensions
{
    public interface IIndex<Q, R>
    {
        ICollectionValue<R> this[Q x] { get; }

        IEnumerator<R> GetEnumerator();

        IEnumerable<R> AsEnumerable();
    }

    // An index maker has a name, it supports adding and removing items
    // from the index, and looking up items by key (here of type
    // Object).
    public abstract class IndexMaker<T> : IIndex<object, T>
    {
        public string Name { get; }
        public abstract bool Add(T item);
        public abstract bool Remove(T item);
        public abstract ICollectionValue<T> this[object key] { get; }

        public abstract IEnumerator<T> GetEnumerator();

        public abstract IEnumerable<T> AsEnumerable();

        public IndexMaker(string name)
        {
            Name = name;
        }
    }

    // The implementation of an index maker consists of a function to
    // map an item of type T to the index key type Q, and a dictionary
    // that maps an index key (of type Q) to a set of items (each of
    // type T).
    public class IndexMaker<T, Q> : IndexMaker<T>
        where T : class
        where Q : IComparable<Q>
    {
        private readonly Func<T, Q> _fun;
        private readonly TreeDictionary<Q, C5.HashSet<T>> _dictionary;

        public IndexMaker(string name, Func<T, Q> fun) : base(name)
        {
            _fun = fun;
            _dictionary = new TreeDictionary<Q, C5.HashSet<T>>();
        }

        public override bool Add(T item)
        {
            var key = _fun(item);
            if (!_dictionary.Contains(key))
            {
                _dictionary.Add(key, new C5.HashSet<T>(C5.EqualityComparer<T>.Default));
            }

            return _dictionary[key].Add(item);
        }

        public override bool Remove(T item)
        {
            var key = _fun(item);

            return _dictionary.Contains(key) && _dictionary[key].Remove(item);
        }

        public ICollectionValue<T> this[Q key] => _dictionary[key];

        public override ICollectionValue<T> this[object key] => _dictionary[(Q)key];

        public override IEnumerator<T> GetEnumerator()
        {
            foreach (var key in _dictionary.Keys)
            {
                foreach (var value in _dictionary[key])
                {
                    yield return value;
                }
            }
        }

        public override IEnumerable<T> AsEnumerable()
        {
            return _dictionary.Values.SelectMany(x=>x);
        }

        public override string ToString()
        {
            return _dictionary.ToString();
        }
    }

    // Weakly typed implementation of multiple indexes on a class T.  

    // The implementation is an array of index makers, each consisting
    // of the index's name and its implementation which supports adding
    // and removing T objects, and looking up T objects by the key
    // relevant for that index.
    public class Index<T> where T : class
    {
        private readonly IndexMaker<T>[] _indexMakers;

        public Index(params IndexMaker<T>[] indexMakers)
        {
            _indexMakers = indexMakers;
        }

        public bool Add(T item)
        {
            bool result = false;

            foreach (var indexMaker in _indexMakers)
            {
                result |= indexMaker.Add(item);
            }

            return result;
        }

        public bool Remove(T item)
        {
            bool result = false;
            foreach (var indexMaker in _indexMakers)
            {
                result |= indexMaker.Remove(item);
            }

            return result;
        }

        public IIndex<object, T> this[string name]
        {
            get
            {
                foreach (var indexMaker in _indexMakers)
                {
                    if (indexMaker.Name == name)
                    {
                        return indexMaker;
                    }
                }

                throw new Exception("Unknown index");
            }
        }

        // For debugging

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var indexMaker in _indexMakers)
            {
                sb.Append("\n----- ").Append(indexMaker.Name).Append("-----\n");
                sb.Append(indexMaker);
            }

            return sb.ToString();
        }
    }
}