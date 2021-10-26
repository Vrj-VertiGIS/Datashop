using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    [DataContract]
	public class PlotdefinitionKey
	{
        // constructor
        public PlotdefinitionKey()
        {
        }

        public PlotdefinitionKey(int mediumCode, string template)
        {
            MediumCode = mediumCode;
            Template = template;
        }

        [DataMember]
        public virtual int MediumCode { get; set; }

        [DataMember]
        public virtual string Template { get; set; }

        // save new extent on db
		public override bool Equals(object other)
		{
			if (this == other) return true;

			PlotdefinitionKey otherKey = other as PlotdefinitionKey;
			if (otherKey == null) return false; // null or not a cat

			if (MediumCode != otherKey.MediumCode) return false;
			if (!Template.Equals(otherKey.Template)) return false;

			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result;
				result = MediumCode.GetHashCode();
				result = (29 * result) + Template.GetHashCode();
				return result;
			}
		}
	}
}