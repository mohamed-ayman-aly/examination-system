using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq.Dynamic;

namespace examination_system.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Unique : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            using (DB db = new DB())
            {
                var Name = validationContext.MemberName;

                if (string.IsNullOrEmpty(Name))
                {
                    var displayName = validationContext.DisplayName;

                    var prop = validationContext.ObjectInstance.GetType().GetProperty(displayName);

                    if (prop != null)
                    {
                        Name = prop.Name;
                    }
                    else
                    {
                        var props = validationContext.ObjectInstance.GetType().GetProperties().Where(x => x.CustomAttributes.Count(a => a.AttributeType == typeof(DisplayAttribute)) > 0).ToList();

                        foreach (PropertyInfo prp in props)
                        {
                            var attr = prp.CustomAttributes.FirstOrDefault(p => p.AttributeType == typeof(DisplayAttribute));

                            var val = attr.NamedArguments.FirstOrDefault(p => p.MemberName == "Name").TypedValue.Value;

                            if (val.Equals(displayName))
                            {
                                Name = prp.Name;
                                break;
                            }
                        }
                    }
                }
                PropertyInfo IdProp = validationContext.ObjectInstance.GetType().GetProperties().FirstOrDefault(x => x.CustomAttributes.Count(a => a.AttributeType == typeof(KeyAttribute)) > 0);

                string Id = IdProp.GetValue(validationContext.ObjectInstance, null).ToString();
                Type entityType = validationContext.ObjectType;


                var result = db.Set(entityType).Where(Name + "==@0", value);
                int count = 0;
                if (Id != "00000000-0000-0000-0000-000000000000")
                {
                    result = result.Where(IdProp.Name + "!=@0",new Guid(Id));
                }

                count = result.Count();

                if (count == 0)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(ErrorMessageString);
            }


        }
    }
}