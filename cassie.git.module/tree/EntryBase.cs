using System;
namespace cassie.git.module.tree
{
    /// <summary>
    /// Abstract base class for all business object in the Business Logic Layer
    /// </summary>
    public abstract class EntryBase
    {
        #region "Member Variables"

        protected Guid? _UniqueId;  //local member variable which stores the object's UniqueId

        #endregion

        #region "Constructors"

        /// <summary>
        /// Default constructor
        /// </summary>
        public EntryBase()
        {
            //create a new unique id for this business object
            _UniqueId = Guid.NewGuid();
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// UniqueId property for every business object
        /// </summary>
        public Guid? UniqueId
        {
            get
            {
                return _UniqueId;
            }
            set
            {
                _UniqueId = value;
            }
        }

        #endregion
    }
}