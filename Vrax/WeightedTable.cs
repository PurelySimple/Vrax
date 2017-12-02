using System;
using System.Collections.Generic;
using System.Text;

namespace LudumDare40.Vrax
{
    public class WeightedTable<T>
    {
        private Func<T,int> WeightSelector { get; set; }
        private List<T> Items { get; set; }

        public int Sum { get; set; }

        public WeightedTable(Func<T,int> weightSelector, IEnumerable<T> collection)
        {
            WeightSelector = weightSelector;
            Items = new List<T>(collection);

            foreach (var item in Items)
            {
                Sum += weightSelector.Invoke(item);
            }
        }

        public T Select(int random)
        {
            random = random % Sum;

            int sum = 0;
            int current = 0;

            foreach (var item in Items)
            {
                current = WeightSelector.Invoke(item);
                if (random < sum + current)
                    return item;

                sum += current;
            }

            throw new Exception("Invalid weighted table state");
        }
    }
}
