using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cassie.git.module.commits;
using cassie.git.module.repo;

namespace cassie.git.module.tree
{
    public class EntryCollection<T> : ICollection<T> where T : TreeEntry
    {
        protected List<T> _innerArray;  //inner ArrayList object
        protected bool _IsReadOnly;       //flag for setting collection to read-only mode (not used in this example)
        public EntryCollection()
        {
            _innerArray = new List<T>();
        }

        /// <summary>
        /// Default accessor for the collection 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual T this[int index]
        {
            get
            {
                return (T)_innerArray[index];
            }
            set
            {
                _innerArray[index] = value;
            }
        }

        /// <summary>
        /// Number of elements in the collection
        /// </summary>
        public virtual int Count
        {
            get
            {
                return _innerArray.Count;
            }
        }

        /// <summary>
        /// Flag sets whether or not this collection is read-only
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
        }

        public void Add(T item)
        {
            _innerArray.Add(item);
        }

        public void Clear()
        {
            _innerArray.Clear();
        }

        public void Sort()
        {
            _innerArray.Sort();
        }
        public void Val(List<T> list)
        {

            _innerArray = list;
        }

        public bool Contains(T item)
        {
            //loop through the inner ArrayList
            foreach (T obj in _innerArray)
            {
                //compare the EntryBase UniqueId property
                if (obj.UniqueId == item.UniqueId)
                {
                    //if it matches return true
                    return true;
                }
            }

            //no match
            return false;
        }

        


        public void CopyTo(T[] array, int arrayIndex)
        {
            
        }

        public IEnumerator<T> GetEnumerator()
        {
            //return a custom enumerator object instantiated to use this EntryCollection 
            return new EntryEnumerator<T>(this);
        }

        public bool Remove(T item)
        {
            bool result = false;

            //loop through the inner array's indices
            for (int i = 0; i < _innerArray.Count; i++)
            {
                //store current index being checked
                T obj = (T)_innerArray[i];

                //compare the EntryBase UniqueId property
                if (obj.UniqueId == item.UniqueId)
                {
                    //remove item from inner ArrayList at index i
                    _innerArray.RemoveAt(i);
                    result = true;
                    break;
                }
            }

            return result;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            //return a custom enumerator object instantiated to use this EntryCollection 
            return new EntryEnumerator<T>(this);
        }
    }
}