using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LizardCode.Framework.Application.Models.Repeater
{
    public class RepeaterViewModel
    {
        private Type _type;
        private IList<object> _items;
        private IList<RepeaterColumn> _columns;
        private string _collection;


        public Type ItemType => _type;
        public IList<object> Items => _items;
        public IList<RepeaterColumn> Columns => _columns;
        public string Collection => _collection;


        public RepeaterViewModel()
        {
            _items = new List<object>();
        }


        public RepeaterViewModel SetItems<TItem>(IList<TItem> items, params Expression<Func<TItem, RepeaterData>>[] data)
            => SetItems<TItem>(items, "Items", data);

        public RepeaterViewModel SetItems<TItem>(IList<TItem> items, string collection, params Expression<Func<TItem, RepeaterData>>[] data)
        {
            _collection = collection;
            _type = typeof(TItem);
            var properties = _type
                .GetProperties()
                .Where(w => w.GetCustomAttributes(typeof(RepeaterColumnAttribute), false).Any())
                .Select(s => new
                {
                    s.Name,
                    Attribute = s.GetCustomAttributes(typeof(RepeaterColumnAttribute), false)
                        .Cast<RepeaterColumnAttribute>()
                        .SingleOrDefault()
                })
                .OrderBy(o => o.Attribute.Position);

            var dataValues = data.ToList()
                .Select(s => GetExpressionValue(s));

            if (!properties.Any())
                throw new ArgumentException($"Faltan los decoradores 'RepeaterColumn' en '{_type.Name}'");

            _columns = properties
                .Select(property =>
                {
                    var dataValue = dataValues.FirstOrDefault(f => f.PropertyName.Equals(property.Name));
                    var columnType = property.Attribute.ControlType;

                    if (property.Attribute.Hidden)
                        columnType = RepeaterColumnType.Hidden;

					return new RepeaterColumn(
						columnType,
						property.Name,
						dataValue?.Values,
						property.Attribute.Header,
						property.Attribute.Position,
						property.Attribute.Width,
						property.Attribute.Align,
                        property.Attribute.Remote,
                        property.Attribute.Action,
                        property.Attribute.PropertyId,
                        property.Attribute.PropertyText,
                        property.Attribute.Readonly
					);
				})
				.ToList();

            if (items != null && items.Any())
                _items = items.Cast<object>().ToList();

            return this;
        }

        public TOut GetExpressionValue<TIn, TOut>(Expression<Func<TIn, TOut>> expression)
        {
            if (expression == null)
            {
                return default;
            }

            if (expression.Body is not NewExpression me)
            {
                throw new InvalidExpressionException();
            }

            var arguments = me.Arguments
                .Select(s =>
                {
                    switch (s)
                    {
                        case ConstantExpression:
                            return ((ConstantExpression)s).Value;

                        case MemberExpression:
                            var member = (MemberExpression)s;

                            switch (member.Member)
                            {
                                case PropertyInfo:
                                    {
                                        var property = (PropertyInfo)member.Member;
                                        var submember = (MemberExpression)member.Expression;
                                        var subproperty = (PropertyInfo)submember.Member;
                                        var constant = (ConstantExpression)submember.Expression;
                                        var value = subproperty.GetValue(constant.Value, null);
                                        return property.GetValue(value, null);
                                    }

                                case FieldInfo:
                                    {
                                        var field = (FieldInfo)member.Member;
                                        var constant = (ConstantExpression)member.Expression;
                                        return field.GetValue(constant.Value);
                                    }

                                default:
                                    throw new NotImplementedException();
                            }

                        default:
                            throw new NotSupportedException();
                    }
                })
                .ToArray();

            var result = (TOut)me.Constructor.Invoke(arguments);

            return result;
        }
    }
}
