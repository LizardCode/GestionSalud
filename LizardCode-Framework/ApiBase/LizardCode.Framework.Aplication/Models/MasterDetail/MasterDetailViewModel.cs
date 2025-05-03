using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Common.Extensions;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace LizardCode.Framework.Application.Models.MasterDetail
{
    public class MasterDetailViewModel
    {
        private Type _type;
        private IList<object> _items;
        private IList<MasterDetailColumn> _columns;
        private IList<MasterDetailFormatAttribute> _formats;
        private string _collection;

        public Type ItemType => _type;
        public IList<object> Items => _items;
        public string Collection => _collection;

        public IList<MasterDetailColumn> Columns => _columns;
        public IList<MasterDetailFormatAttribute> Formats => _formats;
        public MasterDetailViewModel()
        {
            _items = new List<object>();
        }

        public MasterDetailViewModel SetItems<TItem>(IList<TItem> items, params Expression<Func<TItem, MasterDetailData>>[] data)
            => SetItems<TItem>(items, "Items", data);

        public MasterDetailViewModel SetItems<TItem>(IList<TItem> items, string collection, params Expression<Func<TItem, MasterDetailData>>[] data)
        {
            _collection = collection;
            _type = typeof(TItem);

            var properties = _type
                .GetProperties()
                .Where(w => w.GetCustomAttributes(typeof(MasterDetailColumnAttribute), false).Any())
                .Select(s => new
                {
                    s.Name,
                    s.PropertyType,
                    Attribute = s.GetCustomAttributes(typeof(MasterDetailColumnAttribute), false)
                        .Cast<MasterDetailColumnAttribute>()
                        .SingleOrDefault()
                })
                .OrderBy(o => o.Attribute.Position);

            var formats = _type
                .GetProperties()
                .Where(w => w.GetCustomAttributes(typeof(MasterDetailFormatAttribute), false).Any())
                .Select(s => new
                {
                    s.Name,
                    AttributeFormat = s.GetCustomAttributes(typeof(MasterDetailFormatAttribute), false)
                        .Cast<MasterDetailFormatAttribute>()
                        .SingleOrDefault()
                });

            var dataValues = data.ToList()
                .Select(s => GetExpressionValue(s));

            if (!properties.Any())
                throw new ArgumentException($"Faltan los decoradores 'MasterDetailColumn' en '{_type.Name}'");

            _columns = properties
                .Select(property =>
                {
                    var dataValue = dataValues.FirstOrDefault(f => f.PropertyName.Equals(property.Name));
                    var columnType = (
                        dataValue == null
                            ? MasterDetailColumnType.Text
                            : MasterDetailColumnType.Lookup
                    );

                    if (property.Attribute.Hidden)
                        columnType = MasterDetailColumnType.Hidden;
                    else if (property.PropertyType == typeof(bool))
                        columnType = MasterDetailColumnType.Check;

                    var columnFormat = formats.FirstOrDefault(f => f.Name.Equals(property.Name))?.AttributeFormat.Format ?? MasterDetailColumnFormat.None;

                    return new MasterDetailColumn(
                        columnType,
                        columnFormat,
                        property.Name,
                        dataValue?.Values,
                        property.Attribute.Header,
                        property.Attribute.Position,
                        property.Attribute.Width,
                        dataValue?.PropertyDisplayName
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
