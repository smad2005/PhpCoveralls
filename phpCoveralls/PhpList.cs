using System.Collections.Generic;

namespace phpCoveralls
{
    public class PhpList : List<int?>
    {
        public new int? this[int index]
        {
            get { return base[index]; }
            set
            {
                Normalize(index);
                --index;
                base[index] = value;
            }
        }

        public void Normalize(int index)
        {
            --index;
            while (index >= Count)
            {
                Add(null);
            }
        }
    }
}