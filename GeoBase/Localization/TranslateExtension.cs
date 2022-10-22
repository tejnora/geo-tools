using System;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace GeoBase.Localization
{
    [ContentProperty("Parameters")]
    public class Translate : MarkupExtension
    {
        DependencyProperty _property;
        DependencyObject _target;
        readonly Collection<BindingBase> _parameters = new Collection<BindingBase>();
        string _nameSpace;

        public Translate() { }

        public Translate(object defaultValue)
        {
            Default = defaultValue;
        }

        public object Default { get; private set; }
        public string Uid { get; private set; }

        public Collection<BindingBase> Parameters
        {
            get { return _parameters; }
        }

        public static string GetUid(DependencyObject obj)
        {
            return (string)obj.GetValue(UidProperty);
        }

        public static void SetUid(DependencyObject obj, string value)
        {
            obj.SetValue(UidProperty, value);
        }

        public static readonly DependencyProperty UidProperty = DependencyProperty.RegisterAttached("Uid", typeof(string), typeof(Translate), new UIPropertyMetadata(string.Empty));

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service == null)
                return this;

            var property = service.TargetProperty as DependencyProperty;
            var target = service.TargetObject as DependencyObject;
            if (property == null || target == null)
                return this;

            _target = target;
            _property = property;
            _nameSpace = (string)_target.GetValue(LanguageNamespaceDependencyProperty.LanguageNamespaceProperty);
            return BindDictionary(serviceProvider);
        }

        public static DependencyObject GetParent(DependencyObject obj)
        {
            if (obj == null)
                return null;

            var ce = obj as ContentElement;
            if (ce != null)
            {
                DependencyObject parent = ContentOperations.GetParent(ce);
                if (parent != null)
                    return parent;

                var fce = ce as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            return VisualTreeHelper.GetParent(obj);
        }

        public static T FindAncestorOrSelf<T>(DependencyObject obj)
            where T : DependencyObject
        {
            while (obj != null)
            {
                T objTest = obj as T;
                if (objTest != null)
                    return objTest;
                obj = GetParent(obj);
            }

            return null;
        }

        private object BindDictionary(IServiceProvider serviceProvider)
        {
            var uid = Uid ?? GetUid(_target);
            var vid = _property.Name;

            var binding = new Binding("Dictionary");
            binding.Source = LanguageContext.Instance;
            binding.Mode = BindingMode.TwoWay;
            var converter = new LanguageConverter(uid, vid, Default, _nameSpace);
            if (_parameters.Count == 0)
            {
                binding.Converter = converter;
                var value = binding.ProvideValue(serviceProvider);
                return value;
            }
            else
            {
                var multiBinding = new MultiBinding();
                multiBinding.Mode = BindingMode.TwoWay;
                multiBinding.Converter = converter;
                multiBinding.Bindings.Add(binding);
                if (string.IsNullOrEmpty(uid))
                {
                    var uidBinding = _parameters[0] as Binding;
                    if (uidBinding == null)
                    {
                        throw new ArgumentException("Uid Binding parameter must be the first, and of type Binding");
                    }
                }
                foreach (Binding parameter in _parameters)
                {
                    multiBinding.Bindings.Add(parameter);
                }
                var value = multiBinding.ProvideValue(serviceProvider);
                return value;
            }
        }
    }
}
