using System.Collections;
using System.Collections.Generic;
using System.Text;
using DefensiveProgrammingFramework;
using TorrentClient.Exceptions;
using TorrentClient.Extensions;

namespace TorrentClient.BEncoding
{
    /// <summary>
    /// The BEncoded list.
    /// </summary>
    public class BEncodedList : BEncodedValue, IList<BEncodedValue>, IEnumerable
    {
        #region Private Fields

        /// <summary>
        /// The list.
        /// </summary>
        private List<BEncodedValue> list;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedList"/> class.
        /// </summary>
        public BEncodedList()
            : this(new List<BEncodedValue>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedList"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public BEncodedList(int capacity)
            : this(new List<BEncodedValue>(capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedList" /> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public BEncodedList(IEnumerable<BEncodedValue> list)
        {
            list.CannotContainOnlyNull();

            this.list = new List<BEncodedValue>(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEncodedList"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BEncodedList(List<BEncodedValue> value)
        {
            value.CannotBeNull();

            this.list = value;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="System.Collections.Generic.ICollection`1" />.</value>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value>true if the <see cref="System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</value>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The BEncoded value.</returns>
        public BEncodedValue this[int index]
        {
            get
            {
                index.MustBeGreaterThanOrEqualTo(0);

                return this.list[index];
            }

            set
            {
                this.list[index] = value;
            }
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Adds an item to the <see cref="System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="System.Collections.Generic.ICollection`1" />.</param>
        public void Add(BEncodedValue item)
        {
            item.CannotBeNull();

            this.list.Add(item);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void AddRange(IEnumerable<BEncodedValue> collection)
        {
            collection.CannotContainOnlyNull();

            this.list.AddRange(collection);
        }

        /// <summary>
        /// Removes all items from the <see cref="System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(BEncodedValue item)
        {
            item.CannotBeNull();

            return this.list.Contains(item);
        }

        /// <summary>
        /// Copies the automatic.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(BEncodedValue[] array, int arrayIndex)
        {
            array.CannotBeNull();
            arrayIndex.MustBeGreaterThanOrEqualTo(0);

            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Encodes the list to a byte[]
        /// </summary>
        /// <param name="buffer">The buffer to encode the list to</param>
        /// <param name="offset">The offset to start writing the data at</param>
        /// <returns>The number of bytes encoded.</returns>
        public override int Encode(byte[] buffer, int offset)
        {
            buffer.CannotBeNullOrEmpty();
            offset.MustBeGreaterThanOrEqualTo(0);

            int written = 0;

            buffer[offset] = (byte)'l'; // lists start with l

            written++;

            for (int i = 0; i < this.list.Count; i++)
            {
                written += this.list[i].Encode(buffer, offset + written);
            }

            buffer[offset + written] = (byte)'e'; // lists end with e

            written++;

            return written;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            BEncodedList other = obj as BEncodedList;

            if (other == null)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    if (!this.list[i].Equals(other.list[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<BEncodedValue> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int result = 0;

            for (int i = 0; i < this.list.Count; i++)
            {
                result ^= this.list[i].GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(BEncodedValue item)
        {
            item.CannotBeNull();

            return this.list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, BEncodedValue item)
        {
            index.MustBeGreaterThanOrEqualTo(0);
            item.CannotBeNull();

            this.list.Insert(index, item);
        }

        /// <summary>
        /// Returns the size of the list in bytes
        /// </summary>
        /// <returns>The length in bytes.</returns>
        public override int LengthInBytes()
        {
            int length = 0;

            length += 1;   // Lists start with 'l'

            for (int i = 0; i < this.list.Count; i++)
            {
                length += this.list[i].LengthInBytes();
            }

            length += 1;   // Lists end with 'e'

            return length;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(BEncodedValue item)
        {
            item.CannotBeNull();

            return this.list.Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.Encode());
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Decodes a BEncodedList from the given StreamReader
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal override void DecodeInternal(RawReader reader)
        {
            reader.CannotBeNull();

            if (reader.ReadByte() != 'l')
            {
                throw new BEncodingException("Invalid data found. Aborting");
            }

            while (reader.PeekByte() != -1 &&
                   reader.PeekByte() != 'e')
            {
                this.list.Add(BEncodedValue.Decode(reader));
            }

            if (reader.ReadByte() != 'e')
            {
                throw new BEncodingException("Invalid data found. Aborting");
            }
        }

        #endregion Internal Methods
    }
}
