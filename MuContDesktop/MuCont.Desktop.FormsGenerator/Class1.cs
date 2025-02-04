using System;
using System.Collections;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Reactive;


namespace MuCont.Desktop.FormsGenerator;



public class AutoFormPanel : StackPanel
{
    public object? Model
    {
        get => GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    public static readonly StyledProperty<object?> ModelProperty =
        AvaloniaProperty.Register<AutoFormPanel, object?>(nameof(Model));

    public AutoFormPanel()
    {
        this.GetObservable(ModelProperty).Subscribe(new AnonymousObserver<object?>(_ => GenerateForm()));
    }

    private void GenerateForm()
    {
        Children.Clear();

        if (Model == null) return;

        //var properties = Model.GetType()
        //    .GetProperties(); // just get all properties for now
        var properties = Model.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.IsDefined(typeof(FormFieldAttribute), false));

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<FormFieldAttribute>();
            if (attribute == null) continue;

            Control inputControl = ControlFactory.Create(property, Model);
            if (inputControl != null)
            {
                Children.Add(new StackPanel
                {
                    Children = { new TextBlock { Text = attribute.Label ?? property.Name }, inputControl }
                });
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Property)]
    public class FormFieldAttribute : Attribute
    {
        public string? Label { get; }
        public FormFieldAttribute(string? label = null) => Label = label;
    }

public static class ControlFactory
{
    public static Control Create(PropertyInfo property, object model)
    {
        var binding = new Binding(property.Name) { Mode = BindingMode.TwoWay };

        //if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
        //{
        //    var itemType = property.PropertyType.IsGenericType
        //        ? property.PropertyType.GetGenericArguments()[0]
        //        : property.PropertyType.GetElementType();

        //    var itemsControl = new ItemsControl();
        //    itemsControl.Bind(ItemsControl.ItemsSourceProperty, binding);

        //    var itemTemplate = new FuncDataTemplate<object>((item, _) =>
        //    {
        //        if (itemType == typeof(string))
        //        {
        //            var textBox = new TextBox();
        //            textBox.Bind(TextBox.TextProperty, new Binding(".") { Mode = BindingMode.TwoWay });
        //            return textBox;
        //        }
        //        else if (itemType == typeof(bool))
        //        {
        //            var checkBox = new CheckBox();
        //            checkBox.Bind(ToggleButton.IsCheckedProperty, new Binding(".") { Mode = BindingMode.TwoWay });
        //            return checkBox;
        //        }
        //        else if (itemType == typeof(DateTime))
        //        {
        //            var datePicker = new DatePicker();
        //            datePicker.Bind(DatePicker.SelectedDateProperty, new Binding(".") { Mode = BindingMode.TwoWay });
        //            return datePicker;
        //        }
        //        else
        //        {
        //            var textBox = new TextBox();
        //            textBox.Bind(TextBox.TextProperty, new Binding(".") { Mode = BindingMode.TwoWay });
        //            return textBox;
        //        }
        //    });

        //    itemsControl.ItemTemplate = itemTemplate;
        //    return itemsControl;
        //}

        return property.PropertyType switch
        {
            Type t when t == typeof(bool) => new CheckBox { [!ToggleButton.IsCheckedProperty] = binding },
            Type t when t == typeof(DateTime) => new DatePicker { [!DatePicker.SelectedDateProperty] = binding },
            _ => new TextBox { [!TextBox.TextProperty] = binding }
        };
    }
}
