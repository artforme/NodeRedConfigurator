using System.Windows;
using System.Windows.Controls;
using Presentation.ViewsModels;

namespace Presentation.Helpers;

public class ParameterTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextTemplate { get; set; }
    public DataTemplate NumberTemplate { get; set; }
    public DataTemplate BooleanTemplate { get; set; }
    public DataTemplate DropdownTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ChainParameterViewModel param)
        {
            return param.Type switch
            {
                "Text" => TextTemplate,
                "Number" => NumberTemplate,
                "Boolean" => BooleanTemplate,
                "Dropdown" => DropdownTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
        return base.SelectTemplate(item, container);
    }
}