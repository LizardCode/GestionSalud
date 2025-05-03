using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Models.Menu;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class MenuExtensions
    {
        public static (List<ItemMenu>, ItemMenu) AddMenu(this List<ItemMenu> input, string texto, string icono, string action = null, string codigo = null)
        {
            if (action == null)
            {
                action = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(codigo))
            {
                var match = input
                    .Where(w => !string.IsNullOrWhiteSpace(w.Codigo))
                    .Any(a => a.Codigo.ToLower().Equals(codigo.ToLower()));

                if (match)
                {
                    throw new ArgumentException("ItemMenu código existente");
                }
            }

            var newItem = new ItemMenu
            {
                Texto = texto,
                Icono = icono,
                Action = action,
                Codigo = codigo?.ToLower()
            };

            input.Add(newItem);

            return (input, newItem);
        }

        public static (List<ItemMenu>, ItemMenu) AddMenu(this (List<ItemMenu>, ItemMenu) input, string texto, string icono, string action = null, string codigo = null)
        {
            return input.Item1.AddMenu(texto, icono, action, codigo);
        }

        public static (List<ItemMenu>, ItemMenu) AddSubMenu(this (List<ItemMenu>, ItemMenu) input, string texto, string icono, string action = null, string codigo = null)
        {
            if (action == null)
            {
                action = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(codigo))
            {
                var match = input.Item1
                    .Where(w => !string.IsNullOrWhiteSpace(w.Codigo))
                    .Any(a => a.Codigo.ToLower().Equals(codigo.ToLower()));

                if (match)
                {
                    throw new ArgumentException("ItemMenu código existente");
                }
            }

            if (input.Item2.SubMenu == null)
            {
                input.Item2.SubMenu = new List<ItemMenu>();
            }

            input.Item2.SubMenu.Add(new ItemMenu
            {
                Texto = texto,
                Icono = icono,
                Action = action,
                Codigo = codigo?.ToLower()
            });

            return input;
        }

        public static (List<ItemMenu>, ItemMenu) AddSeparator(this (List<ItemMenu>, ItemMenu) input, bool spacer = false, string codigo = null)
        {
            var newItem = new ItemMenu(
                spacer
                    ? TipoMenuItem.Espaciador
                    : TipoMenuItem.Separador
            )
            {
                Codigo = codigo.ToLower()
            };

            input.Item1.Add(newItem);

            return (input.Item1, newItem);
        }

        public static (List<ItemMenu>, ItemMenu) AddSubMenuSeparator(this (List<ItemMenu>, ItemMenu) input, bool spacer = false, string codigo = null)
        {
            if (input.Item2.SubMenu == null)
            {
                input.Item2.SubMenu = new List<ItemMenu>();
            }

            var newItem = new ItemMenu(
                spacer
                    ? TipoMenuItem.Espaciador
                    : TipoMenuItem.Separador
            )
            {
                Codigo = codigo.ToLower()
            };

            input.Item2.SubMenu.Add(newItem);

            return input;
        }

        public static (List<ItemMenu>, ItemMenu) Remove(this List<ItemMenu> input, string codigo)
        {
            var droppedItem = input.Find(codigo);

            if (droppedItem != null)
            {
                input.Remove(droppedItem);
            }

            foreach (var item in input)
            {
                if (item.SubMenu != null && item.SubMenu.Any())
                {
                    item.SubMenu.Remove(droppedItem);
                }
            }

            return (input, droppedItem);
        }

        public static (List<ItemMenu>, ItemMenu) Remove(this (List<ItemMenu>, ItemMenu) input, string codigo)
        {
            return input.Item1.Remove(codigo);
        }

        public static (List<ItemMenu>, ItemMenu) RemoveStartsWith(this List<ItemMenu> input, string codigo)
        {
            var droppedItems = input.FindStartsWith(codigo);

            if (droppedItems.Any())
            {
                foreach (var i in droppedItems)
                {
                    input.Remove(i);
                }
            }

            return (input, null);
        }

        public static (List<ItemMenu>, ItemMenu) RemoveStartsWith(this (List<ItemMenu>, ItemMenu) input, string codigo)
        {
            return input.Item1.RemoveStartsWith(codigo);
        }

        public static ItemMenu Find(this List<ItemMenu> input, string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return null;
            }

            foreach (var i in input)
            {
                if (string.IsNullOrWhiteSpace(i.Codigo))
                {
                    continue;
                }
                else if (i.Codigo.ToLower().Equals(codigo.ToLower()))
                {
                    return i;
                }
                else if (i.SubMenu != null && i.SubMenu.Count > 0)
                {
                    return i.SubMenu.Find(codigo);
                }
            }

            return null;
        }

        public static List<ItemMenu> FindStartsWith(this List<ItemMenu> input, string codigo)
        {
            var results = new List<ItemMenu>();

            if (string.IsNullOrWhiteSpace(codigo))
            {
                return results;
            }

            foreach (var i in input)
            {
                if (string.IsNullOrWhiteSpace(i.Codigo))
                {
                    continue;
                }
                else if (i.Codigo.ToLower().StartsWith(codigo.ToLower()))
                {
                    results.Add(i);
                }

                if (i.SubMenu != null && i.SubMenu.Count > 0)
                {
                    results.AddRange(i.SubMenu.FindStartsWith(codigo));
                }
            }

            return results;
        }

        public static List<ItemMenu> ToList(this (List<ItemMenu>, ItemMenu) input)
        {
            return input.Item1;
        }
    }
}