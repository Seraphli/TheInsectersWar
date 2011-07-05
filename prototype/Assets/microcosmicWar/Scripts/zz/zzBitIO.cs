
namespace zz
{
    //public class BitIO
    //{
    //}
    public struct IntBitIO
    {
        public static int getMask(int count)
        {
            switch (count)
            {
                case 0: return 0;

                case 1: return 1;
                case 2: return 3;
                case 3: return 7;
                case 4: return 0xf;

                case 5: return 0x1f;
                case 6: return 0x3f;
                case 7: return 0x7f;
                case 8: return 0xff;

                case 9: return 0x1ff;
                case 10: return 0x3ff;
                case 11: return 0x7ff;
                case 12: return 0xfff;

                case 13: return 0x1fff;
                case 14: return 0x3fff;
                case 15: return 0x7fff;
                case 16: return 0xffff;

                case 17: return 0x1ffff;
                case 18: return 0x3ffff;
                case 19: return 0x7ffff;
                case 20: return 0xfffff;

                case 21: return 0x1fffff;
                case 22: return 0x3fffff;
                case 23: return 0x7fffff;
                case 24: return 0xffffff;

                case 25: return 0x1ffffff;
                case 26: return 0x3ffffff;
                case 27: return 0x7ffffff;
                case 28: return 0xfffffff;

                case 29: return 0x1fffffff;
                case 30: return 0x3fffffff;
                case 31: return 0x7fffffff;
                //case 32: return unchecked((int)0xffffffff);
            }
            throw new System.InvalidOperationException();
        }

        int mDate;

        public int date
        {
            get { return mDate; }
            set { mDate = value; }
        }

        public IntBitIO(int date)
        {
            mDate = date;
        }

        public void write(byte writeData, int count)
        {
            mDate <<= count;
            mDate |= writeData & getMask(count);
        }

        public void write(short writeData, int count)
        {
            mDate <<= count;
            mDate |= writeData & getMask(count);
        }

        public void write(int writeData, int count)
        {
            mDate <<= count;
            mDate |= writeData & getMask(count);
        }

        public byte readToByte(int count)
        {
            byte lOut = (byte)(mDate & getMask(count));
            mDate >>= count;
            return lOut;
        }

        public short readToShort(int count)
        {
            short lOut = (short)(mDate & getMask(count));
            mDate >>= count;
            return lOut;
        }

        public int readToInt(int count)
        {
            int lOut = (mDate & getMask(count));
            mDate >>= count;
            return lOut;
        }
    }


    public struct LongBitIO
    {
        public static long getMask(int count)
        {
            switch (count)
            {
                case 0: return 0;

                case 1: return 1;
                case 2: return 3;
                case 3: return 7;
                case 4: return 0xf;

                case 5: return 0x1f;
                case 6: return 0x3f;
                case 7: return 0x7f;
                case 8: return 0xff;

                case 9: return 0x1ff;
                case 10: return 0x3ff;
                case 11: return 0x7ff;
                case 12: return 0xfff;

                case 13: return 0x1fff;
                case 14: return 0x3fff;
                case 15: return 0x7fff;
                case 16: return 0xffff;

                case 17: return 0x1ffff;
                case 18: return 0x3ffff;
                case 19: return 0x7ffff;
                case 20: return 0xfffff;

                case 21: return 0x1fffff;
                case 22: return 0x3fffff;
                case 23: return 0x7fffff;
                case 24: return 0xffffff;

                case 25: return 0x1ffffff;
                case 26: return 0x3ffffff;
                case 27: return 0x7ffffff;
                case 28: return 0xfffffff;

                case 29: return 0x1fffffff;
                case 30: return 0x3fffffff;
                case 31: return 0x7fffffff;
                case 32: return 0xffffffff;
            }
            throw new System.InvalidOperationException();
        }

        long mDate;

        public long date
        {
            get { return mDate; }
            set { mDate = value; }
        }

        public LongBitIO(long date)
        {
            mDate = date;
        }

        //public void write(byte writeData, int count)
        //{
        //    mDate <<= count;
        //    mDate |= writeData & getMask(count);
        //}

        //public void write(short writeData, int count)
        //{
        //    mDate <<= count;
        //    mDate |= writeData & getMask(count);
        //}

        public void write(int writeData, int count)
        {
            mDate <<= count;
            mDate |= writeData & getMask(count);
        }

        //public byte readToByte(int count)
        //{
        //    byte lOut = (byte)(mDate & getMask(count));
        //    mDate >>= count;
        //    return lOut;
        //}

        //public short readToShort(int count)
        //{
        //    short lOut = (short)(mDate & getMask(count));
        //    mDate >>= count;
        //    return lOut;
        //}

        public int readToInt(int count)
        {
            int lOut = (int)(mDate & getMask(count));
            mDate >>= count;
            return lOut;
        }

        //public int part1
        //{
        //    get { return (int)(mDate & 0xffffffff); }
        //    set { mDate &= value | 0xffffffff00000000; }
        //}

        //public int part2
        //{
        //    get { return (int)(mDate>>32); }
        //    set { mDate &= ((((long)value) << 32) | 0xffffffff); }
        //}
    }
}