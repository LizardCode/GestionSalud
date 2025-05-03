using LizardCode.Framework.Application.Common.Annotations.Base;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text.Encodings.Web;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class HtmlHelpersExtensions
    {
        public static MvcForm AjaxBeginForm<TModel>(this IHtmlHelper<TModel> htmlHelper, string actionName, string mainClass, string begin = "ajaxFormBegin", string success = "ajaxFormSuccess", string failure = "ajaxFormFailure", bool formHorizontal = false, bool multipart = false, string css = null)
        {
            if (actionName.IsNull() || mainClass.IsNull())
            {
                throw new ArgumentNullException(nameof(actionName));
            }

            var attributes = new Dictionary<string, object>
            {
                { "action", actionName },
                { "class", formHorizontal ? "form-horizontal" : string.Empty },
                { "role", "form" }
            };

            if (multipart)
            {
                attributes.Add("enctype", "multipart/form-data");
            }

            if (!css.IsNull())
            {
                attributes["class"] += $" {css}";
            }

            attributes.AddRange(
                new
                {
                    data_ajax = "true",
                    data_ajax_method = WebRequestMethods.Http.Post,
                    data_ajax_begin = $"{mainClass}.{begin}",
                    data_ajax_success = $"{mainClass}.{success}",
                    data_ajax_failure = $"{mainClass}.{failure}"
                }
                .AsDictionary()
            );

            return htmlHelper.BeginForm(
                actionName: string.Empty,
                controllerName: string.Empty,
                routeValues: null,
                method: FormMethod.Post,
                htmlAttributes: attributes,
                antiforgery: false
            );
        }


        public static HtmlString FormGroup<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, IHtmlContent innerControl, string title = null, string subtitle = null, string colSize = null, HtmlString validationControl = null)
        {
            if (title == null)
            {
                title = name;
            }

            if (colSize.IsNull())
            {
                colSize = "col-md-12";
            }

            var field = innerControl.ToPlainString();

            if (validationControl != null)
            {
                field += validationControl.ToPlainString();
            }

            var snippet =
              $@"<div class=""form-group field {colSize}"">
                    <label class=""form-label"" for=""{name}"">{title}</label>
                    <span class=""desc"">{subtitle}</span>
                    <div class=""controls"">
                        {field}
                    </div>
                </div>";

            return new HtmlString(snippet);
        }

        public static HtmlString FormGroupFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IHtmlContent innerControl, string title = null, string subtitle = null, string colSize = null, HtmlString validationControl = null)
        {
            var modelExpressionProvider = (ModelExpressionProvider)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider));
            var modelExpression = modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var modelMetadata = modelExpression.Metadata;
            var propName = modelMetadata.PropertyName;

            innerControl = ApplyConstraints(innerControl, modelMetadata, propName);

            return htmlHelper.FormGroup(propName, innerControl, title, subtitle, colSize, validationControl);
        }


        public static HtmlString FormGroupInputGroup<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, IHtmlContent innerControl, string title = null, string subtitle = null, string colSize = null, string icon = "", HtmlString validationControl = null)
        {
            if (title == null)
            {
                title = name;
            }

            if (colSize.IsNull())
            {
                colSize = "col-md-12";
            }

            var field = innerControl.ToPlainString();
            var validation = validationControl?.ToPlainString();


            if (icon.IsNull())
            {
                icon = "#";
            }

            var snippet =
              $@"<div class=""form-group field {colSize}"">
                    <label class=""form-label"" for=""{name}"">{title}</label>
                    <span class=""desc"">{subtitle}</span>
                    <div class=""controls"">
                        <div class=""input-group"">
                            <div class=""input-group-prepend"">
                                <span class=""input-group-text"">{icon}</span>
                            </div>
                            {field}
                        </div>
                        <div>
                            {validation}
                        </div>
                    </div>
                </div>";

            return new HtmlString(snippet);
        }

        public static HtmlString FormGroupInputGroupFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IHtmlContent innerControl, string title = null, string subtitle = null, string colSize = null, string icon = null, HtmlString validationControl = null)
        {
            var modelExpressionProvider = (ModelExpressionProvider)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider));
            var modelExpression = modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var modelMetadata = modelExpression.Metadata;
            var propName = modelMetadata.PropertyName;

            innerControl = ApplyConstraints(innerControl, modelMetadata, propName);

            return htmlHelper.FormGroupInputGroup(propName, innerControl, title, subtitle, colSize, icon, validationControl);
        }

        public static HtmlString FormGroupDropDownGroupFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string dropdownName, IEnumerable<SelectListItem> itemList, string title = null, string subtitle = null, string fieldCss = "form-control", string dropdownCss = "btn-primary", string colSize = null, string placeholder = null, string format = null, int maxLength = 0, bool addValidator = false, bool autocomplete = true)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (maxLength > 0)
            {
                attributes.Add("maxlength", maxLength);
            }

            if (!autocomplete)
            {
                attributes.Add("autocomplete", "new-password");
            }

            var dropdownItems = itemList.Select(s => $@"<a class=""dropdown-item"" href=""#"" data-value=""{s.Value}"">{s.Text}</a>");
            var input = htmlHelper.TextBoxFor(expression, format, attributes);
            var snippet =
              $@"<div class=""input-group dropdown-group-list"">
                    <div class=""input-group-prepend"">
                        <input type=""hidden"" id=""{dropdownName}"" name=""{dropdownName}"" />
                        <button type=""button"" class=""btn {dropdownCss} dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false""><span>{itemList.First().Text}</span> <i class=""far fa-chevron-down""></i></button>
                        <div class=""dropdown-menu"">
                            {string.Join("\r\n", dropdownItems)}
                        </div>
                    </div>
                    {input.ToPlainString()}
                </div>";

            var tagBuilder = new TagBuilder("div");
            tagBuilder.InnerHtml.SetHtmlContent(snippet);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, tagBuilder, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, tagBuilder, title, subtitle, colSize);
            }
        }


        public static HtmlString FormGroupTextBox<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string title = null, string subtitle = null, string fieldCss = "form-control", string colSize = null, string placeholder = null, string value = null, string maxlength = null, string format = null, bool autocomplete = true)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (!maxlength.IsNull())
            {
                attributes.Add("maxlength", maxlength);
            }

            if (!autocomplete)
            {
                attributes.Add("autocomplete", "new-password");
            }

            var input = htmlHelper.TextBox(name, value, format, attributes);

            return htmlHelper.FormGroup(name, input, title, subtitle, colSize);
        }

        public static HtmlString FormGroupTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, bool editingDisabled = false, bool newVisible = true, string fieldCss = "form-control", string colSize = null, string placeholder = null, string format = null, int maxLength = 0, bool addValidator = false, bool autocomplete = true)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (maxLength > 0)
            {
                attributes.Add("maxlength", maxLength);
            }

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (!newVisible)
            {
                attributes.Add("data-newVisible", "false");
            }

            if (!autocomplete)
            {
                attributes.Add("autocomplete", "new-password");
            }

            var input = htmlHelper.TextBoxFor(expression, format, attributes);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize);
            }
        }

        public static HtmlString FormGroupPickerDateFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, bool editingDisabled = false, bool newVisible = true, string colSize = null, string placeholder = null, string format = null, bool addValidator = false)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", "form-control flatpickr" }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (!newVisible)
            {
                attributes.Add("data-newVisible", "false");
            }

            var icon = "<i class='fa fa-calendar'></i>";
            var input = htmlHelper.TextBoxFor(expression, format, attributes);

            if (addValidator)
            {
                return htmlHelper.FormGroupInputGroupFor(expression, input, title, subtitle, colSize, icon, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupInputGroupFor(expression, input, title, subtitle, colSize, icon);
            }
        }

        public static HtmlString FormGroupInputGroupFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, string icon = null, bool editingDisabled = false, bool newVisible = true, string fieldCss = "form-control", string colSize = null, string placeholder = null, string format = null, int maxLength = 0, bool addValidator = false)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (maxLength > 0)
            {
                attributes.Add("maxlength", maxLength);
            }

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (!newVisible)
            {
                attributes.Add("data-newVisible", "false");
            }

            var input = htmlHelper.TextBoxFor(expression, format, attributes);

            if (addValidator)
            {
                return htmlHelper.FormGroupInputGroupFor(expression, input, title, subtitle, colSize, icon, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupInputGroupFor(expression, input, title, subtitle, colSize, icon);
            }
        }

        public static HtmlString FormGroupNumberFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, bool editingDisabled = false, bool newVisible = true, string fieldCss = "form-control", string colSize = null, string placeholder = null, string format = null, int minValue = 0, int maxValue = 100, bool addValidator = false, bool autocomplete = true)
        {
            var attributes = new Dictionary<string, object>
            {
                { "type", "number" },
                { "class", fieldCss },
                { "min", minValue },
                { "max", maxValue }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (!newVisible)
            {
                attributes.Add("data-newVisible", "false");
            }

            if (!autocomplete)
            {
                attributes.Add("autocomplete", "new-password");
            }

            var input = htmlHelper.TextBoxFor(expression, format, attributes);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize);
            }
        }

        public static HtmlString FormGroupTextAreaFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, bool editingDisabled = false, bool newVisible = true, string fieldCss = "form-control", string colSize = null, string placeholder = null, int rows = 2, int columns = 1, int maxLength = 0, bool addValidator = false)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (maxLength > 0)
            {
                attributes.Add("maxlength", maxLength);
            }

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (!newVisible)
            {
                attributes.Add("data-newVisible", "false");
            }

            var input = htmlHelper.TextAreaFor(expression, rows, columns, attributes);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize);
            }
        }


        public static HtmlString DropDownList<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, IEnumerable<SelectListItem> itemList, bool incluirTodos = true, string todosText = null, string todosValue = null, string fieldCss = "select2-field validate", string placeholder = null)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            var lista = itemList.ToList();

            if (incluirTodos)
            {
                lista.Insert(0, new SelectListItem
                {
                    Value = todosValue ?? string.Empty,
                    Text = todosText ?? "..."
                });
            }

            return htmlHelper.DropDownList(name, lista, attributes).ToHtmlString();
        }

        public static HtmlString FormGroupDropDownList<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, IEnumerable<SelectListItem> itemList, string title = null, string subtitle = null, bool incluirTodos = true, string todosText = null, string todosValue = null, string fieldCss = "select2-field validate", string colSize = null, string placeholder = null, bool addValidator = false, Dictionary<string, object> htmlAttributes = null)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (htmlAttributes.Any())
                attributes.AddRange(htmlAttributes);

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            var lista = itemList?.ToList() ?? new List<SelectListItem>();

            if (incluirTodos)
            {
                lista.Insert(0, new SelectListItem
                {
                    Value = todosValue ?? string.Empty,
                    Text = todosText ?? string.Empty
                });
            }

            var input = htmlHelper.DropDownList(name, lista, attributes);

            return htmlHelper.FormGroup(name, input, title, subtitle, colSize);
        }

        public static HtmlString DropDownListFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> itemList, bool incluirTodos = true, string todosText = null, string todosValue = null, string fieldCss = "select2-field validate", string placeholder = null)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            var lista = itemList.ToList();

            if (incluirTodos)
            {
                lista.Insert(0, new SelectListItem
                {
                    Value = todosValue ?? string.Empty,
                    Text = todosText ?? "..."
                });
            }

            return htmlHelper.DropDownListFor(expression, lista, attributes).ToHtmlString();
        }

        public static HtmlString FormGroupDropDownListFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> itemList, string title = null, string subtitle = null, bool incluirTodos = true, string todosText = null, string todosValue = null, string fieldCss = "select2-field validate", string colSize = null, string placeholder = null, bool addValidator = false, bool multiple = false, bool editingDisabled = false)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (multiple)
            {
                attributes.Add("multiple", true);
            }

            var lista = itemList?.ToList() ?? new List<SelectListItem>();

            if (incluirTodos)
            {
                lista.Insert(0, new SelectListItem
                {
                    Value = todosValue ?? string.Empty,
                    Text = todosText ?? string.Empty
                });
            }

            var input = htmlHelper.DropDownListFor(expression, lista, attributes);

            HtmlString validationInput = null;

            if (addValidator)
            {
                validationInput = htmlHelper.ValidationMessageFor(expression).ToHtmlString();
            }

            return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize, validationInput);
        }


        public static HtmlString FilterTextBox<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, string title = null, string subtitle = null, string value = null, string fieldCss = "form-control", string colSize = "col-md-3", string placeholder = null, string format = null, int maxLength = 0, bool autocomplete = true)
        {
            var propName = $"Filter_{name}";
            var maxLengthStr = (maxLength > 0 ? maxLength.ToString() : null);

            return FormGroupTextBox(htmlHelper, propName, title, subtitle, fieldCss, colSize, placeholder, value, maxLengthStr, format, autocomplete);
        }

        public static HtmlString FilterTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, string value = null, string fieldCss = "form-control", string colSize = "col-md-3", string placeholder = null, string format = null, int maxLength = 0, bool autocomplete = true)
        {
            var modelExpressionProvider = (ModelExpressionProvider)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider));
            var modelExpression = modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var modelMetadata = modelExpression.Metadata;
            var propName = $"Filter_{modelMetadata.PropertyName}";
            var maxLengthStr = (maxLength > 0 ? maxLength.ToString() : null);

            return FormGroupTextBox(htmlHelper, propName, title, subtitle, fieldCss, colSize, placeholder, value ?? modelExpression.Model?.ToString(), maxLengthStr, format, autocomplete);
        }

        public static HtmlString FilterPickerDateFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, string value = null, string fieldCss = "flatpickr-filter-field", string colSize = "col-md-3", int maxLength = 0)
        {
            var modelExpressionProvider = (ModelExpressionProvider)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider));
            var modelExpression = modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var modelMetadata = modelExpression.Metadata;
            var propName = $"Filter_{modelMetadata.PropertyName}";
            var maxLengthStr = (maxLength > 0 ? maxLength.ToString() : "10");
            var placeholder = "__/__/____";

            return FormGroupTextBox(htmlHelper, propName, title, subtitle, fieldCss, colSize, placeholder, value ?? modelExpression.Model?.ToString(), maxLengthStr);
        }

        public static HtmlString FilterDropDownList<TModel>(this IHtmlHelper<TModel> htmlHelper, string name, IEnumerable<SelectListItem> itemList, string title = null, string subtitle = null, string fieldCss = "select2-filter-field", string colSize = "col-md-3", string placeholder = null, FilterDropDownDataField dataValue = FilterDropDownDataField.Value)
        {
            var propName = $"Filter_{name}";
            var lista = itemList?.ToList() ?? new List<SelectListItem>();

            lista.Insert(0, new SelectListItem
            {
                Value = string.Empty,
                Text = " "
            });

            var attributes = new Dictionary<string, object>
            {
                { "data-value-property", dataValue.ToString() }
            };

            return FormGroupDropDownList(htmlHelper, propName, lista, title, subtitle, false, null, null, fieldCss, colSize, placeholder, false, attributes);
        }

        public static HtmlString FilterDropDownListFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> itemList, string title = null, string subtitle = null, string fieldCss = "select2-filter-field", string colSize = "col-md-3", string placeholder = null, FilterDropDownDataField dataValue = FilterDropDownDataField.Value)
        {
            var modelExpressionProvider = (ModelExpressionProvider)htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider));
            var modelExpression = modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
            var modelMetadata = modelExpression.Metadata;
            var propName = $"Filter_{modelMetadata.PropertyName}";
            var lista = itemList?.ToList() ?? new List<SelectListItem>();

            lista.Insert(0, new SelectListItem
            {
                Value = string.Empty,
                Text = " "
            });

            var attributes = new Dictionary<string, object>
            {
                { "data-value-property", dataValue.ToString() }
            };

            return FormGroupDropDownList(htmlHelper, propName, lista, title, subtitle, false, null, null, fieldCss, colSize, placeholder, false, attributes);
        }

        public static HtmlString FormGroupDropDownAndButtonGroupFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string dropdownName, IEnumerable<SelectListItem> itemList, string title = null, string subtitle = null, string fieldCss = "form-control", string buttonCss = "btnCssId", string buttonText = "button", string position = "prepend", string dropdownCss = "btn-primary", string colSize = null, string placeholder = null, string format = null, int maxLength = 0, bool addValidator = false, bool autocomplete = true)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (maxLength > 0)
            {
                attributes.Add("maxlength", maxLength);
            }

            if (!autocomplete)
            {
                attributes.Add("autocomplete", "new-password");
            }

            var button =
                $@"<div class=""input-group-prepend"">
                    <button type=""button"" class=""btn btn-primary {buttonCss}"">{buttonText}</button>
                </div>";

            if (position == "append")
            {
                button =
                  $@"<div class=""input-group-append"">
                        <button type=""button"" class=""btn btn-primary {buttonCss}"">{buttonText}</button>
                    </div>";
            }

            var dropdownItems = itemList.Select(s => $@"<a class=""dropdown-item"" href=""#"" data-value=""{s.Value}"">{s.Text}</a>");
            var input = htmlHelper.TextBoxFor(expression, format, attributes);
            var snippet =
              $@"<div class=""input-group dropdown-group-list"">
                    <div class=""input-group-prepend"">
                        <input type=""hidden"" id=""{dropdownName}"" name=""{dropdownName}"" />
                        <button type=""button"" class=""btn {dropdownCss} dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false""><span>{itemList.First().Text}</span> <i class=""far fa-chevron-down""></i></button>
                        <div class=""dropdown-menu"">
                            {string.Join("\r\n", dropdownItems)}
                        </div>
                    </div>
                    {input.ToPlainString()}
                    {button}
                </div>";

            var tagBuilder = new TagBuilder("div");
            tagBuilder.InnerHtml.SetHtmlContent(snippet);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, tagBuilder, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, tagBuilder, title, subtitle, colSize);
            }
        }

        public static HtmlString FormGroupCheckboxFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, string title = null, string colSize = null, string fieldCss = "new-control-input", HtmlString validationControl = null, bool checkedInput = false)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (colSize.IsNull())
            {
                colSize = "col-md-12";
            }

            if (checkedInput)
            {
                attributes["checked"] = true;
            }

            var input = htmlHelper.CheckBoxFor(expression, attributes);

            var validation = validationControl?.ToPlainString();

            var snippet =
                $@"<div class=""n-chk {colSize}"">
                        <label class=""new-control new-checkbox checkbox-primary"">
                            {input.ToPlainString()}
                            <span class=""new-control-indicator""></span>{title}
                        </label>
                        <div>
                            {validation}
                        </div>
                    </div>";

            return new HtmlString(snippet);
        }

        public static HtmlString FormGroupButtonGroupFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, string fieldCss = "form-control", string buttonCss = "btnCssId", string buttonText = "button", string colSize = null, string placeholder = null, string format = null, int maxLength = 0, bool addValidator = false, string position = "prepend")
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", fieldCss }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (maxLength > 0)
            {
                attributes.Add("maxlength", maxLength);
            }

            var input = htmlHelper.TextBoxFor(expression, format, attributes);
            var snippet =
                  $@"<div class=""input-group mb-4"">
                    <div class=""input-group-prepend"">
                        <button type=""button"" class=""btn btn-primary {buttonCss}"">{buttonText}</button>
                    </div>
                    {input.ToPlainString()}
                </div>";

            if (position == "append")
            {
                snippet =
                  $@"<div class=""input-group mb-4"">
                    {input.ToPlainString()}
                    <div class=""input-group-append"">
                        <button type=""button"" class=""btn btn-primary {buttonCss}"">{buttonText}</button>
                    </div>
                </div>";
            }

            var tagBuilder = new TagBuilder("div");
            tagBuilder.InnerHtml.SetHtmlContent(snippet);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, tagBuilder, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, tagBuilder, title, subtitle, colSize);
            }
        }

        public static HtmlString FormGroupFileFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, bool editingDisabled = false, bool newVisible = true, string fieldCss = "form-control", string icon = null, string buttonText = null, string colSize = null, bool addValidator = false, string accept = "")
        {
            var attributes = new Dictionary<string, object>
            {
                { "type", "file" },
                { "class", fieldCss }
            };

            if (!string.IsNullOrEmpty(accept))
            {
                attributes.Add("accept", accept);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (buttonText.IsNull())
            {
                attributes.Add("data-buttonText", "Buscar");
            }
            else
            {
                attributes.Add("data-buttonText", buttonText);
            }

            if (icon.IsNull())
            {
                attributes.Add("data-iconName", "fas fa-folder-open");
            }
            else
            {
                attributes.Add("data-iconName", $"fas {icon}");
            }

            if (!newVisible)
            {
                attributes.Add("data-newVisible", "false");
            }

            var input = htmlHelper.TextBoxFor(expression, null, attributes);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize);
            }
        }

        public static HtmlString FormGroupNumberFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string title = null, string subtitle = null, bool editingDisabled = false, bool newVisible = true, string fieldCss = "form-control", string colSize = null, string placeholder = null, string format = null, int minValue = 0, int maxValue = 100, int step = 1, bool addValidator = false, bool autocomplete = true)
        {
            var attributes = new Dictionary<string, object>
            {
                { "type", "number" },
                { "class", fieldCss },
                { "min", minValue },
                { "max", maxValue },
                { "step", step }
            };

            if (!placeholder.IsNull())
            {
                attributes.Add("placeholder", placeholder);
            }

            if (editingDisabled)
            {
                attributes.Add("data-editingDisabled", "true");
            }

            if (!newVisible)
            {
                attributes.Add("data-newVisible", "false");
            }

            if (!autocomplete)
            {
                attributes.Add("autocomplete", "new-password");
            }

            var input = htmlHelper.TextBoxFor(expression, format, attributes);

            if (addValidator)
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize, htmlHelper.ValidationMessageFor(expression).ToHtmlString());
            }
            else
            {
                return htmlHelper.FormGroupFor(expression, input, title, subtitle, colSize);
            }
        }


        public static DisposableBlockHelper BeginModal<TModel>(
            this IHtmlHelper<TModel> htmlHelper,
            string title,
            string mainClass,
            string modalClass,
            string action = null,
            string dialogClass = "",
            bool cancelVisible = true,
            bool confirmVisible = true,
            string cancelText = "Cancelar",
            string confirmText = "Guardar")
        {
            var begin =
              @"<div class=""modal fade [@ModalClass]"" role=""dialog"" aria-hidden=""true"">
                    <div class=""modal-dialog bounceInRight animated [@DialogClass]"">
                        <div class=""modal-content"">
                            <div class=""modal-header"">
                                <h5 class=""modal-title"">[@Title]</h5>
                                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                                    <span aria-hidden=""true"" class=""zmdi zmdi-close""></span>
                                </button>
                            </div>
                            <div class=""modal-body"">
                                <div class=""row"">
                                    <div class=""col"">
                                        <form action=""[@Action]"" data-ajax=""true"" data-ajax-begin=""[@MainClass].[@ModalClass]ajaxFormBegin"" data-ajax-success=""[@MainClass].[@ModalClass]ajaxFormSuccess"" data-ajax-failure=""[@MainClass].[@ModalClass]ajaxFormFailure"" data-ajax-method=""POST"" method=""post"" role=""form"" novalidate=""novalidate"">";

            var btnCancel = @"<button type=""button"" data-dismiss=""modal"" class=""btn btn-default""><i class=""fa fa-times""></i> [@CancelButtonText]</button>";
            var btnConfirm = @"<button type=""submit"" class=""btn btn-primary btConfirm""><i class=""fa fa-check""></i> [@ConfirmButtonText]</button>";

            var end =
              @"                        </form>
                                    </div>
                                </div>
                            </div>
                            <div class=""modal-footer"">
                                [@CancelButton]
                                [@ConfirmButton]
                            </div>
                        </div>
                    </div>
                </div>";

            begin = begin
                .Replace("[@MainClass]", mainClass)
                .Replace("[@ModalClass]", modalClass)
                .Replace("[@DialogClass]", dialogClass)
                .Replace("[@Title]", title)
                .Replace("[@Action]", action ?? "#");

            end = end
                .Replace("[@CancelButton]", cancelVisible ? btnCancel : "").Replace("[@CancelButtonText]", cancelText)
                .Replace("[@ConfirmButton]", confirmVisible ? btnConfirm : "").Replace("[@ConfirmButtonText]", confirmText);

            return new DisposableBlockHelper(htmlHelper, new HtmlString(begin), new HtmlString(end));
        }

        public static HtmlString ToHtmlString(this HtmlString input)
        {
            return (input as IHtmlContent).ToHtmlString();
        }

        public static HtmlString ToHtmlString(this IHtmlContent input)
        {
            var content = input.ToPlainString();

            return new HtmlString(content);
        }


        private static string ToPlainString(this IHtmlContent input)
        {
            using var sw = new StringWriter();

            input.WriteTo(sw, HtmlEncoder.Default);

            return sw.ToString();
        }

        private static Dictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name.Replace("_", "-"),
                propInfo => propInfo.GetValue(source, null)
            );

        }

        private static IHtmlContent ApplyConstraints(IHtmlContent innerControl, ModelMetadata modelMetadata, string propertyName)
        {
            var pi = modelMetadata.ContainerType.GetProperty(propertyName);
            var constraints = pi.GetCustomAttributes<PropertyConstraint>();

            if (!constraints.Any())
                return innerControl;

            var tagBuilder = new TagBuilder((TagBuilder)innerControl);

            foreach (var constraint in constraints)
                foreach (var attribute in constraint.HtmlAttributes)
                    tagBuilder.Attributes.Add(attribute.Key, attribute.Value);

            return tagBuilder;
        }
    }

    public enum FilterDropDownDataField
    {
        Text,
        Value
    }
}