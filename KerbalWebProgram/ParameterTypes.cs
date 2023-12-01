using System;
using System.Collections.Generic;
using System.Text;

namespace KerbalWebProgram
{
    public class ParameterValidationException: Exception
    {
        public ParameterValidationException() { }
        public ParameterValidationException(string message) : base(message) { }
    }

    public abstract class KWPParameterType
    {
        public abstract string Name { get; set;  }
        public abstract string Description { get; set; }
        public abstract string Type { get; }
        public abstract bool IsRequired { get; set; }

        public bool validate(dynamic value)
        {
            if (value == null && this.IsRequired == true) throw new ParameterValidationException("Value is required");
            else if (value == null && this.IsRequired == false) return true;
            return this.validateInternal(value);
        }

        protected abstract bool validateInternal(dynamic value);

        public abstract Dictionary<string, object> getSerializedMetadata();

        public abstract dynamic transformValue(dynamic value);
    }

    public class StringParameter : KWPParameterType
    {
        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Type => "string";

        public override bool IsRequired { get; set; }

        public int? minLength { get; set; }
        public int? maxLength { get; set; }

        public StringParameter(string name, string description, bool isRequired, int? minLength = null, int? maxLength = null)
        {
            this.Name = name;
            this.Description = description;
            this.IsRequired = isRequired;
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        protected override bool validateInternal(dynamic value)
        {
            if(value is not string) throw new ParameterValidationException("Value must be of type string");
            if(minLength != null && ((string)value).Length < minLength)
            {
                if (maxLength != null) throw new ParameterValidationException(String.Format("Value must be between {0} and {1} characters", minLength, maxLength));
                else throw new ParameterValidationException(String.Format("Value must be at least {0} characters", minLength));
            }
            if(maxLength != null && ((string)value).Length > maxLength)
            {
                if (minLength != null) throw new ParameterValidationException(String.Format("Value must be between {0} and {1} characters", minLength, maxLength));
                else throw new ParameterValidationException(String.Format("Value must be at most {0} characters", maxLength));
            }
            return true;
        }

        public override Dictionary<string, object> getSerializedMetadata()
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            if (minLength != null) metadata.Add("minLength", minLength);
            if (maxLength != null) metadata.Add("maxLength", maxLength);
            return metadata;
        }

        public override dynamic transformValue(dynamic value)
        {
            return (string) value;
        }
    }

    public class StringChoicesParameter : KWPParameterType
    {
        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Type => "string";

        public override bool IsRequired { get; set; }

        public List<string> choices { get; set; }

        public StringChoicesParameter(string name, string description, bool isRequired, List<string> choices)
        {
            this.Name = name;
            this.Description = description;
            this.IsRequired = isRequired;
            this.choices = choices;
        }

        protected override bool validateInternal(dynamic value)
        {
            if (value is not string) throw new ParameterValidationException("Value must be of type string");
            if(!choices.Contains(value)) throw new ParameterValidationException(String.Format("Value must be one of {0}", String.Join(", ", choices)));
            return true;
        }

        public override Dictionary<string, object> getSerializedMetadata()
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata.Add("choices", choices);
            return metadata;
        }

        public override dynamic transformValue(dynamic value)
        {
            return (string)value;
        }
    }

    public class FloatParameter : KWPParameterType
    {
        public override string Name { get; set; }

        public override string Description { get; set; }

        public override string Type => "float";

        public override bool IsRequired { get; set; }

        public float? min { get; set; }
        public float? max { get; set; }

        public FloatParameter(string name, string description, bool isRequired, float? min = null, float? max = null)
        {
            this.Name = name;
            this.Description = description;
            this.IsRequired = isRequired;
            this.min = min;
            this.max = max;
        }

        protected override bool validateInternal(dynamic value)
        {
            if (value is not double && value is not Int64) throw new ParameterValidationException(String.Format("Value must be of type float, received {0}", value.GetType()));
            if (min != null && ((double)value)< min)
            {
                if (max != null) throw new ParameterValidationException(String.Format("Value must be between {0} and {1}", min, max));
                else throw new ParameterValidationException(String.Format("Value must be at least {0}", min));
            }
            if (max != null && ((double)value) > max)
            {
                if (min != null) throw new ParameterValidationException(String.Format("Value must be between {0} and {1}", min, max));
                else throw new ParameterValidationException(String.Format("Value must be at most {0}", max));
            }
            return true;
        }

        public override Dictionary<string, object> getSerializedMetadata()
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            if (min != null) metadata.Add("min", min);
            if (max != null) metadata.Add("max", max);
            return metadata;
        }

        public override dynamic transformValue(dynamic value)
        {
            return (float)value;
        }
    }
}
