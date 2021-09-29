using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaliveForm
{
    public class Arcacon
    {
        internal int dataid;
        internal string address;

        public Arcacon()
        {
            dataid = -1;
            address = string.Empty;
        }
        public Arcacon(string add)
        {
            dataid = -1;
            address = add;
        }

        public Arcacon(int id, string add)
        {
            dataid = id;
            address = add;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == (obj as Arcacon).GetHashCode();
        }

        public override int GetHashCode()
        {
            return dataid * 123 + address.Length * 321;
        }
    }
    public class ArcaconPack
    {
        public int id;
        public List<Arcacon> arcacons;

        public ArcaconPack()
        {
            id = -1;
            arcacons = new List<Arcacon>();
        }
        public ArcaconPack(int id)
        {
            this.id = id;
            arcacons = new List<Arcacon>();
        }
    }
}
