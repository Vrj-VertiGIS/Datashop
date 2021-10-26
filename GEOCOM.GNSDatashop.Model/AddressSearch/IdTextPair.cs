using System;

namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public class IdTextPair : IComparable<IdTextPair>
    {
        public string Id;

        public string Text;

        public bool IsText;

        public string OrderValue;

        public bool OrderDesc;

        #region IComparable<IdTextPair> Members

        public int CompareTo(IdTextPair other)
        {
            if (other != null)
            {
                if (!IsText && !other.IsText)
                {
                    // numeric compare if none of them are text
                    float num1, num2;
                    if (float.TryParse(OrderValue, out num1) && float.TryParse(other.OrderValue, out num2))
                    {
                        if (!OrderDesc)
                        {
                            return num1.CompareTo(num2);
                        }
                        else
                        {
                            return num2.CompareTo(num1);
                        }
                    }
                    else
                    {
                        return OrderText(other);
                    }
                }
                else
                {
                    return OrderText(other);
                }
            }

            return -1;
        }

        private int OrderText(IdTextPair other)
        {
            var result = OrderValue.CompareTo(other.OrderValue);

            if (OrderDesc)
                result *= -1;

            return result;
        }

        #endregion
    }
}