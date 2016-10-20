/* ****************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015 Colin D. Green (colin.green1@gmail.com)
 *
 * This software is issued under the MIT License.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace BreadPlayer.Tags.ID3
{
    /// <summary>
    /// A memory backed stream that stores byte data in blocks, this gives improved performance over System.IO.MemoryStream
    /// in some circumstances.
    /// 
    /// MemoryStream is backed by a single byte array, hence if the capacity is reached a new byte array must be instantiated 
    /// and the existing data copied across. In contrast, MemoryBlockStream grows in blocks and therefore avoids copying and 
    /// re-instantiating large byte arrays.
    /// 
    /// Also note that by using a sufficiently small block size the blocks will avoid being placed onto the large object heap,
    ///  with various benefits, e.g. avoidance/mitigation of memory fragmentation.
    /// </summary>
    public class MemoryBlockStream : Stream
    {
        #region Instance Fields

        readonly int _blockSize;

        /// <summary>
        /// Indicates if the stream is open.
        /// </summary>
        bool _isOpen;
        /// <summary>
        /// The read/write position within the stream; note that this can be moved back to write over existing data.
        /// </summary>
        int _position;
        /// <summary>
        /// MemoryBlockStream length. Indicates where the end of the stream is.
        /// </summary>
        int _length;

        List<byte[]> _blockList;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the default block size.
        /// </summary>
        public MemoryBlockStream() : this(8192) { }

        /// <summary>
        /// Construct with the specified block size.
        /// </summary>
        /// <param name="blockSize">The block size to use.</param>
        public MemoryBlockStream(int blockSize)
        {
            _isOpen = true;
            _position = 0;
            _length = 0;
            _blockSize = blockSize;
            _blockList = new List<byte[]>(100);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the MemoryBlockStream current capacity.
        /// </summary>
        private int Capacity
        {
            get { return _blockList.Count * _blockSize; }
        }

        #endregion

        #region MemoryBlockStream Overrides [Properties]

        /// <summary>
        /// Gets a flag flag that indicates if the stream is readable (always true for MemoryBlockStream while the stream is open).
        /// </summary>
        public override bool CanRead
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Gets a flag flag that indicates if the stream is seekable (always true for MemoryBlockStream while the stream is open).
        /// </summary>
        public override bool CanSeek
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Gets a flag flag that indicates if the stream is seekable (always true for MemoryBlockStream while the stream is open).
        /// </summary>
        public override bool CanWrite
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Gets or sets the current stream position.
        /// </summary>
        public override long Position
        {
            get
            {
                if (!_isOpen)
                {
                    throw new ObjectDisposedException("MemoryBlockStream is closed.");
                }
                return (long)(this._position);
            }
            set
            {
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value", "Number must be either non-negative and less than or equal to Int32.MaxValue or -1.");
                }

                if (!this._isOpen)
                {
                    throw new ObjectDisposedException("MemoryBlockStream is closed.");
                }

                if (value > (long)int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value", "MemoryBlockStream length must be non-negative and less than 2^31-1.");
                }

                if (value > _length)
                {
                    EnsureCapacity((int)value);
                }

                this._position = (int)value;
            }
        }

        /// <summary>
        /// Gets the current stream length.
        /// </summary>
        public override long Length
        {
            get
            {
                if (!_isOpen)
                {
                    throw new ObjectDisposedException("MemoryBlockStream is closed.");
                }
                return (long)(this._length);
            }
        }

        #endregion

        #region MemoryBlockStream Overrides [Methods]

        /// <summary>
        /// Reads a block of bytes from the stream and writes the data to a buffer.
        /// </summary>
        /// <param name="buffer">The byte array to read bytes into.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing data from the current stream.</param>
        /// <param name="count">The maximum number of bytes to read. </param>
        /// <returns>The total number of bytes written into the buffer. This can be less than the number of bytes requested if that number of
        /// bytes are not currently available, or zero if there are no bytes to read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Basic checks.
            if (null == buffer) throw new ArgumentNullException("buffer", "Buffer cannot be null.");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
            if (count < 0) throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
            if ((buffer.Length - offset) < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }
            if (!this._isOpen) throw new ObjectDisposedException("MemoryBlockStream is closed.");

            // Test for end of stream (or beyond end)
            if (_position >= _length)
            {
                return 0;
            }

            // Read bytes into the buffer.
            int blockIdx = _position / _blockSize;
            int blockOffset = _position % _blockSize;
            return ReadInner(buffer, offset, count, blockIdx, blockOffset);
        }

        /// <summary>
        /// Reads a byte from the stream.
        /// </summary>
        /// <returns>The byte cast to a Int32, or -1 if the end of the stream has been reached.</returns>
        public override int ReadByte()
        {
            if (!this._isOpen) throw new ObjectDisposedException("MemoryBlockStream is closed.");

            // Test for end of stream (or beyond end).
            if (_position >= _length)
            {
                return -1;
            }

            // Read byte.
            int blkIdx = _position / _blockSize;
            int blkOffset = _position++ % _blockSize;
            return _blockList[blkIdx][blkOffset];
        }

        /// <summary>
        /// Writes a block of bytes into the stream.
        /// </summary>
        /// <param name="buffer">The byte data to write into the stream</param>
        /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the current stream.</param>
        /// <param name="count">The maximum number of bytes to write. </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // Basic checks.
            if (null == buffer) throw new ArgumentNullException("buffer", "Buffer cannot be null.");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
            if (count < 0) throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
            if ((buffer.Length - offset) < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }
            if (!this._isOpen) throw new ObjectDisposedException("MemoryBlockStream is closed.");

            if (0 == count)
            {   // Nothing to do.
                return;
            }

            // Determine new position (post write).
            int endPos = _position + count;
            // Check for overflow 
            if (endPos < 0) throw new IOException("MemoryBlockStream was too long.");

            // Ensure there are enough blocks ready to write all of the provided data into.
            EnsureCapacity(endPos);

            // Write the bytes into the stream.
            int blockIdx = _position / _blockSize;
            int blockOffset = _position % _blockSize;
            WriteInner(buffer, offset, count, blockIdx, blockOffset);
        }
        
        /// <summary>
        /// Writes a byte to the stream at the current position.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        public override void WriteByte(byte value)
        {
            if (!this._isOpen) throw new ObjectDisposedException("MemoryBlockStream is closed.");

            // Determine new position (post write).
            int endPos = _position + 1;
            // Check for overflow 
            if (endPos < 0) throw new IOException("MemoryBlockStream was too long.");

            // Ensure there is capacity to write the byte into.
            EnsureCapacity(endPos);

            // Write the byte into the stream.
            int blkIdx = _position / _blockSize;
            int blkOffset = _position % _blockSize;
            _blockList[blkIdx][blkOffset] = value;

            // Update state.
            _position++;
            if (_position > _length)
            {
                _length = _position;
            }
        }

        /// <summary>
        /// Overrides MemoryBlockStream.Flush so that no action is performed.
        /// </summary>
        public override void Flush()
        {   // Memory based stream with nothing to flush; Do nothing.
        }

        /// <summary>
        /// Sets the position within the stream to the specified value.
        /// </summary>
        /// <param name="offset">The new position within the stream. This is relative to the loc parameter, and can be positive or negative. </param>
        /// <param name="origin">A value of type SeekOrigin, which acts as the seek reference point.</param>
        /// <returns>The new position within the stream, calculated by combining the initial reference point and the offset.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!this._isOpen) throw new ObjectDisposedException("MemoryBlockStream is closed.");
            if (offset > (long)int.MaxValue) throw new ArgumentOutOfRangeException("offset", "MemoryBlockStream length must be non-negative and less than 2^31-1.");

            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        if (offset < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");
                        _position = (int)offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        int newPos = unchecked(_position + (int)offset);
                        if (newPos < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");
                        _position = newPos;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        int newPos = unchecked(_length + (int)offset);
                        if (newPos < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");
                        _position = newPos;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid seek origin.");
                    }
            }
            return _position;
        }

        /// <summary>
        /// Sets the length of the stream to the specified value.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            if (value < 0 || value > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("value", "MemoryBlockStream length must be non-negative and less than 2^31 - 1.");
            }

            int newLength = (int)value;
            if (newLength == _length)
            {   // Do nothing.
                return;
            }

            if (newLength > _length)
            {
                // Handle case where new length is beyond the current length.
                // Ensure that any existing capacity after _length is zeroed.
                ZeroSpareCapacity();

                // Grow the capacity to ensure _length is within the bounds of allocated space that can be read.
                // Note. newly creatde blocks are zeroed by default.
                EnsureCapacity(newLength);
            }
            else if (newLength < _length)
            {
                _length = newLength;
            }

            // 'Snap back' the position. This is done to mimic the behaviour of MemoryStream, although the reason for doing this is 
            // unclear since setting Position directly allows a position beyond the end of the stream.
            if (_position > newLength)
            {
                _position = newLength;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the Position property.
        /// </summary>
        /// <returns>A new byte array.</returns>
        public byte[] ToArray()
        {
            // Allocate new byte array.
            byte[] buff = new byte[_length];
            int buffIdx = 0;

            // Calc number of full blocks.
            int fullBlockCount = _length / _blockSize;

            // Loop full blocks, copying them into buff as we go.
            for (int i = 0; i < fullBlockCount; i++)
            {
                byte[] blk = _blockList[i];
                Array.Copy(blk, 0, buff, buffIdx, _blockSize);
                buffIdx += _blockSize;
            }

            // Handle final block possibly/probably partially filled.
            int tailCount = _length % _blockSize;
            if (0 != tailCount)
            {
                byte[] blk = _blockList[fullBlockCount];
                Array.Copy(blk, 0, buff, buffIdx, tailCount);
            }

            return buff;
        }

        /// <summary>
        /// Remove excess blocks from the block list.
        /// </summary>
        public void Trim()
        {
            int currBlockCount = _blockList.Count;
            int newBlockCount = _length / _blockSize;
            if (0 != (_length % _blockSize))
            {
                newBlockCount++;
            }

            if (newBlockCount < currBlockCount)
            {
                _blockList.RemoveRange(newBlockCount, currBlockCount - newBlockCount);
            }
        }

        #endregion

        #region Private Methods

        private void EnsureCapacity(int value)
        {
            if (value > this.Capacity)
            {
                int blockCount = (value / _blockSize) + 1;
                int createCount = blockCount - _blockList.Count;
                for (int i = 0; i < createCount; i++)
                {
                    _blockList.Add(new byte[_blockSize]);
                }
            }
        }

        /// <summary>
        /// Read bytes from the memory stream into the provided buffer, starting at the specified block index and intra-block offset..
        /// </summary>
        private int ReadInner(byte[] buff, int offset, int count, int blockIdx, int blockOffset)
        {
            // Determine how many bytes will be read (based on requested bytes versus the number available).
            int readCount = Math.Min(count, _length - _position);
            if (0 == readCount)
            {
                return 0;
            }

            int remaining = readCount;
            int tgtOffset = offset;
            int blkIdx = blockIdx;
            int blkOffset = blockOffset;

            for (;;)
            {
                // Get handle on target block.
                byte[] blk = _blockList[blkIdx];

                // Determine how many bytes to write into this block.
                int copyCount = Math.Min(_blockSize - blkOffset, remaining);

                // Write bytes into the buffer.
                Array.Copy(blk, blkOffset, buff, tgtOffset, copyCount);

                // Test for completion.
                remaining -= copyCount;
                if (0 == remaining)
                {   // All bytes have been copied. 
                    break;
                }

                // Update state.
                tgtOffset += copyCount;
                blkIdx++;
                blkOffset++;
                blkOffset = 0;
            }

            _position += readCount;
            return readCount;
        }

        /// <summary>
        /// Write bytes into the memory stream, starting at the specified block index and intra-block offset.
        /// </summary>
        private void WriteInner(byte[] buff, int offset, int count, int blockIdx, int blockOffset)
        {
            int remaining = count;
            int srcOffset = offset;
            int blkIdx = blockIdx;
            int blkOffset = blockOffset;

            for (;;)
            {
                // Get handle on target block.
                byte[] blk = _blockList[blkIdx];

                // Determine how many bytes to write into this block.
                int copyCount = Math.Min(_blockSize - blkOffset, remaining);

                // Write bytes into the block.
                Array.Copy(buff, srcOffset, blk, blkOffset, copyCount);

                // Test for completion.
                remaining -= copyCount;
                if (0 == remaining)
                {   // All bytes have been copied. 
                    break;
                }

                // Update state.
                srcOffset += copyCount;
                blkIdx++;
                blkOffset++;
                blkOffset = 0;
            }

            _position += count;
            if (_position > _length)
            {
                _length = _position;
            }
        }

        /// <summary>
        /// Ensure that any existing capacity after _length is zeroed.
        /// </summary>
        private void ZeroSpareCapacity()
        {
            // Handle tail of the first block to zero/reset.
            int blockIdx = _length / _blockSize;
            int blockOffset = _length % _blockSize;
            byte[] blk = _blockList[blockIdx];
            Array.Clear(blk, blockOffset, _blockSize - blockOffset);

            // Zero any further blocks.
            for (blockIdx++; blockIdx < _blockList.Count; blockIdx++)
            {
                blk = _blockList[blockIdx];
                Array.Clear(blk, 0, _blockSize);
            }
        }
        #endregion
    }
}