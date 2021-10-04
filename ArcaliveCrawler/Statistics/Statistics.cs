using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcaliveCrawler.Statistics
{
    public class Statistics : IEnumerable<string[]>
    {
        public int SizeofRow { get; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[] Characteristics { get; }
        private List<string[]> _values;

        public Statistics(int sizeofRow)
        {
            SizeofRow = sizeofRow;
            Characteristics = new string[sizeofRow];
            _values = new List<string[]>();
        }

        public Statistics(params string[] characteristics) : this(characteristics.Length)
        {
            if (characteristics == null || characteristics.Length <= 1)
                throw new ArgumentException();
            for (int i = 0; i < SizeofRow; i++)
            {
                Characteristics[i] = characteristics[i];
            }
        }

        public void AddRow(params object[] value)
        {
            if (value.Length != SizeofRow)
                throw new ArgumentException();
            _values.Add(value.Select(x => x.ToString()).ToArray());
        }

        public IEnumerable<string> ToStrings(bool withName = true, bool withDescription = true, bool withCharacteristics = true, string separator = ", ")
        {
            if (withName)
                yield return "//" + Name;
            if (withDescription)
                yield return "//" + Description;
            if (withCharacteristics)
                yield return String.Join(separator, Characteristics);
            foreach (var statistic in this)
            {
                var result = String.Join(separator, statistic.Select(x => x.ToString()));
                yield return result;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in ToStrings())
            {
                sb.AppendLine(s);
            }

            return sb.ToString();
        }

        public IEnumerator<string[]> GetEnumerator()
        {
            return ((IEnumerable<string[]>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_values).GetEnumerator();
        }
    }
}
