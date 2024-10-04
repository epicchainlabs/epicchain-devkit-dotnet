using EpicChain.SmartContract.Framework.Native;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_List : SmartContract
    {
        public static int TestCount(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            return some.Count;
        }

        public static string TestAdd(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            return EssentialLib.JsonSerialize(some);
        }

        public static string TestRemoveAt(int count, int removeAt)
        {
            if (removeAt >= count) throw new System.Exception("Invalid test parameters");

            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            some.RemoveAt(removeAt);
            return EssentialLib.JsonSerialize(some);
        }

        public static string TestClear(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            some.Clear();
            return EssentialLib.JsonSerialize(some);
        }

        public static int[] TestArrayConvert(int count)
        {
            List<int> some = new List<int>();
            for (int i = 0; i < count; i++)
            {
                some.Add(i);
            }

            return some;
        }
    }
}
