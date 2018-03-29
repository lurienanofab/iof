using System;

namespace IOF.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public int ParentID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNumber { get; set; }
        public bool Active { get; set; }

        public string DisplayName
        {
            get { return $"{CategoryNumber} - {CategoryName}"; }
        }

        public double CategoryNumberToDouble()
        {
            string[] splitter = CategoryNumber.Split('.');

            if (splitter.Length == 0)
                throw new Exception($"Invalid CategoryNumber format.");

            if (string.IsNullOrEmpty(splitter[0]))
                throw new Exception("Invalid CategoryNumber: whole part must contain at least one digit.");

            if (!int.TryParse(splitter[0], out int whole))
                throw new Exception("Invalid CategoryNumber: whole part must be numeric.");

            if (splitter.Length == 1)
                return whole;

            if (splitter[1].Length > 3)
                throw new Exception("Invalid CategoryNmber: decimal part must not contain more than 3 digits.");

            if (!int.TryParse(splitter[1], out int frac))
                throw new Exception("Invalid CategoryNumber: decimal part must be numeric.");

            return whole + (frac * 0.001);
        }
    }
}
